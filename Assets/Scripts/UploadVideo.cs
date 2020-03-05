using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleFileBrowser;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

public class UploadVideo : MonoBehaviour
{
    public Button uploadBtn;
	public GameObject uploadVideoPanel, selectPatternPanel;
	public string poseEstimationServerIP = "192.168.43.8";
	public int portNo = 60000;
	
    void Start () {
		Button btn = uploadBtn.GetComponent<Button>();
		btn.onClick.AddListener(UploadVideoTask);
		uploadVideoPanel = GameObject.Find("UploadVideoPanel");
		selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		// FileBrowser.SetFilters(true, (".mp4"));
		// FileBrowser.SingleClickMode = true;
		yield return FileBrowser.WaitForLoadDialog( false, null, "Load File", "Load" );
		Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> Success, Result: " + FileBrowser.Success + ", " + FileBrowser.Result);
		
		if( FileBrowser.Success )
		{
			Debug.Log ("UploadVideo.ShowLoadDialogCoroutine() ===> Selecting video file");

			/** HTTP POST Request for sending file **/
			string fileName = FileBrowser.Result;
			Debug.Log("Video filename: " + fileName);

			/** Using HttpClient **/
			Upload(fileName);
			}
	}

	private async void Upload(string fileName) {
		byte[] fileContents = File.ReadAllBytes(fileName);

		Uri webService = new Uri(Credentials.database_server_ip+"upload_video");
		HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, webService);
		requestMessage.Headers.ExpectContinue = false;

		MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----MyGreatBoundary");
		ByteArrayContent byteArrayContent = new ByteArrayContent(fileContents);
		byteArrayContent.Headers.Add("Content-Type", "application/octet-stream");
		
		StringContent stringContent = new StringContent(Login.sessionUser);
		
		multiPartContent.Add(byteArrayContent, "file", fileName);
		multiPartContent.Add(stringContent, "username");
		requestMessage.Content = multiPartContent;

		HttpClient httpClient = new HttpClient();
		Task<HttpResponseMessage> httpRequest = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
		HttpResponseMessage httpResponse = httpRequest.Result;
	}	

	void UploadVideoTask(){
		StartCoroutine( ShowLoadDialogCoroutine() );
	}
}
