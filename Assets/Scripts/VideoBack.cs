using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoBack : MonoBehaviour
{
    public GameObject trialRoomPanel, selectPatternPanel, videoPlayer;
    public Button backBtn;
    void Start() {
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
        selectPatternPanel.SetActive(true);
    }
}
