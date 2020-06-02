//using UnityEngine;
//using System;
//using System.Collections.Generic;
//using TrackIRUnity;

//[Serializable]
//public class LimitObj {
//    public float lowerobj, upperobj;
//}

//public class TrackIRObj : MonoBehaviour {
//    public bool useGUI;
//    TrackIRUnity.TrackIRClient trackIRclient;
    
//    bool running;
//    string status, data;
//    public Rect ObjstatusRect;
//    public Rect ObjdataRect;
//    public float ObjpositionReductionFactor;
//    public float ObjrotationReductionFactor = 0.01f;
//	public LimitObj ObjpositionXLimits, ObjpositionYLimits, ObjpositionZLimits, ObjyawLimits, ObjpitchLimits, ObjrollLimits;
//	public bool ObjuseLimits;
//	public Transform Target;

//    public bool turnAroundY = true;


//	// Use this for initialization
//	void Start () {
//        Target = this.transform;
//        trackIRclient = new TrackIRUnity.TrackIRClient();  // Create an instance of the TrackerIR Client to get data from
//        status = trackIRclient.GetFullDllPath("C: \\Users\\u2012\\Documents\\Aimxy\\AimxyHeadaim\\AimxyPlugins\\");  //edited, skr
//        data = "";
//        StartCamera();
//		//Target = GameObject.FindGameObjectWithTag("Player").transform;	
//	}

//    void StartCamera() {
//        if (trackIRclient != null && !running) {                        // Start tracking
//            Debug.Log(trackIRclient.getInternalStatus());           //edited, skr
//            status = trackIRclient.TrackIR_Enhanced_Init();
//            running = true;
//        }
//    }

//    void StopCamera() {
//        if (trackIRclient != null && running) {                         // Stop tracking
//            status = trackIRclient.TrackIR_Shutdown();
//            running = false;
//        }
//    }

//    void OnEnable() {
//        StartCamera();
//    }

//    void OnDisable() {
//        StopCamera();
//    }

//    void OnApplicationQuit() {                              // Shutdown the camera when we quit the application.
//        StopCamera();
//    }

//    void OnGUI() {
//        if (useGUI) {                                       // Gui for testing
//            if (GUI.Button(new Rect(10, 10, 100, 25), "Init")) {
//                Debug.Log("Thanks!");  //edited, skr
//                StartCamera();
                
//            }
//            if (GUI.Button(new Rect(10, 45, 100, 25), "Shutdown")) {
//                StopCamera();
//            }
//			GUI.TextArea(ObjstatusRect, status);
//			GUI.TextArea(ObjdataRect, data);
//        }
//    }

//	// Update is called once per frame
//	void Update () {
//        if (running) {
//            data = trackIRclient.client_TestTrackIRData();          // Data for debugging output, can be removed if not debugging/testing
//            TrackIRClient.LPTRACKIRDATA tid = trackIRclient.client_HandleTrackIRData(); // Data for head tracking
//			Vector3 pos = this.transform.localPosition;                          // Updates main camera, change to whatever
//			Vector3 rot = this.transform.rotation.eulerAngles;
//			if (!ObjuseLimits) {
//				pos.x = tid.fNPX * ObjpositionReductionFactor;                                        
//				pos.y = tid.fNPY * ObjpositionReductionFactor;
//				pos.z = tid.fNPZ * ObjpositionReductionFactor;

//				rot.y = tid.fNPYaw * ObjrotationReductionFactor;
//				rot.x = tid.fNPPitch * ObjrotationReductionFactor*-1f;
//				rot.z = tid.fNPRoll * ObjrotationReductionFactor;
                
//            } else {
//				pos.x = Mathf.Clamp(-tid.fNPX *- ObjpositionReductionFactor, ObjpositionXLimits.lowerobj, ObjpositionXLimits.upperobj);
//				pos.y = Mathf.Clamp(tid.fNPY * ObjpositionReductionFactor, ObjpositionYLimits.lowerobj, ObjpositionYLimits.upperobj);
//				pos.z = Mathf.Clamp(-tid.fNPZ * ObjpositionReductionFactor, ObjpositionZLimits.lowerobj, ObjpositionZLimits.upperobj);
                
//				rot.y = Mathf.Clamp(-tid.fNPYaw * ObjrotationReductionFactor, ObjyawLimits.lowerobj, ObjyawLimits.upperobj);
//				rot.x = Mathf.Clamp(tid.fNPPitch * ObjrotationReductionFactor, ObjpitchLimits.lowerobj, ObjpitchLimits.upperobj);
//				rot.z = Mathf.Clamp(tid.fNPRoll * ObjrotationReductionFactor, ObjrollLimits.lowerobj, ObjrollLimits.upperobj);
//            }

//            //Debug.Log(rot.x + " " + rot.y + " " + rot.z);
//            //this.transform.rotation.eulerAngles.Set(rot.x, rot.y, rot.z);
//            this.transform.rotation = Quaternion.Euler(rot.x, rot.y + (turnAroundY ? 180f : 0f), -1f*rot.z);

//            //Target.localPosition = pos;
//        }
//    }
//}