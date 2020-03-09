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
	public GameObject trialRoomPanel, selectPatternPanel, videoPlayer, bounds;

    void Start()
    {
        Button tryBtn = tryClothBtn.GetComponent<Button>();
		tryBtn.onClick.AddListener(TryClothTask);    
    }

    void TryClothTask() {
		trialRoomPanel = Credentials.FindInActiveObjectByName("TrailRoomPanel");
		videoPlayer = Credentials.FindInActiveObjectByName("VideoPlayerGO");
		bounds = Credentials.FindInActiveObjectByName("bounds");
		if(bounds == null) {
			bounds.SetActive(true);
		}
		selectPatternPanel = GameObject.Find("/Canvas/SelectPatternPanel");
		selectPatternPanel.SetActive(false);
		videoPlayer.SetActive(true);
		Debug.Log(Path.Combine("file://" + Login.videoFilePath, PreloadVideoList.VideoName + ".mp4"));
		videoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>().url = Path.Combine("file://" + Login.videoFilePath, PreloadVideoList.VideoName + ".mp4");
		trialRoomPanel.SetActive(true);
		// Handheld.PlayFullScreenMovie(Path.Combine(Login.videoFilePath, PreloadVideoList.VideoName + ".mp4"), Color.black, FullScreenMovieControlMode.Full,  FullScreenMovieScalingMode.AspectFit);
	}
}
