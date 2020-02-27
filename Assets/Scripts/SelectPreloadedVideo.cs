using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPreloadedVideo : MonoBehaviour
{
    public GameObject selectPatternPanel, uploadVideoPanel;
    public Button selectBtn;

    void Start() {
        uploadVideoPanel = GameObject.Find("UploadVideoPanel");
        selectPatternPanel = Credentials.FindInActiveObjectByName("SelectPatternPanel");
        Button btn = selectBtn.GetComponent<Button>();
        btn.onClick.AddListener(SelectTask);
    }

    public void SelectTask() {
        uploadVideoPanel.SetActive(false);
        selectPatternPanel.SetActive(true);
    }
}
