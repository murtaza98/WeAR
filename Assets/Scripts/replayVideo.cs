using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class replayVideo : MonoBehaviour
{
    public Button replayButton;
    public UnityEngine.Video.VideoPlayer videoPlayer;
    void Start() {
        Button btn = replayButton.GetComponent<Button>();
        videoPlayer = GameObject.Find("VideoPlayerGO").GetComponent<UnityEngine.Video.VideoPlayer>();
		btn.onClick.AddListener(ReplayTask);
    }

    void ReplayTask() {
        videoPlayer.frame = 0;
        MapOn3d.Timer = 0.00f;
        MapOn3d.NowFrame = 0;
    }
}
