using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
using SimpleFileBrowser;

public class UploadPatternImage : MonoBehaviour
{
    public Button uploadImageBtn;
	public string path;
	public PopulateLayout p;
    public Text errorText;

	void Start()
    {
		p = FindObjectOfType<PopulateLayout>();
		Button btn = uploadImageBtn.GetComponent<Button>();
		btn.onClick.AddListener(UploadImageTask);
		errorText = GameObject.Find("ErrorText").GetComponent<Text>();	
	}   

	void UploadImageTask() {
		StartCoroutine( ShowLoadDialogCoroutine() );
	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		// FileBrowser.SetFilters(true, (".png", ".jpg", ".jpeg"));
		// FileBrowser.SingleClickMode = true;
		Debug.Log("UploadPatternImage.ShowLoadDialogCoroutine() ===> Permission: " + FileBrowser.CheckPermission());
		
		yield return FileBrowser.WaitForLoadDialog( false, null, "Load File", "Load" );
		Debug.Log("UploadPatternImage.ShowLoadDialogCoroutine() ===> Success, Result: " +FileBrowser.Success + ", " + FileBrowser.Result);

		if( FileBrowser.Success )
		{
			Debug.Log ("UploadPatternImage.ShowLoadDialogCoroutine() ===> Selecting Pattern");
			string fileName = FileBrowser.Result;
		    Debug.Log("UploadPatternImage.ShowLoadDialogCoroutine() ===> Saving file: " + fileName);

			path = Path.Combine(Application.persistentDataPath, "Pattern");
			
			if(fileName.EndsWith(".png") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg")) {
				if(Directory.Exists(path)) {
					/*** TODO extract filename from path ***/
					string[] splitFileName = fileName.Split('/');
					path = Path.Combine(path, splitFileName[splitFileName.Length-1]);
					Debug.Log("New Path: " + path);
					if(!Directory.Exists(path)) {
						File.WriteAllBytes(path, FileBrowserHelpers.ReadBytesFromFile(fileName));
					}
					else {
						Debug.Log("UploadPatternImage.ShowLoadDialogCoroutine() ===> File already Uploaded");
					}
				}
				else {
					Debug.Log("UploadPatternImage.ShowLoadDialogCoroutine() ===> Directory not found: " + path);
				}			
				errorText.text = "Pattern uploaded!!!";
				path = Path.Combine(Application.persistentDataPath, "Pattern");
				p.GenerateList(path);
			}
			else {
				errorText.text = "Only png, jpg, jpeg formats supported!!!";
			}
		}
	}
}
