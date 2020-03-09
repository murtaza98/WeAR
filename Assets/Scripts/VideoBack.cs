using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoBack : MonoBehaviour
{
    public GameObject bounds, trialRoomPanel, selectPatternPanel, videoPlayer;
    public Button backBtn;
    void Start() {
        bounds = GameObject.Find("bounds");
        trialRoomPanel = GameObject.Find("TrailRoomPanel");
        Debug.Log("VideoBack: " + trialRoomPanel);
        videoPlayer = GameObject.Find("VideoPlayerGO");
        Debug.Log("VideoBack: " + videoPlayer);
        selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
        Debug.Log("VideoBack: " + selectPatternPanel);
        Button btn = backBtn.GetComponent<Button>();
        btn.onClick.AddListener(BackTask);
    }

    public void BackTask() {
        Debug.Log("VideoBack: BackTask");
        trialRoomPanel.SetActive(false);
        videoPlayer.SetActive(false);
        bounds.SetActive(false);
        selectPatternPanel.SetActive(true);
    }
}
