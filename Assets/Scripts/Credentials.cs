﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credentials : MonoBehaviour
{
    public static string database_server_ip = "http://1942d18d.ngrok.io/";
	public static string pose_estimation_server_ip = "192.168.0.104";
    public static string video_folder_path = "";
    public static string json_files_path = "";
	public  static string json_2D_name = "";
	public static string json_3D_name = "";

	// Creating notification channels

    public static GameObject FindInActiveObjectByName(string name)
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
