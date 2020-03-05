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
using System.Threading.Tasks;
using System;

public class PreloadedVideo : MonoBehaviour
{
    public static string videoPathURI, json2DFileURI, json3DFileURI;
    public static GameObject uploadVideoPanel, selectPatternPanel, preloadVideoPanel;
    public static PreloadVideoList p;
    private static readonly HttpClient client = new HttpClient();

    public static async void PreloadVideoTask() {
        /**** Downloading Processes video, 2D points JSON and 3D points JSON files. 
              Periodic check if the files are ready.
              Copy the files in respective folders.
        ****/
        Debug.Log("PreloadVideoTask started");
        uploadVideoPanel = GameObject.Find("UploadVideoPanel");
		// selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");  // DELETE
        preloadVideoPanel = Credentials.FindInActiveObjectByName("PreloadVideoPanel");

        var values = new Dictionary<string, string>{{ "username", Login.sessionUser }};
		var content = new FormUrlEncodedContent(values);
        // var response, responseString;

        while(true) {
            var response = await client.PostAsync(Credentials.database_server_ip+"download_ready", content);
		    var responseString = await response.Content.ReadAsStringAsync();
            if(responseString == "NotReady") {
                Debug.Log("Files not ready");
                // TODO: Show text on UI for files not ready
                await Task.Delay(TimeSpan.FromSeconds(60));
                
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
                break;
            }
        }
    }

    private static void Completed(object sender, AsyncCompletedEventArgs e) {
        Debug.Log("Files Downloaded");
        p = FindObjectOfType<PreloadVideoList>();
        // TODO: Show text on UI for files ready
        uploadVideoPanel.SetActive(false);
        preloadVideoPanel.SetActive(true);
        p.GenerateButtons();
        // selectPatternPanel.SetActive(true);   // DELETE

        // Send notifications when files are ready
        
    }
}
