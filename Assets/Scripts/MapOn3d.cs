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

public class MapOn3d : MonoBehaviour
{
    static int bone_num = 17;
    static int bone_num_2d = 18;
    static int bone_num_3d = 32;
    Transform[] cube_t; // Transform of Cube
    Transform[] cube_3d_t;
    Transform[] cube_2d_t;

    public static int NowFrame = 0;
    static Vector3[] now_pos = new Vector3[bone_num];
    static Vector3[] now_pos_2d = new Vector3[bone_num_2d];
    static Vector3[] now_pos_3d = new Vector3[bone_num_3d];
 
    public static float Timer;
    float scale_ratio = 0.001f; //Scale ratio between pos.txt and Unity model
                                //Since the unit of pos.txt is mm and Unity is m, 
                                //specify a value close to 0.001. Adjust according to model size 
    float heal_position = 0.00f; // 足の沈みの補正値(単位：m)。プラス値で体全体が上へ移動する

    float FrameRate = 15.0f;
    int totalFrames = 351;

    static Cloth5 cloth;

    // static String clothName = "polo_with_bones_scaled";
    static String clothName = "bounds";

    static JObject points_2d = null;
    static JObject points_3d = null;

    Transform torso_skel, neck_left_skel, neck_right_skel, left_shoulder_skel, right_shoulder_skel, left_elbow_skel, right_elbow_skel, left_wrist_skel, right_wrist_skel;

    private Camera cam;
    public GameObject videoPlayer;
    static bool updateIsActive = true;

    void Start()
    {
        Transform left_shoulder = GameObject.Find(clothName+"/Armature/torso/neck_left/left_shoulder").transform;
        Transform left_elbow = GameObject.Find(clothName+"/Armature/torso/neck_left/left_shoulder/left_elbow").transform;
        Transform left_wrist = GameObject.Find(clothName+"/Armature/torso/neck_left/left_shoulder/left_elbow/left_wrist").transform;
        Transform right_shoulder = GameObject.Find(clothName+"/Armature/torso/neck_right/right_shoulder").transform;
        Transform right_elbow = GameObject.Find(clothName+"/Armature/torso/neck_right/right_shoulder/right_elbow").transform;
        Transform right_wrist = GameObject.Find(clothName+"/Armature/torso/neck_right/right_shoulder/right_elbow/right_wrist").transform;
        Transform neck_left = GameObject.Find(clothName+"/Armature/torso/neck_left").transform;
        Transform neck_right = GameObject.Find(clothName+"/Armature/torso/neck_right").transform;
        Transform torso = GameObject.Find(clothName+"/Armature/torso").transform;


        torso_skel = GameObject.Find("torso_skel").transform;
        neck_left_skel = GameObject.Find("neck_left_skel").transform;
        neck_right_skel = GameObject.Find("neck_right_skel").transform;
        left_shoulder_skel = GameObject.Find("left_shoulder_skel").transform;
        right_shoulder_skel = GameObject.Find("right_shoulder_skel").transform;
        left_elbow_skel = GameObject.Find("left_elbow_skel").transform;
        right_elbow_skel = GameObject.Find("right_elbow_skel").transform;
        left_wrist_skel = GameObject.Find("left_wrist_skel").transform;
        right_wrist_skel = GameObject.Find("right_wrist_skel").transform;


        cloth = new Cloth5();
        cloth.left_shoulder = left_shoulder;
        cloth.right_shoulder = right_shoulder;
        cloth.right_elbow = right_elbow;
        cloth.left_elbow = left_elbow;
        cloth.left_wrist = left_wrist;
        cloth.right_wrist = right_wrist;
        cloth.neck_left = neck_left;
        cloth.neck_right = neck_right;
        cloth.torso = torso;

         // Read 3d data points
        string points_3d_str = Read("3d_data");
        points_3d = JObject.Parse(points_3d_str);

        // Read 2d data points
        string points_2d_str = Read(PreloadVideoList.VideoName.Substring(0, PreloadVideoList.VideoName.Length-4) + "_2D");
        points_2d = JObject.Parse(points_2d_str);

        cam = Camera.main;
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
                t.name = i.ToString()+"2D";
                t.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                cube_2d_t[i] = t;

                Destroy(t.GetComponent<BoxCollider>());
            }
        }
        // モデルと重ならないように少しずらして表示
        // Vector3 offset = new Vector3(500.0f, 500.0f, 500.0f);
        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
        // If already initialized, update the cube position
        for (int i = 0; i < bone_num_2d; i++)
        {
            // convert to unity world coordinate
            cube_2d_t[i].position =  cam.ScreenToWorldPoint(now_pos_2d[i]) + offset;
            // Debug.Log(now_pos_2d[i]);
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
                t.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                t.GetComponent<Renderer>().material.color = new Color(255,0,0);
                cube_3d_t[i] = t;

                Destroy(t.GetComponent<BoxCollider>());
            }
        }
        // モデルと重ならないように少しずらして表示
        Vector3 offset1 = new Vector3(500.0f, 500.0f, 500.0f);
        // If already initialized, update the cube position
        // for (int i = 0; i < bone_num_3d; i++)
        // {
        //     // cube_2d_t[i].position = now_pos_2d[i] * scale_ratio ;
        //     // Vector3 tmp3d = cam.ScreenToWorldPoint(now_pos_3d[i] * 0.01f + offset1);
        //     // cube_3d_t[i].position = tmp3d * 0.01f + offset1;
        //     cube_3d_t[i].position = now_pos_3d[i] * scale_ratio + new Vector3(0, heal_position, 0) + offset;
        // }


        Vector3 left_shoulder_pos_3d = now_pos_3d[25];
        Vector3 left_elbow_pos_3d = now_pos_3d[26];
        Vector3 left_wrist_pos_3d = now_pos_3d[27];
        Vector3 right_shoulder_pos_3d = now_pos_3d[17];
        Vector3 right_elbow_pos_3d = now_pos_3d[18];
        Vector3 right_wrist_pos_3d = now_pos_3d[19];
        Vector3 neck_pos_3d = now_pos_3d[13];
        Vector3 torso_pos_3d = now_pos_3d[0];
        Vector3 left_waist_3d = now_pos_3d[1];
        Vector3 right_waist_3d = now_pos_3d[6];

        // reset skel
        this.torso_skel.Rotate(-90.0f, 0, 0, Space.Self);
        // translate skel to 3d points
        this.torso_skel.position = torso_pos_3d * scale_ratio;
        this.neck_left_skel.position = neck_pos_3d * scale_ratio;
        this.neck_right_skel.position = neck_pos_3d * scale_ratio;
        this.left_shoulder_skel.position = left_shoulder_pos_3d * scale_ratio;
        this.right_shoulder_skel.position = right_shoulder_pos_3d * scale_ratio;
        this.left_elbow_skel.position = left_elbow_pos_3d * scale_ratio;
        this.right_elbow_skel.position = right_elbow_pos_3d * scale_ratio;
        this.left_wrist_skel.position = left_wrist_pos_3d * scale_ratio;
        this.right_wrist_skel.position = right_wrist_pos_3d * scale_ratio;
        // rotate skel
        this.torso_skel.Rotate(90.0f, 0, 0, Space.Self);


        // update now_pos_3d with the rotated positions
        now_pos_3d[25] = this.left_shoulder_skel.position;
        now_pos_3d[26] = this.left_elbow_skel.position;
        now_pos_3d[27] = this.left_wrist_skel.position;
        now_pos_3d[17] = this.right_shoulder_skel.position;
        now_pos_3d[18] = this.right_elbow_skel.position;
        now_pos_3d[19] = this.right_wrist_skel.position;
        now_pos_3d[13] = this.neck_left_skel.position;
        now_pos_3d[0] = this.torso_skel.position;
        // Vector3 left_waist_3d = now_pos_3d[1];
        // Vector3 right_waist_3d = now_pos_3d[6];


        // MOVE CLOTH to this rotated skel
        cloth.UpdatePositions(now_pos_3d, now_pos_2d, cam);
    }

    public static string Read(string filename) {
        //Load the text file using Reources.Load
        // TextAsset theTextFile = Resources.Load<TextAsset>(filename);
        Debug.Log("2D JSON FILE NAME: " + filename);
        //There's a text file named filename, lets get it's contents and return it
        // if(theTextFile != null){
            // Debug.Log("-----------------------------------FILE READ--------------------------");
            // return theTextFile.text;
        // }

        StreamReader reader = new StreamReader(Path.Combine(Login.jsonFilePath, filename));
        string jsonFileContents = reader.ReadToEnd();
        reader.Close();

        return jsonFileContents;
        //There's no file, return an empty string.
        // Debug.Log("ERROR WHILE READING FILE " + filename);
        // return string.Empty;
    }

    void PointUpdate()
    {
        // read 2d points for current frame
        JObject cframe = (JObject)points_2d[""+NowFrame];
        for(int i=0;i<bone_num_2d;i++){
            JArray cpoints_str = (JArray) cframe[i+""]["translate"];
            float[] cpoints = cpoints_str.Select(jv => (float)jv).ToArray();
            now_pos_2d[i] = new Vector3(cpoints[0], cpoints[1], 2.00f);
            // // convert to unity world coordinate
            // now_pos_2d[i] = cam.ScreenToWorldPoint(now_pos_2d[i]);
        }

        // read 3d points for current frame
        cframe = (JObject)points_3d[""+NowFrame];
        Vector3 offset1 = new Vector3(500.0f, 500.0f, 500.0f);
        for(int i=0;i<bone_num_3d;i++){
            JArray cpoints_str = (JArray) cframe[i+""]["translate"];
            float[] cpoints = cpoints_str.Select(jv => (float)jv).ToArray();
            now_pos_3d[i] = new Vector3(cpoints[0], -cpoints[1], -cpoints[2]);
            // now_pos_3d[i] = cam.ScreenToWorldPoint(now_pos_3d[i] * 0.01f + offset1);
        }

        NowFrame = (NowFrame + 1) % totalFrames;
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

    void SetupCanvas() {
        GameObject canvas = GameObject.Find("Canvas");
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        // canvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        rectTransform.sizeDelta = new Vector2(736, 311);
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
}


class Cloth5
{
    public Transform left_shoulder, left_elbow, left_wrist, right_shoulder, right_elbow, right_wrist, neck_left, neck_right, torso;
    Vector3 offset = new Vector3(0, 0, 2.0f);
    float scale_ratio = 0.001f; //Scale ratio between pos.txt and Unity model
                                //Since the unit of pos.txt is mm and Unity is m, 
                                //specify a value close to 0.001. Adjust according to model size 
    float heal_position = 0.00f; // 足の沈みの補正値(単位：m)。プラス値で体全体が上へ移動する
    Vector3 prev_torso_rot = new Vector3(0.0f, 0.0f, 0.0f);

    public void UpdatePositions(Vector3[] points3d, Vector3[] points2d, Camera cam)
    {
    	// INITIALIZE CLOTH SIZE
        float cloth_scale = 0.6f;
    	float shoulder_size = 269.0f * cloth_scale;
    	float elbow_size = 330.0f * cloth_scale;
    	float wrist_size = 330.0f * cloth_scale;
    	float torso_size = 762.0f * cloth_scale;

        // READ 3d pose estimation data
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

        // map cloth to 3d points in virtual 3d space
        this.torso.position = torso_pos_3d;
        this.neck_left.position = neck_pos_3d;
        this.neck_right.position = neck_pos_3d;
        this.left_shoulder.position = left_shoulder_pos_3d;
        this.right_shoulder.position = right_shoulder_pos_3d;
        this.left_elbow.position = left_elbow_pos_3d;
        this.right_elbow.position = right_elbow_pos_3d;
        this.left_wrist.position = left_wrist_pos_3d;
        this.right_wrist.position = right_wrist_pos_3d;
        //  calc rotation
        this.right_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, right_shoulder_pos_3d - right_elbow_pos_3d);
        this.left_shoulder.rotation = Quaternion.FromToRotation(Vector3.down, left_shoulder_pos_3d - left_elbow_pos_3d);
        this.right_elbow.rotation = Quaternion.FromToRotation(Vector3.down, right_elbow_pos_3d - right_wrist_pos_3d);
        this.left_elbow.rotation = Quaternion.FromToRotation(Vector3.down, left_elbow_pos_3d - left_wrist_pos_3d);
        this.neck_left.rotation = Quaternion.FromToRotation(Vector3.down, neck_pos_3d - left_shoulder_pos_3d);
        this.neck_right.rotation = Quaternion.FromToRotation(Vector3.down, neck_pos_3d - right_shoulder_pos_3d);
        this.torso.rotation = Quaternion.FromToRotation(Vector3.left, left_shoulder_pos_3d - right_shoulder_pos_3d);



        // now translate this points to 2d world
        this.torso.position = cam.ScreenToWorldPoint(torso_pos_2d);
    }   

    public Vector3 cloneObj(Vector3 v){
        return new Vector3(v.x, v.y, v.z);
    }

    public Vector3 convert3d(Vector2 v){
        float cscale = 0.01f;
        return new Vector3(v.x, -v.y, 0.0f) * cscale;
    }

    public Vector3 custom_scale(Vector3 v){
    	return v * scale_ratio + new Vector3(0, heal_position, 0) + offset;
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
