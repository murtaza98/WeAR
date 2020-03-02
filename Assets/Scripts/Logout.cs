using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logout : MonoBehaviour
{
    public Button logoutBtn;
    public GameObject uploadVideoPanel, uploadClothPanel, loginPanel, selectPatternPanel, preloadVideoPanel;

    void Start()
    {
    	Button btn = logoutBtn.GetComponent<Button>();
		btn.onClick.AddListener(LogoutTask);
    }

    void LogoutTask(){
		Debug.Log("Logout.LogoutTask() ===> Logout button clicked");
		
		loginPanel = Credentials.FindInActiveObjectByName("LoginPanel");
    	uploadVideoPanel = Credentials.FindInActiveObjectByName("UploadVideoPanel");
    	uploadClothPanel = Credentials.FindInActiveObjectByName("UploadClothPanel");
		selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
		preloadVideoPanel = Credentials.FindInActiveObjectByName("PreloadVideoPanel");

    	/*** TODO: Exception handling (Null Exception) ***/
    	uploadVideoPanel.SetActive(false);
    	uploadClothPanel.SetActive(false);
		selectPatternPanel.SetActive(false);
		preloadVideoPanel.SetActive(false);
    	loginPanel.SetActive(true);
    }
}
