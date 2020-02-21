using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SimpleFileBrowser;

public class UploadClothModel : MonoBehaviour
{
    public Button uploadClothBtn;
    Texture2D myTexture;
	public string clothModelServerIP = "192.168.43.8";
	public int port_no = 50000;
    void Start () {
		Button btn = uploadClothBtn.GetComponent<Button>();
		btn.onClick.AddListener(UploadVideoTask);
	}
	
	IEnumerator ShowLoadDialogCoroutine()
	{
		/*** TODO: Add file extension filter and request permission ***/
		Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Permission: " + FileBrowser.CheckPermission());
		yield return FileBrowser.WaitForLoadDialog( false, null, "Load File", "Load" );
		Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> File Dialog Opened: " + FileBrowser.Success);
		Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> File Dialog Result: " + FileBrowser.Result);

		if( FileBrowser.Success )
		{
			Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Selecting cloth model file");
			try {  
				IPAddress ipAddr = IPAddress.Parse(clothModelServerIP);
		        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port_no); 
		 
		        Socket sender = new Socket(ipAddr.AddressFamily, 
		                   SocketType.Stream, ProtocolType.Tcp); 
		  
		        try { 
		            sender.Connect(localEndPoint); 
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Socket connected to: " + sender.RemoteEndPoint.ToString());
		            string fileName = FileBrowser.Result;
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Sending file: " + fileName);
		            sender.Send(FileBrowserHelpers.ReadBytesFromFile(fileName));
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> File sent");
		            sender.Shutdown(SocketShutdown.Both);  
		            sender.Close(); 
		        }   
		        catch (ArgumentNullException ane) { 
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> ArgumentNullException: " + ane.ToString()); 
		        } 
		        catch (SocketException se) { 
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> SocketException: " + se.ToString()); 
		        } 
		        catch (Exception e) { 
		            Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Unexpected exception: " + e.ToString()); 
		        } 
		    } 
		    catch (Exception e) { 
		        Debug.Log("UploadClothModel.ShowLoadDialogCoroutine() ===> Other exception: " + e.ToString()); 
		    } 

		}
	}

	void UploadVideoTask(){
		StartCoroutine( ShowLoadDialogCoroutine() );
	}
}
