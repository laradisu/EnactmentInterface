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

    // range of the character/object in camera coordinates
    public Vector2 xRange = new Vector2(-50, 50);
    public Vector2 yRange = new Vector2(0, 75);
    public bool useRangeNotDamping = true;

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


    double valueX = 0;
    double valueY = 0;
    double width = 0;
    double height = 0;

    bool shouldChangePos = false;

    // start from Unity3d
    void Start() {
        valueX = transform.position.x;
        valueY = transform.position.y;

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

	// set positions to bottom center of YOLO tracking box
	valueX += width/2;
	valueY += height;

        if (useRangeNotDamping) {
            float xRatio = (float)valueX / (float)xRange.y;
            float yRatio = (float)valueY / (float)yRange.y;

            pos.x = xRatio * xGameRange.y;
            pos.y = yRatio * yGameRange.y;
        }
        else {
            pos.x = (float)valueX * xMotionDamping;
            pos.y = (float)valueY * yMotionDamping;
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

                if (usingYolo) {
                    valueX = BitConverter.ToDouble(data, 0);
                    valueY = BitConverter.ToDouble(data, 8);
                    width = BitConverter.ToDouble(data, 16);
		    height = BitConverter.ToDouble(data, 24);
                    shouldChangePos = true;
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
