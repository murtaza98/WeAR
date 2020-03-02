using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class PreloadVideoList : MonoBehaviour
{
    public List<string> fileList; 
    public static string VideoName;
	public Dictionary<string, bool> fileDict;
    public GameObject prefab, selectPatternPanel, preloadVideoPanel;
    public Button newButton;
    void Start() {
        Debug.Log("PreloadVideoPanel");
        fileDict = new Dictionary<string, bool>();
		fileList = new List<string>();
        preloadVideoPanel = GameObject.Find("PreloadVideoPanel");
        selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
        GenerateButtons();
    }

    public void GenerateButtons() {
        Debug.Log("In Generate Buttons");
        if(Directory.Exists(Login.videoFilePath)) {
            Debug.Log(Login.videoFilePath);
            foreach (string file in System.IO.Directory.GetFiles(Login.videoFilePath)) {
                Debug.Log("GenrateButtons(): " + file);
                if(file.EndsWith(".mp4")) {
                    if(!fileDict.ContainsKey(file)) {
                        Debug.Log("PreloadVideoPanel: " + file);
                        fileDict.Add(file, false);
                    }
                }
            }
            PopulateButtons();
        }
        else {
            Debug.Log("SelectPreloadedVideo.GenerateButtons() ==> Directory not found");
        }
    }

    public void PopulateButtons() {
        Debug.Log("In PopulateButtons()");
        foreach(KeyValuePair<string, bool> entry in fileDict) {
            if(!entry.Value) {    
                GameObject newObj = (GameObject)Instantiate(prefab, transform);
                fileList.Add(entry.Key);
                string[] splitFileName = entry.Key.Split('/');

                newButton = newObj.GetComponent<Button>();
                newButton.GetComponentInChildren<Text>().text = splitFileName[splitFileName.Length-1];
                newButton.onClick.AddListener(OnClickButton);
            }
        }

        foreach(string file in fileList) {
			if(fileDict.ContainsKey(file)){
				fileDict[file] = true;
			}
		}
    }

    public void OnClickButton() {
        string videoName = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponentInChildren<Text>().text;
        VideoName = videoName;
        preloadVideoPanel.SetActive(false);
        selectPatternPanel.SetActive(true);
    }
}
