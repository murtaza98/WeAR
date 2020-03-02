using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System;
public class TrialRoom : MonoBehaviour
{
    public Button uploadImageBtn, tryClothBtn;
	public GameObject trialRoomPanel, selectPatternPanel, videoPlayer;

    void Start()
    {
        Button tryBtn = tryClothBtn.GetComponent<Button>();
		tryBtn.onClick.AddListener(TryClothTask);    
    }

    void TryClothTask() {
		trialRoomPanel = Credentials.FindInActiveObjectByName("TrailRoomPanel");
		videoPlayer = Credentials.FindInActiveObjectByName("VideoPlayerGO");
		selectPatternPanel = GameObject.Find("/Canvas/SelectPatternPanel");
		selectPatternPanel.SetActive(false);
		videoPlayer.SetActive(true);
		Debug.Log(Path.Combine(Login.videoFilePath, PreloadVideoList.VideoName));
		videoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>().url = Path.Combine(Login.videoFilePath, PreloadVideoList.VideoName);
		trialRoomPanel.SetActive(true);
	}
}
