using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPreloadedVideo : MonoBehaviour
{
    public GameObject selectPatternPanel, uploadVideoPanel, preloadVideoPanel;
    public Button selectBtn;
    public PreloadVideoList p;

    void Start() {
        uploadVideoPanel = GameObject.Find("UploadVideoPanel");
        preloadVideoPanel = Credentials.FindInActiveObjectByName("PreloadVideoPanel");
        // selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel"); // DELETE
        p = FindObjectOfType<PreloadVideoList>();
        Button btn = selectBtn.GetComponent<Button>();
        btn.onClick.AddListener(SelectTask);
    }

    public void SelectTask() {
        uploadVideoPanel.SetActive(false);
        preloadVideoPanel.SetActive(true);
        p.GenerateButtons();
        // selectPatternPanel.SetActive(true);    // DELETE
    }
}
