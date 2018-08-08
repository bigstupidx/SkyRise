using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;

[System.Serializable]
public struct IAPProduct {
	public string sku;
	public ProductType type;
}

public class MyIAPManager : MonoBehaviour, IStoreListener {

    IStoreController controller;
    IExtensionProvider extensions;
	[SerializeField]
	IAPProduct[] products;

	void Awake () {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		foreach (IAPProduct product in products) {
			builder.AddProduct (product.sku, product.type);
		}
		UnityPurchasing.Initialize (this, builder);
    }

    public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
    {
		Debug.Log ("initsuccess");
        this.controller = controller;
        this.extensions = extensions;
		InitPurchase ("com.skyrise.removeads");
    }

    public void OnInitializeFailed (InitializationFailureReason error)
    {
		Debug.Log ("initfailed");
    }

  	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
    {
		Debug.Log ("inappsuccess" + e.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
    {
		Debug.Log ("inappfailed" + i.definition.id + ", " + p.ToString ());
    }

	public void InitPurchase(string sku) {
		if (controller == null)
		{
			Debug.LogError("Purchasing is not initialized");
			return;
		}

		if (controller.products.WithID(sku) == null)
		{
			Debug.LogError("invalid sku");
			return;
		}

		controller.InitiatePurchase (sku);
	}

}