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
using Unity.Notifications.Android;

public class PreloadedVideo : MonoBehaviour
{
    public static string videoPathURI, json2DFileURI, json3DFileURI;
    public static GameObject uploadVideoPanel, selectPatternPanel, preloadVideoPanel, msgPanel;
    public static PreloadVideoList p;
    private static readonly HttpClient client = new HttpClient();
    public static Text preprocessText;
    
    public static async void PreloadVideoTask() {
        /**** Downloading Processes video, 2D points JSON and 3D points JSON files. 
              Periodic check if the files are ready.
              Copy the files in respective folders.
        ****/
        Debug.Log("PreloadVideoTask started");
        uploadVideoPanel = GameObject.Find("UploadVideoPanel");
        Debug.Log("UVP: " + uploadVideoPanel);
        msgPanel = Credentials.FindInActiveObjectByName("PreprocessMessagePanel");
        Debug.Log("MSGP: " + msgPanel);
        preloadVideoPanel = Credentials.FindInActiveObjectByName("PreloadVideoPanel");

        var values = new Dictionary<string, string>{{ "username", Login.sessionUser }};
		var content = new FormUrlEncodedContent(values);
        Debug.Log("sfsdf");
        // preprocessText = msgPanel.GetComponent<Text>();
        Debug.Log("PT: ", preprocessText);

        while(true) {
            var response = await client.PostAsync(Credentials.database_server_ip+"download_ready", content);
		    var responseString = await response.Content.ReadAsStringAsync();
            if(responseString == "NotReady") {
                Debug.Log("Files not ready");
                msgPanel.SetActive(true);
                // preprocessText.text = "Files not ready!!!";
                await Task.Delay(TimeSpan.FromSeconds(60));
                
            }
            else {
                values = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                Debug.Log(values);
                videoPathURI = Credentials.database_server_ip + "return_files/" + values["VideoFileName"];
                json2DFileURI = Credentials.database_server_ip + "return_files/" + values["Json2DFileName"];
                json3DFileURI = Credentials.database_server_ip + "return_files/" + values["Json3DFileName"];

                Debug.Log(videoPathURI);
                Debug.Log(json2DFileURI );

                WebClient webClient = new WebClient();
                webClient.DownloadFile(new System.Uri(videoPathURI), Login.videoFilePath + "/" + values["VideoFileName"].Split('/')[1]);
                Debug.Log("Video downloaded at: " + Login.videoFilePath);

                WebClient webClient1 = new WebClient();
                webClient1.DownloadFile(new System.Uri(json2DFileURI), Login.jsonFilePath + "/" + values["Json2DFileName"].Split('/')[1]);
                
                WebClient webClient2 = new WebClient();
                webClient2.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient2.DownloadFile(new System.Uri(json3DFileURI), Login.jsonFilePath + "/" + values["Json3DFileName"].Split('/')[1]);
                Debug.Log("JSON files downloaded at: " + Login.jsonFilePath);
                break;
            }
        }
    }

    private static void Completed(object sender, AsyncCompletedEventArgs e) {
        Debug.Log("Files Downloaded");
        msgPanel.SetActive(true);
        // preprocessText.text = "Files Downloaded. Click on select preloaded video button";
        p = FindObjectOfType<PreloadVideoList>();

        // Send notifications when files are ready
        var c = new AndroidNotificationChannel() {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);

        var notification = new AndroidNotification();
        notification.Title = "SomeTitle";
        notification.Text = "SomeText";
        notification.FireTime = System.DateTime.Now;

        AndroidNotificationCenter.SendNotification(notification, "channel_id");

        // TODO: Show text on UI for files ready
        uploadVideoPanel.SetActive(false);
        preloadVideoPanel.SetActive(true);
        p.GenerateButtons();
        // selectPatternPanel.SetActive(true);   // DELETE
    }
}
