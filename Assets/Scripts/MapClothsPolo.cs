using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Globalization;


// 0 :Hip
// 1 :RHip
// 2 :RKnee
// 3 :RFoot
// 4 :LHip
// 5 :LKnee
// 6 :LFoot
// 7 :Spine
// 8 :Thorax
// 9 :Neck/Nose
// 10:Head
// 11:LShoulder
// 12:LElbow
// 13:LWrist
// 14:RShoulder
// 15:RElbow
// 16:RWrist

public class MapClothsPolo : MonoBehaviour
{
    public GameObject videoPlayer;
    static int bone_num_3d = 32;
    static int bone_num_2d = 18;
    // debug points cube
    Transform[] cube_3d_t;

    Transform[] cube_2d_t;

    public static int NowFrame = 0;
    static Vector3[] now_pos_3d = new Vector3[bone_num_3d];
    static Vector3[] now_pos_2d = new Vector3[bone_num_2d];
 
    float Timer=0.0f;
    float scale_ratio = 0.001f; //Scale ratio between pos.txt and Unity model
                                //Since the unit of pos.txt is mm and Unity is m, 
                                //specify a value close to 0.001. Adjust according to model size 
    float heal_position = 0.00f; // 足の沈みの補正値(単位：m)。プラス値で体全体が上へ移動する

    float FrameRate = 15.0f;
    int totalFrames = 156;

    static ClothPolo cloth;

    // static String clothName = "polo_with_bones_scaled";
    static String clothName = "bounds";

    static JObject points_2d = null;
    static JObject points_3d = null;
    private Camera cam;
    static bool updateIsActive = true;

    void Start()
    {   
        // Read 2d data points
        string points_2d_str = Read(Login.jsonFilePath + "/scale_out_a_ojas");
        points_2d = JObject.Parse(points_2d_str);

        // Read 3d data points
        string points_3d_str = Read(Login.jsonFilePath + "/3d_data_a_ojas");
        points_3d = JObject.Parse(points_3d_str);


        cam = Camera.main;


        // initialize cloth
        // Transform left_shoulder = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_left/left_shoulder").transform;
        // Transform left_elbow = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_left/left_shoulder/left_elbow").transform;
        // Transform left_wrist = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_left/left_shoulder/left_elbow/left_wrist").transform;
        // Transform right_shoulder = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_right/right_shoulder").transform;
        // Transform right_elbow = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_right/right_shoulder/right_elbow").transform;
        // Transform right_wrist = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_right/right_shoulder/right_elbow/right_wrist").transform;
        // Transform neck_left = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_left").transform;
        // Transform neck_right = GameObject.Find("Main Camera/"+clothName+"/Armature/torso/neck_right").transform;
        // Transform torso = GameObject.Find("Main Camera/"+clothName+"/Armature/torso").transform;
        
        // TODO -- make this generic so that multiple clothes can be selected
        Transform left_shoulder = GameObject.Find("left_shoulder").transform;
        Transform left_elbow = GameObject.Find("left_elbow").transform;
        Transform left_wrist = GameObject.Find("left_wrist").transform;
        Transform right_shoulder = GameObject.Find("right_shoulder").transform;
        Transform right_elbow = GameObject.Find("right_elbow").transform;
        Transform right_wrist = GameObject.Find("right_wrist").transform;
        Transform neck_left = GameObject.Find("neck_left").transform;
        Transform neck_right = GameObject.Find("neck_right").transform;
        Transform torso = GameObject.Find("torso").transform;

        cloth = new ClothPolo();
        cloth.left_shoulder = left_shoulder;
        cloth.right_shoulder = right_shoulder;
        cloth.right_elbow = right_elbow;
        cloth.left_elbow = left_elbow;
        cloth.left_wrist = left_wrist;
        cloth.right_wrist = right_wrist;
        cloth.neck_left = neck_left;
        cloth.neck_right = neck_right;
        cloth.torso = torso;

        // cloth.left_shoulder.parent = this.transform;
        // cloth.right_shoulder.parent = this.transform;
        // cloth.right_elbow.parent = this.transform;
        // cloth.left_elbow.parent = this.transform;
        // cloth.left_wrist.parent = this.transform;
        // cloth.right_wrist.parent = this.transform;
        // cloth.neck_left.parent = this.transform;
        // cloth.neck_right.parent = this.transform;
        // cloth.torso.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > (1.0f / FrameRate))
        {
            Timer = 0.00f;
            PointUpdate();
            UpdateCube();
        }
    }

    void UpdateCube()
    {
        // UPDATE 2d Points
        if (cube_2d_t == null)
        {
            // Initialize and generate cube
            cube_2d_t = new Transform[bone_num_2d];

            for (int i = 0; i < bone_num_2d; i++)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.transform.parent = this.transform;
                // t.localPosition = now_pos_2d[i] * scale_ratio;
                t.localPosition = now_pos_2d[i];
                t.name = i.ToString();
                t.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                cube_2d_t[i] = t;

                Destroy(t.GetComponent<BoxCollider>());
            }
        }
        // モデルと重ならないように少しずらして表示
        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
        // If already initialized, update the cube position
        for (int i = 0; i < bone_num_2d; i++)
        {
            // convert to unity world coordinate
            cube_2d_t[i].position =  cam.ScreenToWorldPoint(now_pos_2d[i]) + offset;
        }


        // Update 3d Points
        if (cube_3d_t == null)
        {
            // Initialize and generate cube
            cube_3d_t = new Transform[bone_num_3d];

            for (int i = 0; i < bone_num_3d; i++)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.transform.parent = this.transform;
                // t.localPosition = now_pos_2d[i] * scale_ratio;
                t.localPosition = now_pos_3d[i];
                t.name = i.ToString()+"3D";
                t.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                cube_3d_t[i] = t;

                Destroy(t.GetComponent<BoxCollider>());
            }
        }
        // モデルと重ならないように少しずらして表示
        Vector3 offset1 = new Vector3(0,0,0);
        // If already initialized, update the cube position
        for (int i = 0; i < bone_num_3d; i++)
        {
            // cube_2d_t[i].position = now_pos_2d[i] * scale_ratio ;
            Vector3 tmp3d = cam.ScreenToWorldPoint(now_pos_3d[i] * 0.01f + offset1);
            cube_3d_t[i].position = tmp3d * 0.01f + offset1;
        }


        // update cloth
        cloth.UpdatePositions(now_pos_2d, now_pos_3d, cam);
    }

    public static string Read(string filename) {
        //Load the text file using Reources.Load
        TextAsset theTextFile = Resources.Load<TextAsset>(filename);

        //There's a text file named filename, lets get it's contents and return it
        if(theTextFile != null){
            // Debug.Log("-----------------------------------FILE READ--------------------------");
            return theTextFile.text;
        }

        // string text = File.ReadAllText(filename);
            

        //There's no file, return an empty string.
        Debug.Log("ERROR WHILE READING FILE " + filename);
        return string.Empty;
    }

    void PointUpdate()
    {

        // read 2d points for current frame
        JObject cframe = (JObject)points_2d[""+NowFrame];
        for(int i=0;i<bone_num_2d;i++){
            JArray cpoints_str = (JArray) cframe[i+""]["translate"];
            float[] cpoints = cpoints_str.Select(jv => (float)jv).ToArray();
            now_pos_2d[i] = new Vector3(cpoints[0], cpoints[1], 2.0f);

            // // convert to unity world coordinate
            // now_pos_2d[i] = cam.ScreenToWorldPoint(now_pos_2d[i]);
        }

        // read 3d points for current frame
        cframe = (JObject)points_3d[""+NowFrame];
        Vector3 offset1 = new Vector3(500.0f, 500.0f, 500.0f);
        for(int i=0;i<bone_num_3d;i++){
            JArray cpoints_str = (JArray) cframe[i+""]["translate"];
            float[] cpoints = cpoints_str.Select(jv => (float)jv).ToArray();
            now_pos_3d[i] = new Vector3(cpoints[0], cpoints[1], cpoints[2]);
            // now_pos_3d[i] = cam.ScreenToWorldPoint(now_pos_3d[i] * 0.01f + offset1);
        }

        NowFrame = (NowFrame + 1) % totalFrames;

    }

    void SetupCanvas() {
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        // canvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        rectTransform.sizeDelta = new Vector2(730, 311);
        rectTransform.eulerAngles = new Vector3(0.0f, -180.0f, 0.0f);
    }

    void ResetCanvas()
    {
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        // canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;

        rectTransform.sizeDelta = new Vector2(800, 1002);
        rectTransform.eulerAngles = new Vector3(0.0f, -180.0f, 0.0f);
    }

    void OnEnable()
    {
        SetupCanvas();
        Debug.Log("MapClothsPolo onEnable Called");
        videoPlayer = GameObject.Find("VideoPlayerGO");
        Debug.Log("VIDEO PLAYER OBJ: " + videoPlayer);
        var video = videoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>();
        Debug.Log("VIDEO OBJ: " + video);
        video.Play();

        Timer = 0.0f;
        NowFrame = 0;
        while(updateIsActive) {
            Update();
        }
    }

    private void OnDisable()
    {
        ResetCanvas();
        Debug.Log("MapClothsPolo onDisable Called");
        videoPlayer = GameObject.Find("VideoPlayerGO");
        Debug.Log("VIDEO PLAYER OBJ: " + videoPlayer);
        var video = videoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>();
        Debug.Log("VIDEO OBJ: " + video);
        video.Stop();
        updateIsActive = false;
    }

    private GameObject FindInActiveObjectByName(string name)
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



class ClothPolo
{
    public Transform left_shoulder, left_elbow, left_wrist, right_shoulder, right_elbow, right_wrist, neck_left, neck_right, torso;
    Vector3 offset = new Vector3(0, 0, 2.0f);
    float scale_ratio = 0.05f; //Scale ratio between pos.txt and Unity model
                                //Since the unit of pos.txt is mm and Unity is m, 
                                //specify a value close to 0.001. Adjust according to model size 

    public void UpdatePositions(Vector3[] points2d, Vector3[] points3d, Camera cam)
    {
    	// INITIALIZE CLOTH SIZE
        float cloth_scale = 0.00004f;
    	float shoulder_size = 269.0f * cloth_scale;
    	float elbow_size = 330.0f * cloth_scale;
    	float wrist_size = 330.0f * cloth_scale;
    	float torso_size = 762.0f * cloth_scale;


        // Read 3d pose estimation data
        Vector3 left_shoulder_pos_3d = points3d[25];
        Vector3 left_elbow_pos_3d = points3d[26];
        Vector3 left_wrist_pos_3d = points3d[27];
        Vector3 right_shoulder_pos_3d = points3d[17];
        Vector3 right_elbow_pos_3d = points3d[18];
        Vector3 right_wrist_pos_3d = points3d[19];
        Vector3 neck_pos_3d = points3d[13];
        Vector3 torso_pos_3d = points3d[0];
        Vector3 left_waist_3d = points3d[1];
        Vector3 right_waist_3d = points3d[6];

        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_shoulder_pos_3d " + left_shoulder_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame+" left_elbow_pos_3d" + left_elbow_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_wrist_pos_3d " + left_wrist_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_shoulder_pos_3d " + right_shoulder_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_elbow_pos_3d " + right_elbow_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_wrist_pos_3d " + right_wrist_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "neck_pos_3d " + neck_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "torso_pos_3d " + torso_pos_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_waist_3d " + left_waist_3d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_waist_3d " + right_waist_3d);

        // Read 2d pose estimation data
        Vector3 neck_pos_2d = points2d[1];
        Vector3 left_shoulder_pos_2d = points2d[2];
        Vector3 left_elbow_pos_2d = points2d[3];
        Vector3 left_wrist_pos_2d = points2d[4];
        Vector3 right_shoulder_pos_2d = points2d[5];
        Vector3 right_elbow_pos_2d = points2d[6];
        Vector3 right_wrist_pos_2d = points2d[7];
        Vector3 left_waist_2d = points2d[8];
        Vector3 right_waist_2d = points2d[11];
        Vector3 torso_pos_2d = (left_waist_2d+right_waist_2d)/2.00f;

        // convert to unity world coordinate
        neck_pos_2d = cam.ScreenToWorldPoint(neck_pos_2d);
        left_shoulder_pos_2d = cam.ScreenToWorldPoint(left_shoulder_pos_2d);
        left_elbow_pos_2d = cam.ScreenToWorldPoint(left_elbow_pos_2d);
        left_wrist_pos_2d = cam.ScreenToWorldPoint(left_wrist_pos_2d);
        right_shoulder_pos_2d = cam.ScreenToWorldPoint(right_shoulder_pos_2d);
        right_elbow_pos_2d = cam.ScreenToWorldPoint(right_elbow_pos_2d);
        right_wrist_pos_2d = cam.ScreenToWorldPoint(right_wrist_pos_2d);
        left_wrist_pos_2d = cam.ScreenToWorldPoint(left_elbow_pos_2d);
        right_wrist_pos_2d = cam.ScreenToWorldPoint(right_wrist_pos_2d);
        torso_pos_2d = cam.ScreenToWorldPoint(torso_pos_2d);

        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " ====");
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "neck_pose_2d " +neck_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_shoulder_pos_2d " + left_shoulder_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_elbow_pos_2d " + left_elbow_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_wrist_pos_2d " + left_wrist_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_shoulder_pos_2d " + right_shoulder_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_elbow_pos_2d " + right_elbow_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_wrist_pos_2d " + right_wrist_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "left_wrist_pos_2d " + left_wrist_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "right_wrist_pos_2d " + right_wrist_pos_2d);
        Debug.Log("==== Frame No. " + MapClothsPolo.NowFrame + " " + "torso_pos_2d " + torso_pos_2d);

        // ORIGINAL
        Vector3 new_neck_pos_3d = neck_pos_2d;
        Vector3 new_left_shoulder_pos = calc_newPosition(neck_pos_3d, left_shoulder_pos_3d, new_neck_pos_3d, shoulder_size);
        Vector3 new_right_shoulder_pos = calc_newPosition(neck_pos_3d, right_shoulder_pos_3d, new_neck_pos_3d, shoulder_size);
        Vector3 new_left_elbow_pos = calc_newPosition(left_shoulder_pos_3d, left_elbow_pos_3d, new_left_shoulder_pos, elbow_size);
        Vector3 new_right_elbow_pos = calc_newPosition(right_shoulder_pos_3d, right_elbow_pos_3d, new_right_shoulder_pos, elbow_size);
        Vector3 new_left_wrist_pos = calc_newPosition(left_elbow_pos_3d, left_wrist_pos_3d, new_left_elbow_pos, wrist_size);
        Vector3 new_right_wrist_pos = calc_newPosition(right_elbow_pos_3d, right_wrist_pos_3d, new_right_elbow_pos, wrist_size);
        Vector3 new_torso_pos = calc_newPosition(neck_pos_2d, torso_pos_2d, new_neck_pos_3d, torso_size);


        // ONLY 2d POINTS
        // Vector3 new_neck_pos_3d = neck_pos_2d;
        // Vector3 new_left_shoulder_pos = calc_newPosition(neck_pos_2d, left_shoulder_pos_2d, neck_pos_2d, calc_dist(neck_pos_2d, left_shoulder_pos_2d));
        // Vector3 new_right_shoulder_pos = calc_newPosition(neck_pos_2d, right_shoulder_pos_2d, neck_pos_2d, calc_dist(neck_pos_2d, right_shoulder_pos_2d));
        // Vector3 new_left_elbow_pos = calc_newPosition(left_shoulder_pos_2d, left_elbow_pos_2d, left_shoulder_pos_2d, calc_dist(left_shoulder_pos_2d, left_elbow_pos_2d));
        // Vector3 new_right_elbow_pos = calc_newPosition(right_shoulder_pos_2d, right_elbow_pos_2d, right_shoulder_pos_2d, calc_dist(right_shoulder_pos_2d, right_elbow_pos_2d));
        // Vector3 new_left_wrist_pos = calc_newPosition(left_elbow_pos_2d, left_wrist_pos_2d, left_elbow_pos_2d, calc_dist(left_elbow_pos_2d, left_wrist_pos_2d));
        // Vector3 new_right_wrist_pos = calc_newPosition(right_elbow_pos_2d, right_wrist_pos_2d, right_elbow_pos_2d, calc_dist(right_elbow_pos_2d, right_wrist_pos_2d));
        // Vector3 new_torso_pos = calc_newPosition(neck_pos_2d, torso_pos_2d, neck_pos_2d, calc_dist(neck_pos_2d, torso_pos_2d));


        // 2D DISTANCE WITH 3D ROTATION
        // Vector3 new_neck_pos_3d = neck_pos_2d;
        // Vector3 new_left_shoulder_pos = calc_newPosition(neck_pos_3d, left_shoulder_pos_3d, new_neck_pos_3d, calc_dist(neck_pos_2d, left_shoulder_pos_2d));
        // Vector3 new_right_shoulder_pos = calc_newPosition(neck_pos_3d, right_shoulder_pos_3d, new_neck_pos_3d, calc_dist(neck_pos_2d, right_shoulder_pos_2d));
        // Vector3 new_left_elbow_pos = calc_newPosition(left_shoulder_pos_3d, left_elbow_pos_3d, new_left_shoulder_pos, calc_dist(left_shoulder_pos_2d, left_elbow_pos_2d));
        // Vector3 new_right_elbow_pos = calc_newPosition(right_shoulder_pos_3d, right_elbow_pos_3d, new_right_shoulder_pos, calc_dist(right_shoulder_pos_2d, right_elbow_pos_2d));
        // Vector3 new_left_wrist_pos = calc_newPosition(left_elbow_pos_3d, left_wrist_pos_3d, new_left_elbow_pos, calc_dist(left_elbow_pos_2d, left_wrist_pos_2d));
        // Vector3 new_right_wrist_pos = calc_newPosition(right_elbow_pos_3d, right_wrist_pos_3d, new_right_elbow_pos, calc_dist(right_elbow_pos_2d, right_wrist_pos_2d));
        // Vector3 new_torso_pos = calc_newPosition(neck_pos_2d, torso_pos_2d, new_neck_pos_3d, calc_dist(neck_pos_2d, torso_pos_2d));


        
        // new method
        // https://gamedev.stackexchange.com/questions/89776/how-can-i-draw-a-line-of-certain-length-and-direction?rq=1
        
        
        // Debug.Log(left_elbow_pos_2d);
        this.torso.position = new_torso_pos;
        // this.neck_left.position = new_neck_pos_3d;
        // this.neck_right.position = new_neck_pos_3d;
        // this.left_shoulder.position = new_right_shoulder_pos;
        // this.right_shoulder.position = new_left_shoulder_pos;
        // this.left_elbow.position = new_right_elbow_pos;
        // this.right_elbow.position = new_left_elbow_pos;
        // this.left_wrist.position = new_right_wrist_pos;
        // this.right_wrist.position = new_left_wrist_pos;
        

        // Debug.Log("after --> "+cam.ScreenToWorldPoint(new_neck_pos_3d));
        // ORIGINAL
    	// calc rotation
        // this.neck_right.localRotation = Quaternion.FromToRotation(Vector3.down, neck_pos_3d - left_shoulder_pos_3d);
        // this.neck_left.localRotation = Quaternion.FromToRotation(Vector3.down, neck_pos_3d - right_shoulder_pos_3d);
        // this.neck_left.Rotate(-80.0f, 0.0f, 0.0f, Space.Self);
        // this.neck_right.Rotate(-80.0f, 0.0f, 0.0f, Space.Self);
        // this.right_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, right_shoulder_pos_3d - right_elbow_pos_3d);
        // this.left_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, left_shoulder_pos_3d - left_elbow_pos_3d);
        // this.right_elbow.rotation = Quaternion.FromToRotation(Vector3.down, right_elbow_pos_3d - right_wrist_pos_3d);
        // this.left_elbow.rotation = Quaternion.FromToRotation(Vector3.down, left_elbow_pos_3d - left_wrist_pos_3d);
        // Not including torso, bcs results are not proper
        // this.torso.rotation = Quaternion.FromToRotation(Vector3.up, torso_pos - neck_pos);

        // 2D POINTS
        // this.neck_left.rotation = Quaternion.FromToRotation(Vector3.down, neck_pos_2d - right_shoulder_pos_2d);
        // this.neck_right.rotation = Quaternion.FromToRotation(Vector3.down, neck_pos_2d - left_shoulder_pos_2d);
        // this.right_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, right_shoulder_pos_2d - right_elbow_pos_2d);
        // this.left_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, left_shoulder_pos_2d - left_elbow_pos_2d);
        // this.right_elbow.rotation = Quaternion.FromToRotation(Vector3.down, right_elbow_pos_2d - right_wrist_pos_2d);
        // this.left_elbow.rotation = Quaternion.FromToRotation(Vector3.down, left_elbow_pos_2d - left_wrist_pos_2d);

        // right_waist_3d = new Vector3(right_waist_3d.x, 0, 0);
        // left_waist_3d = new Vector3(left_waist_3d.x, 0, 0);
        // torso rotation
        // Debug.Log(Quaternion.FromToRotation(Vector3.down, right_waist_3d - left_waist_3d));
        var torso_rot_vec = right_shoulder_pos_3d - left_shoulder_pos_3d;
        var torso_rot_vec_i = left_shoulder_pos_3d - right_shoulder_pos_3d;
        var torso_rot = Quaternion.FromToRotation(Vector3.right, torso_rot_vec);
        // this.torso.localRotation = torso_rot * Quaternion.Euler(-90, -90, 0);
        this.torso.localRotation = torso_rot;
        this.torso.Rotate(85.0f, 0.0f, 0.0f, Space.Self);

        // this.neck_right.localRotation = Quaternion.FromToRotation(torso_rot_vec_i, neck_pos_3d - left_shoulder_pos_3d);
        // this.neck_right.Rotate(85.0f, 0.0f, 0.0f, Space.Self);
        // this.neck_left.localRotation = Quaternion.FromToRotation(torso_rot_vec, neck_pos_3d - right_shoulder_pos_3d);
        // this.neck_left.Rotate(85.0f, 0.0f, 0.0f, Space.Self);
        this.left_shoulder.localEulerAngles = new Vector3(this.left_shoulder.localEulerAngles.x, 30.0f, this.left_shoulder.localEulerAngles.z);

        
        this.left_shoulder.localRotation = Quaternion.FromToRotation(this.torso.rotation.eulerAngles, left_shoulder_pos_3d - left_elbow_pos_3d);
        this.right_shoulder.localRotation = Quaternion.FromToRotation(this.torso.rotation.eulerAngles, right_shoulder_pos_3d - right_elbow_pos_3d);

        // this.left_shoulder.Rotate(-85.0f, 0.0f, 0.0f, Space.Self);
        // this.left_shoulder.Rotate(85.0f, 0.0f, 0.0f, Space.Self);
        // this.neck_right.Rotate(180.0f, 0.0f, 0.0f, Space.Self);
        // this.torso.rotation = Quaternion.FromToRotation(Vector3.up, right_waist_3d - left_waist_3d);
    }

    public float calc_dist(Vector3 v1, Vector3 v2){
        Vector3 difference = new Vector3(
            v1.x - v2.x,
            v1.y - v2.y,
            v1.z - v2.z);
        float distance = (float)Math.Sqrt(
            Math.Pow(difference.x, 2f) +
            Math.Pow(difference.y, 2f) +
            Math.Pow(difference.z, 2f));

        return distance;
    }

    public Vector3 cloneObj(Vector3 v){
        return new Vector3(v.x, v.y, v.z);
    }

    public Vector3 convert3d(Vector2 v){
        float cscale = 0.01f;
        return new Vector3(v.x, -v.y, 0.0f) * cscale;
    }

    public Vector3 custom_scale(Vector3 v){
    	return v * scale_ratio + offset;
    }

    // calc position of point B, such that it is at 'distance' apart from start and same direction as A-B 
    public Vector3 calc_newPosition(Vector3 A, Vector3 B, Vector3 start, float distance){
    	//Get the direction of the line
		Vector3 direction = B - A;		
		// Normalize vector and
		// Get a new point at your distance from point start
		Vector3 newB = start + (Vector3.Normalize(direction) * distance);

		return newB;
    }

}