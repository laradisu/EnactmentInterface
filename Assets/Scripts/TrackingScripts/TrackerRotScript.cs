using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TrackerRotScript : MonoBehaviour
{
    //local host
    public string IP = "127.0.0.3";

    //Ports
    public int portLocal = 3000;

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


    // public string lastReceivedUDPPacket = "";
    // public string allReceivedUDPPackets = "";
    // clear this from time to time!

    // start from Unity3d
    void Start()
    {
        // Create remote endpoint (to Matlab) 
        // remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), portRemote);

        // Create local client
        //  client = new UdpClient(portLocal);
        init();
        Application.targetFrameRate = 30;
    }

    //originally no update function at all, all stuff in onGUI()
    void FixedUpdate()
    {
        var pos = this.transform.position;
        Quaternion rot = this.transform.rotation;
        //pos.x = valueX;
        //this.transform.position = pos;


        //var pos = this.transform.position;
        //var rot = this.transform.rotation;

        //IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        //byte[] data = client.Receive(ref anyIP);

        //int byteLength = Buffer.ByteLength(data);

        //need to change to float/double
        //float first
        //float valueX = BitConverter.ToSingle(data, 0);
        //stringX = valueX.ToString();
        //float valueY = BitConverter.ToSingle(data, 4);
        //stringY = valueY.ToString();
        //float valueZ = BitConverter.ToSingle(data, 8);
        //stringZ = valueZ.ToString();
        //float valueR = BitConverter.ToSingle(data, 12);
        //stringR = valueR.ToString();
        //float valueP = BitConverter.ToSingle(data, 16);
        //stringP = valueP.ToString();
        //float valueYaw = BitConverter.ToSingle(data, 20);
        //stringYaw = valueYaw.ToString();


        //  string lengthByte = byteLength.ToString();

        //print(">> " + text);
        //lastReceivedUDPPacket = lengthByte;//text;
        //allReceivedUDPPackets = allReceivedUDPPackets + text;

        //add (float) for strict data transform
        pos.x = (float)valueX * xMotionDamping;
        pos.y = (float)valueY * yMotionDamping;

        if (keyboardInputDepth)
        {
            if (useShiftInput == Input.GetKey(KeyCode.LeftShift))
            { 
                if (Input.GetKey(KeyCode.End))
                    zOffset -= 0.2f;
                if (Input.GetKey(KeyCode.Home))
                    zOffset += 0.2f;
            }
        }

        pos.z = zOffset + 0f*(float)valueZ*zMotionDamping;  // Z IS CONSTANT NOW (with keyboard input)
        if (useRangeNotDamping)
        {
            float xRatio = (float)valueX / (float)xRange.y;
            float yRatio = (float)valueY / (float)yRange.y;
            float zRatio = (float)valueZ / (float)zRange.y;

            pos.x = xRatio * xGameRange.y;
            pos.y = yRatio * yGameRange.y;
        }

        rot.x = (float) valueYaw;
        rot.y = (float) valueP;
        rot.z = (float) valueR;

        if (mapToPhoneRotation)
        {
            valueR = -valueR;
            var tempyaw = valueYaw;
            valueYaw = -valueP;
            valueP = -tempyaw;
        }

        if (turnAroundY)
        {
            valueP += 180;
        }
        
        if (keyboardInputYRot)
        {
            if (useShiftInput == Input.GetKey(KeyCode.LeftShift))
            {
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

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.T))
        {
            valueYaw = 0;
            valueR = 0;
        }

        if (flipX)
            pos.x *= -1f;
        if (flipY)
            pos.y *= -1f;

        pos.x += translatePos.x;
        pos.y += translatePos.y;

       
            this.transform.rotation = Quaternion.Euler((float)valueYaw, (float)valueP, (float)valueR);
    }

    /*
    void OnDestroy()
    {
        client.Close();
    }
    */




    // OnGUI
    /*
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDP Object Receive\n127.0.0.1:" + portLocal + "\n"
        + "\n X = " + stringX //"\nLast Packet: \n" + //lastReceivedUDPPacket
        + "\n Y = " + stringY
        + "\n Z = " + stringZ
        + "\n R = " + stringR
        + "\n P = " + stringP
        + "\n Yaw = " + stringYaw//  + "\n\nAll Messages: \n" + allReceivedUDPPackets
            , style);

        strMessageSend = GUI.TextField(new Rect(40, 420, 140, 20), strMessageSend);
        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
        {
            sendData(strMessageSend + "\n");
        }

    }

    */

    // Initialization code
    private void init()
    {
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
    private void ReceiveData()
    {
        while (true)
        {

            try
            {
                //var pos = this.transform.position;
                //var rot = this.transform.rotation;

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

             

                    valueYaw = BitConverter.ToDouble(data, 24);
                    stringYaw = valueYaw.ToString();
                    valueP = BitConverter.ToDouble(data, 32);
                    stringP = valueP.ToString();
                    valueR = BitConverter.ToDouble(data, 40);
                    stringR = valueR.ToString();
                
               


                //int byteLength = Buffer.ByteLength(data);

                //float valueX = BitConverter.ToSingle(data, 0);
                //stringX = valueX.ToString();
                //float valueY = BitConverter.ToSingle(data, 4);
                //stringY = valueY.ToString();
                //float valueZ = BitConverter.ToSingle(data, 8);
                //stringZ = valueZ.ToString();
                //float valueR = BitConverter.ToSingle(data, 12);
                //stringR = valueR.ToString();
                //float valueP = BitConverter.ToSingle(data, 16);
                //stringP = valueP.ToString();
                //float valueYaw = BitConverter.ToSingle(data, 20);
                //stringYaw = valueYaw.ToString();

                //string lengthByte = byteLength.ToString();

                //print(">> " + text);
                //lastReceivedUDPPacket = lengthByte;//text;
                //allReceivedUDPPackets = allReceivedUDPPackets + text;

                //pos.x = stringX.toFloat;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }


    /*
    // Send data
    private void sendData(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);

        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    */


    /*
    // getLatestUDPPacket, clears all previous packets
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
    */

    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        if (receiveThread1 != null)
            receiveThread1.Abort();

        client.Close();
    }

}
