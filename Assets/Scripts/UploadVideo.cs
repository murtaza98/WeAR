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
		selectPatternPanel = FindInActiveObjectByName("SelectPatternPanel");
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
			try {  
				// IPHostEntry ipHost = Dns.GetHostEntry("https://0c7cd9c4.ngrok.io"); 
				// IPAddress ipAddr = ipHost.AddressList[0]; 
				// Debug.Log(ipHost.AddressList[0]);
				// IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 7000);
				IPAddress ipAddr = IPAddress.Parse(Credentials.pose_estimation_server_ip);
		        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 60000); 
		 		Debug.Log("Local End point: " + localEndPoint);
		        Socket sender = new Socket(ipAddr.AddressFamily, 
		                   SocketType.Stream, ProtocolType.Tcp); 
		  		Debug.Log("Sender socket: " + sender);
		        try { 
		            sender.Connect(localEndPoint); 
		            Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> Socket connected to: " + sender.RemoteEndPoint.ToString());
		            string fileName = FileBrowser.Result;
		            Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> Sending file: " + fileName);
		            sender.Send(FileBrowserHelpers.ReadBytesFromFile(fileName));
		            sender.Shutdown(SocketShutdown.Both);  
		            sender.Close(); 
					// Call script 
					PreloadedVideo.PreloadVideoTask();
		        }   
		        catch (ArgumentNullException ane) { 
		            Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> ArgumentNullException: " + ane.ToString()); 
		        } 
		        catch (SocketException se) { 
		            Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> SocketException: " + se.ToString()); 
		        } 
		        catch (Exception e) { 
		            Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> Unexpected exception: " + e.ToString()); 
		        } 
		    } 
		    catch (Exception e) { 
		        Debug.Log("UploadVideo.ShowLoadDialogCoroutine() ===> " + e.ToString()); 
		    } 
		    // uploadVideoPanel.SetActive(false);
		    // selectPatternPanel.SetActive(true);
		}
	}

	// Same in Logout.cs
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

	void UploadVideoTask(){
		StartCoroutine( ShowLoadDialogCoroutine() );
	}
}
