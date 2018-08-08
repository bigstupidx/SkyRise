using UnityEditor;
using UnityEngine;

public class EditorExtender : MonoBehaviour
{
	
	[MenuItem("Help/DeletePrefs")]
	static void DoSomething()
	{
		PlayerPrefs.DeleteAll ();
	}

}