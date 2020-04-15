using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System.IO;
using UnityEngine.Android;

public class Login : MonoBehaviour
{
    public Button loginBtn;
	public static string sessionUser;
	public static string path, jsonFilePath, videoFilePath;
    public GameObject uploadVideoPanel, uploadClothPanel, loginPanel, selectPatternPanel, trailRoomPanel, videoPlayer, preloadVideoPanel;
    public InputField unameField, passwdField;
    public Text errorText;
    private static readonly HttpClient client = new HttpClient();

    void Start () {
		// Creating directory for saving patterns, videos and 2D/3D JSON files
		path = Path.Combine(Application.persistentDataPath, "Pattern");
		jsonFilePath = Path.Combine(Application.persistentDataPath, "JsonFiles");
		videoFilePath = Path.Combine(Application.persistentDataPath, "Videos");
		Debug.Log(videoFilePath);
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		if(!Directory.Exists(jsonFilePath)) {
			Directory.CreateDirectory(jsonFilePath);
			Debug.Log("JSON FOLDER CREATED");
		}

		if(!Directory.Exists(videoFilePath)) {
			Directory.CreateDirectory(videoFilePath);
			Debug.Log("VIDEO FOLDER CREATED");
		}
		
		// Asking for storage permission
		if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead)) {
			Permission.RequestUserPermission(Permission.ExternalStorageRead);
		}

    	uploadVideoPanel = GameObject.Find("UploadVideoPanel");
    	uploadClothPanel = GameObject.Find("UploadClothPanel");
    	loginPanel = GameObject.Find("LoginPanel");
    	selectPatternPanel = GameObject.Find("SelectPatternPanel");
		trailRoomPanel = GameObject.Find("TrailRoomPanel");
		videoPlayer = GameObject.Find("VideoPlayerGO");
		preloadVideoPanel = GameObject.Find("PreloadVideoPanel");

		uploadVideoPanel.SetActive(false);
    	uploadClothPanel.SetActive(false);
    	selectPatternPanel.SetActive(false);
		trailRoomPanel.SetActive(false);
		videoPlayer.SetActive(false);
		preloadVideoPanel.SetActive(false);
		
    	unameField = GameObject.Find("UsernameInput").GetComponent<InputField>();
    	passwdField = GameObject.Find("PasswordInput").GetComponent<InputField>();

		Button btn = loginBtn.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		Debug.Log("Login.TaskOnClick() ===> Login Button Clicked");
		/*** HARDCODED LOGIN ***/
		if(unameField.text == "murtaza"){
			loginPanel.SetActive(false);
			uploadVideoPanel.SetActive(true);
		}
		else {
			// Invalid credentials display message
			errorText = GameObject.Find("ErrorText").GetComponent<Text>();
			errorText.text = "Invalid Credentials!!!";
		}
		sessionUser = unameField.text;  // TODO: Comment after testing
		/*** ------------------------- ***/

    	// CheckLogin(unameField.text, passwdField.text);
    }

    public async void CheckLogin(string username, string password){
    	var values = new Dictionary<string, string>{{ "username", username },{ "password", password }};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsync(Credentials.database_server_ip+"login", content);

		var responseString = await response.Content.ReadAsStringAsync();
		Debug.Log("Login.CheckLogin() ===> Response from database server: " + responseString);

		if(responseString == "success"){
			sessionUser = username;
			loginPanel.SetActive(false);
			uploadVideoPanel.SetActive(true);
		}
		else {
			// Invalid Credentials message display
		}
    }
}
