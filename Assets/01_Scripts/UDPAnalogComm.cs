using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using static Extension;
using System.Collections.Generic;

public class UDPAnalogComm : MonoBehaviour
{
    public static UDPAnalogComm Instance { get; private set; }
    //TODO Customize each packet data if not interested in any of the input values
    #region Classes and structs
    [System.Serializable]
    public class BodyDataPacket
    {
        public String name;
        public BodyPartEnum bodyPart;
        public float rotX, rotY, accX, accY, accZ;

        public void SetValueByEnum(DataReqEnum dataReq, float value)
        {

            switch (dataReq)
            {
                case DataReqEnum.rotX:
                    rotX = value;
                    break;
                case DataReqEnum.rotY:
                    rotY = value;
                    break;
                case DataReqEnum.accX:
                    accX = value;
                    break;
                case DataReqEnum.accY:
                    accY = value;
                    break;
                case DataReqEnum.accZ:
                    accZ = value;
                    break;
            }
            //print(dataReq.ToString() + " " + rotX.ToString());
        }
        public float GetValueByEnum(DataReqEnum dataReq)
        {
            switch (dataReq)
            {
                case DataReqEnum.rotX:
                    return rotX;
                case DataReqEnum.rotY:
                    return rotY;
                case DataReqEnum.accX:
                    return accX;
                case DataReqEnum.accY:
                    return accY;
                case DataReqEnum.accZ:
                    return accZ;
            }
            return 0f;
        }
    }
    #endregion
    [Space]
    [Header("UDP Configuration and data")]
    [SerializeField] private bool enConnection = false;
    [SerializeField] public int port = 42069;
    [SerializeField] public string ip = "127.0.0.1";
    [SerializeField] private float connectionWD =1.0f;
    private float timerWD;
    private bool recievedData = false;
    private float temp_input;
    private bool endConnection = false;

    [Space]
    [Header("Data management")]
    public List<BodyPartEnum> bodyPartSequence = new List<BodyPartEnum>();
    public List<DataReqEnum> dataReqSequence = new List<DataReqEnum>();
    public List<BodyDataPacket> bodyPackets = new List<BodyDataPacket>();

    private UdpClient UDPclient;
    private IPEndPoint ep;
    private string recievedString;

    //UDP Thread
    private Thread thread;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        ep = new IPEndPoint(IPAddress.Parse(ip), port);
        if (enConnection) StartConnection();                        //Start connection
        timerWD = connectionWD;
    }
    private void RequestServer()
    {
        
            while (!endConnection)
            {            
                recievedData = false;
                print("Sending request data...");
                RequestAllParts();
            }
            thread.Abort();

    }
    private void RequestAllParts()
    {
        foreach (BodyPartEnum bodyPart in bodyPartSequence)
        {
            foreach (DataReqEnum dataReq in dataReqSequence)
            {
                SendRecieveData(bodyPart, dataReq);
                recievedData = true;
            }
        }
    }
    public void StartConnection()
    {
        endConnection = false;
        UDPclient = new UdpClient();
        print("Starting connection with IP: " + ip+ " and PORT: " + port.ToString());
        thread = new Thread(new ThreadStart(RequestServer));
        UDPclient.Connect(ep);
        thread.Start();
    }
    public void StopConnection()
    {
        print("Stopping connection with IP: " + ip + " and PORT: " + port.ToString());
        endConnection = true;
        UDPclient.Close();
    }
    private void sendRequestData(UdpClient client, string str)
    {
        byte[] message = Encoding.ASCII.GetBytes(str);
        print("Request: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
        client.Send(message, str.Length);
    }

    private void SendRecieveData(BodyPartEnum bodyPart, DataReqEnum dataReq)
    {
        if (endConnection) return;
        
        sendRequestData(UDPclient, GetRequestString(bodyPart,dataReq));
        try
        {
            print("Recieve: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
            recievedString = Encoding.ASCII.GetString(UDPclient.Receive(ref ep));
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        if (recievedString != null)
        {
            if (float.TryParse(recievedString, out temp_input))
            {
                print("Recieved " + dataReq.ToString() +" from "+bodyPart.ToString()+ " = " + temp_input.ToString());
                StoreDataPacket(bodyPart,dataReq,temp_input);
                recievedString = null;
            }
            else print("On await: " + recievedString); //Recibimos "w" y la imprimimos
        }
        else
        {
            print("No string recieved");
        }
    }

    private void StoreDataPacket(BodyPartEnum bodyPart, DataReqEnum dataReq,float data)
    {
        foreach (BodyDataPacket packet in bodyPackets)
        {
            if (packet.bodyPart == bodyPart) packet.SetValueByEnum(dataReq,data);
        }

    }
        private String GetRequestString(BodyPartEnum bodyPart, DataReqEnum dataReq)
    {
        String str = "";
        switch (bodyPart)
        {
            case BodyPartEnum.armRight:
                str += "ar";
                break;
            case BodyPartEnum.armLeft:
                str += "al";
                break;
            case BodyPartEnum.center:
                str += "cc";
                break;
            case BodyPartEnum.legRight:
                str += "fr";
                break;
            case BodyPartEnum.legLeft:
                str += "fl";
                break;
        }
        switch (dataReq)
        {
            case DataReqEnum.rotX:
                str += "xz";
                break;
            case DataReqEnum.rotY:
                str += "yz";
                break;
            case DataReqEnum.accX:
                str += "x";
                break;
            case DataReqEnum.accY:
                str += "y";
                break;
            case DataReqEnum.accZ:
                str += "z";
                break;
        }
        return str;
    }
}
