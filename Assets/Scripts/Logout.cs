using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logout : MonoBehaviour
{
    public Button logoutBtn;
    public GameObject uploadVideoPanel, uploadClothPanel, loginPanel, selectPatternPanel;

    void Start()
    {
    	Button btn = logoutBtn.GetComponent<Button>();
		btn.onClick.AddListener(LogoutTask);
    }

    void LogoutTask(){
		Debug.Log("Logout.LogoutTask() ===> Logout button clicked");
		
		loginPanel = FindInActiveObjectByName("LoginPanel");
    	uploadVideoPanel = FindInActiveObjectByName("UploadVideoPanel");
    	uploadClothPanel = FindInActiveObjectByName("UploadClothPanel");
		selectPatternPanel = FindInActiveObjectByName("SelectPatternPanel");

    	/*** TODO: Exception handling (Null Exception) ***/
    	uploadVideoPanel.SetActive(false);
    	uploadClothPanel.SetActive(false);
		selectPatternPanel.SetActive(false);
    	loginPanel.SetActive(true);
    }

	// Same in UploadVideo.cs
	/*** TODO: Make this method common ***/
    GameObject FindInActiveObjectByName(string name)
	{
	    Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
	    for (int i = 0; i < objs.Length; i++)
	    {
	        if (objs[i].hideFlags == HideFlags.None)
	        {
	            if (objs[i].name == name)
	            {
	                return objs[i].gameObject;
	            }
	        }
	    }
	    return null;
	}
}
