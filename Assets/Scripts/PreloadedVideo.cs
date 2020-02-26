using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.ComponentModel;

public class PreloadedVideo : MonoBehaviour
{
    public Button preloadBtn;
    public string videoPathURI, json2DFileURI, json3DFileURI;
    public GameObject uploadVideoPanel, selectPatternPanel;
    private static readonly HttpClient client = new HttpClient();
    void Start() {
        Button btn = preloadBtn.GetComponent<Button>();
        btn.onClick.AddListener(PreloadVideoTask);
        uploadVideoPanel = GameObject.Find("UploadVideoPanel");
		selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
    }

    public async void PreloadVideoTask() {
        /**** Downloading Processes video, 2D points JSON and 3D points JSON files. 
              Periodic check if the files are ready.
              Copy the files in respective folders.
        ****/
        Debug.Log("PreloadVideoTask started");
        var values = new Dictionary<string, string>{{ "username", Login.sessionUser }};
		var content = new FormUrlEncodedContent(values);
		var response = await client.PostAsync(Credentials.database_server_ip+"download_ready", content);
		var responseString = await response.Content.ReadAsStringAsync();

        if(responseString == "NotReady") {
            Debug.Log("Files not ready");
        }
        else {
            values = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            Debug.Log(values);
            videoPathURI = Credentials.database_server_ip + "return_files/Videos/" + values["VideoFileName"];
            json2DFileURI = Credentials.database_server_ip + "return_files/JSONfiles/" + values["Json2DFileName"];
            json3DFileURI = Credentials.database_server_ip + "return_files/JSONfiles/" + values["Json3DFileName"];

            WebClient webClient = new WebClient();
            // webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadFileAsync(new System.Uri(videoPathURI), Login.videoFilePath + "/" + values["VideoFileName"]);
            Debug.Log("Video downloaded at: " + Login.videoFilePath);

            WebClient webClient1 = new WebClient();
            webClient1.DownloadFileAsync(new System.Uri(json2DFileURI), Login.jsonFilePath + "/" + values["Json2DFileName"]);
            
            WebClient webClient2 = new WebClient();
            webClient2.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient2.DownloadFileAsync(new System.Uri(json3DFileURI), Login.jsonFilePath + "/" + values["Json3DFileName"]);
            Debug.Log("JSON files downloaded at: " + Login.jsonFilePath);
        }
    }

    private void Completed(object sender, AsyncCompletedEventArgs e) {
        Debug.Log("Files Downloaded");
        uploadVideoPanel.SetActive(false);
		selectPatternPanel.SetActive(true);
    }
}
