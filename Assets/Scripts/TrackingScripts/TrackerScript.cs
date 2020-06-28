using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TrackerScript : MonoBehaviour {
    //local host
    public string IP = "127.0.0.1";

    //Ports
    public int portLocal = 6000;

    public bool smoothMovements = true;
    public bool reduceJitters = true;
    public float jitterMin = 0.5f;

    public float xMotionDamping = 1f;
    public float yMotionDamping = 1f;
    public float zMotionDamping = 0.2f;

    public float zOffset = 2f;

    // range of the character/object in camera coordinates
    public Vector2 xRange = new Vector2(-50, 50);
    public Vector2 yRange = new Vector2(0, 75);
    public Vector2 zRange = new Vector2(0, 30);
    public bool useRangeNotDamping = true;

    public bool keyboardInputDepth = true;
    public bool keyboardInputYRot = true;
    public bool useShiftInput = false;

    public bool mapToPhoneRotation = false;
    public bool turnAroundY = false;

    public Vector3 targetPosition;

    public bool usingYolo = false;

    public bool flipX = true;
    public bool flipY = true;

    public Vector2 translatePos = new Vector2();

    // in game camera view range (should be universal)
    Vector2 xGameRange = new Vector2(-5, 5);
    Vector2 yGameRange = new Vector2(0, 5);

    // public int portRemote = 9001;

    // Create necessary UdpClient objects
    UdpClient client;
    //IPEndPoint remoteEndPoint;

    // Receiving Thread
    Thread receiveThread1;
    // Message to be sent
    //string strMessageSend = "";

    // info strings, no need to change to float/double
    public string stringX = "";
    public string stringY = "";
    public string stringZ = "";
    public string stringYaw = "";
    public string stringP = "";
    public string stringR = "";


    public double valueX = 0;
    public double valueY = 0;
    public double valueZ = 0;
    public double valueYaw = 0;
    public double valueP = 0;
    public double valueR = 0;

    bool shouldChangePos = false;

    // start from Unity3d
    void Start() {
        valueX = transform.position.x;
        valueY = transform.position.y;
        valueZ = transform.position.z;

        targetPosition = transform.position;

        if (!GameObject.FindGameObjectWithTag("GameController").GetComponent<ModeController>().IsUsingYOLO()) {
            this.enabled = false;
        }
        else
            init();

        Application.targetFrameRate = 30;
    }

    //originally no update function at all, all stuff in onGUI()
    void FixedUpdate() {
        var pos = this.transform.position;
        Quaternion rot = this.transform.rotation;


        if (keyboardInputDepth) {
            if (useShiftInput == Input.GetKey(KeyCode.LeftShift)) {
                if (Input.GetKey(KeyCode.End))
                    zOffset -= 0.2f;
                if (Input.GetKey(KeyCode.Home))
                    zOffset += 0.2f;
            }
        }

        pos.z = zOffset + 0f * (float)valueZ * zMotionDamping;  // Z IS CONSTANT NOW (with keyboard input)
        if (useRangeNotDamping) {
            float xRatio = (float)valueX / (float)xRange.y;
            float yRatio = (float)valueY / (float)yRange.y;
            float zRatio = (float)valueZ / (float)zRange.y;

            pos.x = xRatio * xGameRange.y;
            pos.y = yRatio * yGameRange.y;
        }
        else {
            pos.x = (float)valueX * xMotionDamping;
            pos.y = (float)valueY * yMotionDamping;
        }

        /*
            rot.x = (float) valueYaw;
            rot.y = (float) valueP;
            rot.z = (float) valueR;
        */

        if (mapToPhoneRotation) {
            valueR = -valueR;
            var tempyaw = valueYaw;
            valueYaw = -valueP;
            valueP = -tempyaw;
        }

        if (turnAroundY) {
            valueP += 180;
        }

        if (keyboardInputYRot) {
            if (useShiftInput == Input.GetKey(KeyCode.LeftShift)) {
                if (Input.GetKey(KeyCode.LeftArrow))
                    valueP += 6;
                if (Input.GetKey(KeyCode.RightArrow))
                    valueP -= 6;

                if (Input.GetKey(KeyCode.UpArrow))
                    valueYaw += 6;
                if (Input.GetKey(KeyCode.DownArrow))
                    valueYaw -= 6;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.T)) {
            valueYaw = 0;
            valueR = 0;
        }

        if (flipX)
            pos.x *= -1f;
        if (flipY)
            pos.y *= -1f;

        pos.x += translatePos.x;
        pos.y += translatePos.y;

        if (shouldChangePos) {
            if (reduceJitters)
                if (Vector3.Magnitude(pos - transform.position) < jitterMin)
                    pos = transform.position;
            targetPosition = pos;
            shouldChangePos = false;
        }
        if (!usingYolo)
            this.transform.rotation = Quaternion.Euler((float)valueYaw, (float)valueP, (float)valueR);

        if (Vector3.Magnitude(targetPosition - transform.position) > 0.05f) {
            if (!smoothMovements)
                this.transform.position = targetPosition;
            else {
                this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, Vector3.Magnitude(targetPosition - transform.position));
            }
        }
    }

    // Initialization code
    private void init() {
        //Initialize(seen in comments window)
        print("UDP Object init()");

        //  Create remote endpoint(to Matlab)
        //remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), portRemote);

        // Create local client
        client = new UdpClient(portLocal);

        //  local endpoint define(where messages are received)
        // Create a new thread for reception of incoming messages

        receiveThread1 = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread1.IsBackground = true;
        receiveThread1.Start();

    }



    // Receive data, update packets received
    private void ReceiveData() {
        while (true) {

            try {
                //var pos = this.transform.position;
                //var rot = this.transform.rotation;

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                if (!usingYolo) {
                    //choose one, double here
                    valueX = BitConverter.ToDouble(data, 0);
                    stringX = valueX.ToString();
                    valueY = BitConverter.ToDouble(data, 8);
                    stringY = valueY.ToString();
                    valueZ = BitConverter.ToDouble(data, 16);
                    stringZ = valueZ.ToString();


                    valueYaw = BitConverter.ToDouble(data, 24);
                    stringYaw = valueYaw.ToString();
                    valueP = BitConverter.ToDouble(data, 32);
                    stringP = valueP.ToString();
                    valueR = BitConverter.ToDouble(data, 40);
                    stringR = valueR.ToString();
                }
                else {
                    valueX = BitConverter.ToDouble(data, 0);
                    stringX = valueX.ToString();
                    valueY = BitConverter.ToDouble(data, 8);
                    stringY = valueY.ToString();
                    valueZ = 500 / (BitConverter.ToDouble(data, 16));
                    stringZ = valueZ.ToString();
                    shouldChangePos = true;
                    Debug.Log("Shoudl change pos");
                }
            }
            catch (Exception err) {
                print(err.ToString());
            }
        }
    }

    //Prevent crashes - close clients and threads properly!
    void OnDisable() {
        if (receiveThread1 != null)
            receiveThread1.Abort();

        try {
            client.Close();
        }
        catch {
            Debug.Log("Didn't quit client correctly");
        }
    }

}