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
	public string path, database_server_ip = "http://192.168.43.8:5000/";
    public GameObject uploadVideoPanel, uploadClothPanel, loginPanel, selectPatternPanel, trailRoomPanel;
    public InputField unameField, passwdField;
    public Dropdown roleField;
    private static readonly HttpClient client = new HttpClient();

    void Start () {
		// Creating a Directory Pattern at path for saving uploaded pattern
		path = Path.Combine(Application.persistentDataPath, "Pattern"); 
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
			Debug.Log("Login.Start() ===> Creating Directory at " + path);
		}
		else {
			Debug.Log("Login.Start() ===> Directory exists at " + path);
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

		uploadVideoPanel.SetActive(false);
    	uploadClothPanel.SetActive(false);
    	selectPatternPanel.SetActive(false);
		trailRoomPanel.SetActive(true);
        loginPanel.SetActive(false);

    	unameField = GameObject.Find("UsernameInput").GetComponent<InputField>();
    	passwdField = GameObject.Find("PasswordInput").GetComponent<InputField>();
    	roleField = GameObject.Find("RoleDropdown").GetComponent<Dropdown>();

		Button btn = loginBtn.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		Debug.Log("Login.TaskOnClick() ===> Login Button Clicked");
		string role = roleField.options[roleField.value].text;
		loginPanel.SetActive(false);
		uploadVideoPanel.SetActive(true);
    	// CheckLogin(unameField.text, passwdField.text, role);
    }

    public async void CheckLogin(string username, string password, string role){
    	var values = new Dictionary<string, string>{{ "username", username },{ "password", password },{"role", role}};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsync(database_server_ip, content);

		var responseString = await response.Content.ReadAsStringAsync();
		Debug.Log("Login.CheckLogin() ===> Response from database server: " + responseString);

		if(responseString == "success"){
			loginPanel.SetActive(false);
			if(role == "User"){
    			uploadVideoPanel.SetActive(true);
			}
			else{
				uploadClothPanel.SetActive(true);
			}
		}
    }
}
