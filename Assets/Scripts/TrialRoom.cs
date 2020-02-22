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
	public GameObject trialRoomPanel, selectPatternPanel;

    void Start()
    {
        Button tryBtn = tryClothBtn.GetComponent<Button>();
		tryBtn.onClick.AddListener(TryClothTask);    
    }

    void TryClothTask() {
		trialRoomPanel = FindInActiveObjectByName("TrailRoomPanel");
		selectPatternPanel = GameObject.Find("/Canvas/SelectPatternPanel");
		selectPatternPanel.SetActive(false);
		trialRoomPanel.SetActive(true);
	}

    // Same in UploadVideo.cs
	/*** TODO: Make this method common ***/
    GameObject FindInActiveObjectByName(string name)
	{
	    Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
	    for (int i = 0; i < objs.Length; i++)
	    {
	        if (objs[i].hideFlags == HideFlags.None)
	        {
	            if (objs[i].name == name)
	            {
	                return objs[i].gameObject;
	            }
	        }
	    }
	    return null;
	}
}
