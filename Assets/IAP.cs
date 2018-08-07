using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Store; // UnityChannel


public class IAP : MonoBehaviour, IStoreListener
{
	public static IAP instance;
	// Unity IAP objects
	private IStoreController m_Controller;

	private IAppleExtensions m_AppleExtensions;
	private IMoolahExtension m_MoolahExtensions;
	private ISamsungAppsExtensions m_SamsungExtensions;
	private IMicrosoftExtensions m_MicrosoftExtensions;
	private IUnityChannelExtensions m_UnityChannelExtensions;
	private ITransactionHistoryExtensions m_TransactionHistoryExtensions;

#pragma warning disable 0414
	private bool m_IsGooglePlayStoreSelected;
#pragma warning restore 0414
	private bool m_IsSamsungAppsStoreSelected;
	private bool m_IsCloudMoolahStoreSelected;
	private bool m_IsUnityChannelSelected;

	private string m_LastTransactionID;
	private bool m_IsLoggedIn;
	private UnityChannelLoginHandler unityChannelLoginHandler; // Helper for interfacing with UnityChannel API
	private bool m_FetchReceiptPayloadOnPurchase = false;

	private bool m_PurchaseInProgress;

	private Dictionary<string, IAPDemoProductUI> m_ProductUIs = new Dictionary<string, IAPDemoProductUI>();

	/// <summary>
	/// This will be called when Unity IAP has finished initialising.
	/// </summary>
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions>();
		m_MoolahExtensions = extensions.GetExtension<IMoolahExtension>();
		m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();
		m_UnityChannelExtensions = extensions.GetExtension<IUnityChannelExtensions>();
		m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();


		// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
		// On non-Apple platforms this will have no effect; OnDeferred will never be called.
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

		Debug.Log("Available items:");
		foreach (var item in controller.products.all)
		{
			if (item.availableToPurchase)
			{
				Debug.Log(string.Join(" - ",
					new[]
					{
						item.metadata.localizedTitle,
						item.metadata.localizedDescription,
						item.metadata.isoCurrencyCode,
						item.metadata.localizedPrice.ToString(),
						item.metadata.localizedPriceString,
						item.transactionID,
						item.receipt
					}));

			}
		}

		// Populate the product menu now that we have Products

		LogProductDefinitions();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		Debug.Log("Receipt: " + e.purchasedProduct.receipt);

		m_LastTransactionID = e.purchasedProduct.transactionID;
		m_PurchaseInProgress = false;

		// Decode the UnityChannelPurchaseReceipt, extracting the gameOrderId
		if (m_IsUnityChannelSelected)
		{
			var unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(e.purchasedProduct.receipt);
			if (unifiedReceipt != null && !string.IsNullOrEmpty(unifiedReceipt.Payload))
			{
				var purchaseReceipt = JsonUtility.FromJson<UnityChannelPurchaseReceipt>(unifiedReceipt.Payload);
				Debug.LogFormat(
					"UnityChannel receipt: storeSpecificId = {0}, transactionId = {1}, orderQueryToken = {2}",
					purchaseReceipt.storeSpecificId, purchaseReceipt.transactionId, purchaseReceipt.orderQueryToken);
			}
		}

		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// This will be called if an attempted purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("Purchase failed: " + item.definition.id);
		Debug.Log(r);

		// Detailed debugging information
		Debug.Log("Store specific error code: " + m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
		if (m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
		{
			Debug.Log("Purchase failure description message: " +
				m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
		}

		if (m_IsUnityChannelSelected)
		{
			var extra = m_UnityChannelExtensions.GetLastPurchaseError();
			var purchaseError = JsonUtility.FromJson<UnityChannelPurchaseError>(extra);

			if (purchaseError != null && purchaseError.purchaseInfo != null)
			{
				// Additional information about purchase failure.
				var purchaseInfo = purchaseError.purchaseInfo;
				Debug.LogFormat(
					"UnityChannel purchaseInfo: productCode = {0}, gameOrderId = {1}, orderQueryToken = {2}",
					purchaseInfo.productCode, purchaseInfo.gameOrderId, purchaseInfo.orderQueryToken);
			}
			if (r == PurchaseFailureReason.DuplicateTransaction)
			{
				// Unlock `item` in inventory if not already present.
				Debug.Log("Duplicate transaction detected, unlock this item");
			}

		}

		m_PurchaseInProgress = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("Billing failed to initialize!");
		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			// Ask the user if billing is disabled in device settings.
			Debug.Log("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			// Developer configuration error; check product metadata.
			Debug.Log("No products available for purchase!");
			break;
		}
	}

	[Serializable]
	public class UnityChannelPurchaseError
	{
		public string error;
		public UnityChannelPurchaseInfo purchaseInfo;
	}

	[Serializable]
	public class UnityChannelPurchaseInfo
	{
		public string productCode; // Corresponds to storeSpecificId
		public string gameOrderId; // Corresponds to transactionId
		public string orderQueryToken;
	}

	void Start()
	{
		instance = this;

		var module = StandardPurchasingModule.Instance();

		// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and
		// developer ui (initialization, purchase, failure code setting). These correspond to
		// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

		var builder = ConfigurationBuilder.Instance(module);

		// Set this to true to enable the Microsoft IAP simulator for local testing.
		builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;

		m_IsGooglePlayStoreSelected =
			Application.platform == RuntimePlatform.Android && module.appStore == AppStore.GooglePlay;

		// CloudMoolah Configuration setings
		// All games must set the configuration. the configuration need to apply on the CloudMoolah Portal.
		// CloudMoolah APP Key
		builder.Configure<IMoolahConfiguration>().appKey = "d93f4564c41d463ed3d3cd207594ee1b";
		// CloudMoolah Hash Key
		builder.Configure<IMoolahConfiguration>().hashKey = "cc";
		// This enables the CloudMoolah test mode for local testing.
		// You would remove this, or set to CloudMoolahMode.Production, before building your release package.
		builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.AlwaysSucceed);
		// This records whether we are using Cloud Moolah IAP.
		// Cloud Moolah requires logging in to access your Digital Wallet, so:
		// A) IAPDemo (this) displays the Cloud Moolah GUI button for Cloud Moolah
		m_IsCloudMoolahStoreSelected =
			Application.platform == RuntimePlatform.Android && module.appStore == AppStore.CloudMoolah;

		// UnityChannel, provides access to Xiaomi MiPay.
		// Products are required to be set in the IAP Catalog window.  The file "MiProductCatalog.prop"
		// is required to be generated into the project's
		// Assets/Plugins/Android/assets folder, based off the contents of the
		// IAP Catalog window, for MiPay.
		m_IsUnityChannelSelected =
			Application.platform == RuntimePlatform.Android && module.appStore == AppStore.XiaomiMiPay;
		// UnityChannel supports receipt validation through a backend fetch.
		builder.Configure<IUnityChannelConfiguration>().fetchReceiptPayloadOnPurchase = m_FetchReceiptPayloadOnPurchase;

		// Define our products.
		// Either use the Unity IAP Catalog, or manually use the ConfigurationBuilder.AddProduct API.
		// Use IDs from both the Unity IAP Catalog and hardcoded IDs via the ConfigurationBuilder.AddProduct API.

		builder.AddProduct ("com.skyrise.removeads", ProductType.NonConsumable);


//		builder.AddProduct("100.gold.coins", ProductType.Consumable, new IDs
//			{
//				{"com.unity3d.unityiap.unityiapdemo.100goldcoins.7", MacAppStore.Name},
//				{"000000596586", TizenStore.Name},
//				{"com.ff", MoolahAppStore.Name},
//				{"100.gold.coins", AmazonApps.Name}
//			}
//		);

//		builder.AddProduct ("sword", ProductType.NonConsumable, new IDs {
//			{ "com.unity3d.unityiap.unityiapdemo.sword.7", MacAppStore.Name },
//			{ "000000596583", TizenStore.Name },
//			{ "sword", AmazonApps.Name }
//		}
//		);


		// Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
		// This should be removed from a production build.
		//builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

		// This enables simulated purchase success for Samsung IAP.
		// You would remove this, or set to SamsungAppsMode.Production, before building your release package.
		builder.Configure<ISamsungAppsConfiguration>().SetMode(SamsungAppsMode.AlwaysSucceed);
		// This records whether we are using Samsung IAP. Currently ISamsungAppsExtensions.RestoreTransactions
		// displays a blocking Android Activity, so:
		// A) Unity IAP does not automatically restore purchases on Samsung Galaxy Apps
		// B) IAPDemo (this) displays the "Restore" GUI button for Samsung Galaxy Apps
		m_IsSamsungAppsStoreSelected =
			Application.platform == RuntimePlatform.Android && module.appStore == AppStore.SamsungApps;


		// This selects the GroupId that was created in the Tizen Store for this set of products
		// An empty or non-matching GroupId here will result in no products available for purchase
		builder.Configure<ITizenStoreConfiguration>().SetGroupId("100000085616");

		Action initializeUnityIap = () =>
		{
			// Now we're ready to initialize Unity IAP.
			UnityPurchasing.Initialize(this, builder);
		};

		bool needExternalLogin = m_IsUnityChannelSelected;

		if (!needExternalLogin)
		{
			initializeUnityIap();
		}
		else
		{
			// Call UnityChannel initialize and (later) login asynchronously

			// UnityChannel configuration settings. Required for Xiaomi MiPay.
			// Collect this app configuration from the Unity Developer website at
			// [2017-04-17 PENDING - Contact support representative]
			// https://developer.cloud.unity3d.com/ providing your Xiaomi MiPay App
			// ID, App Key, and App Secret. This permits Unity to proxy from the
			// user's device into the MiPay system.
			// IMPORTANT PRE-BUILD STEP: For mandatory Chinese Government app auditing
			// and for MiPay testing, enable debug mode (test mode)
			// using the `AppInfo.debug = true;` when initializing Unity Channel.

//			AppInfo unityChannelAppInfo = new AppInfo();
//			unityChannelAppInfo.appId = "abc123appId";
//			unityChannelAppInfo.appKey = "efg456appKey";
//			unityChannelAppInfo.clientId = "hij789clientId";
//			unityChannelAppInfo.clientKey = "klm012clientKey";
//			unityChannelAppInfo.debug = false;

			// Shared handler for Unity Channel initialization, here, and login, later
			unityChannelLoginHandler = new UnityChannelLoginHandler();
			unityChannelLoginHandler.initializeFailedAction = (string message) =>
			{
				Debug.LogError("Failed to initialize and login to UnityChannel: " + message);
			};
			unityChannelLoginHandler.initializeSucceededAction = () => { initializeUnityIap(); };

//			StoreService.Initialize(unityChannelAppInfo, unityChannelLoginHandler);
		}
	}

	// For handling initialization and login of UnityChannel, returning control to our store after.
	class UnityChannelLoginHandler : ILoginListener
	{
		internal Action initializeSucceededAction;
		internal Action<string> initializeFailedAction;
		internal Action<UserInfo> loginSucceededAction;
		internal Action<string> loginFailedAction;

		public void OnInitialized()
		{
			initializeSucceededAction();
		}

		public void OnInitializeFailed(string message)
		{
			initializeFailedAction(message);
		}

		public void OnLogin(UserInfo userInfo)
		{
			loginSucceededAction(userInfo);
		}

		public void OnLoginFailed(string message)
		{
			loginFailedAction(message);
		}
	}

	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("Transactions restored.");
	}

	/// <summary>
	/// iOS Specific.
	/// This is called as part of Apple's 'Ask to buy' functionality,
	/// when a purchase is requested by a minor and referred to a parent
	/// for approval.
	///
	/// When the purchase is approved or rejected, the normal purchase events
	/// will fire.
	/// </summary>
	/// <param name="item">Item.</param>
	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);
	}

	public void RemoveAds() {
		PurchaseButtonClick ("com.skyrise.removeads");
	}

	void PurchaseButtonClick(string productID)
	{
		if (m_PurchaseInProgress == true)
		{
			Debug.Log("Please wait, purchase in progress");
			return;
		}

		if (m_Controller == null)
		{
			Debug.LogError("Purchasing is not initialized");
			return;
		}

		if (m_Controller.products.WithID(productID) == null)
		{
			Debug.LogError("No product has id " + productID);
			return;
		}

		// Don't need to draw our UI whilst a purchase is in progress.
		// This is not a requirement for IAP Applications but makes the demo
		// scene tidier whilst the fake purchase dialog is showing.
		m_PurchaseInProgress = true;
		m_Controller.InitiatePurchase(m_Controller.products.WithID(productID), "aDemoDeveloperPayload");
	}

	public void RestoreButtonClick()
	{
		if (m_IsCloudMoolahStoreSelected)
		{
			if (m_IsLoggedIn == false)
			{
				Debug.LogError("CloudMoolah purchase restoration aborted. Login incomplete.");
			}
			else
			{
				// Restore abnornal transaction identifer, if Client don't receive transaction identifer.
				m_MoolahExtensions.RestoreTransactionID((RestoreTransactionIDState restoreTransactionIDState) =>
					{
						Debug.Log("restoreTransactionIDState = " + restoreTransactionIDState.ToString());
						bool success =
							restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed &&
							restoreTransactionIDState != RestoreTransactionIDState.NotKnown;
						OnTransactionsRestored(success);
					});
			}
		}
		else if (m_IsSamsungAppsStoreSelected)
		{
			m_SamsungExtensions.RestoreTransactions(OnTransactionsRestored);
		}
		else if (Application.platform == RuntimePlatform.WSAPlayerX86 ||
			Application.platform == RuntimePlatform.WSAPlayerX64 ||
			Application.platform == RuntimePlatform.WSAPlayerARM)
		{
			m_MicrosoftExtensions.RestoreTransactions();
		}
		else
		{
			m_AppleExtensions.RestoreTransactions(OnTransactionsRestored);
		}
	}

	public void LoginButtonClick()
	{
		if (!m_IsUnityChannelSelected)
		{
			Debug.Log("Login is only required for the Xiaomi store");
			return;
		}

		unityChannelLoginHandler.loginSucceededAction = (UserInfo userInfo) =>
		{
			m_IsLoggedIn = true;
			Debug.LogFormat("Succeeded logging into UnityChannel. channel {0}, userId {1}, userLoginToken {2} ",
				userInfo.channel, userInfo.userId, userInfo.userLoginToken);
		};

		unityChannelLoginHandler.loginFailedAction = (string message) =>
		{
			m_IsLoggedIn = false;
			Debug.LogError("Failed logging into UnityChannel. " + message);
		};

		StoreService.Login(unityChannelLoginHandler);
	}

	private bool NeedRestoreButton()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer ||
			Application.platform == RuntimePlatform.OSXPlayer ||
			Application.platform == RuntimePlatform.tvOS ||
			Application.platform == RuntimePlatform.WSAPlayerX86 ||
			Application.platform == RuntimePlatform.WSAPlayerX64 ||
			Application.platform == RuntimePlatform.WSAPlayerARM ||
			m_IsSamsungAppsStoreSelected ||
			m_IsCloudMoolahStoreSelected;
	}


	private void LogProductDefinitions()
	{
		var products = m_Controller.products.all;
		foreach (var product in products)
		{
			Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\nenabled: {3}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString(), product.definition.enabled ? "enabled" : "disabled"));
		}
	}
}
