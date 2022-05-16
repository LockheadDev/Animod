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
    //TODO Customize each packet data if not interested in any of the input values
    #region Classes and structs
    [System.Serializable]
    public class BodyDataPacket
    {
        public String name;
        public BodyPartEnum bodyPart;
        public float rotX, rotY, accX, accY, accZ;

        public void SetValueByEnum(DataReqEnum dataReq,float value)
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
    [SerializeField]private bool enConnection = false;
    [SerializeField]private int port = 42069;
    public float temp_input;

    [Space]
    [Header("Data management")]
    public List<BodyPartEnum> bodyPartSequence = new List<BodyPartEnum>();
    public List<DataReqEnum> dataReqSequence = new List<DataReqEnum>();
    public List<BodyDataPacket> bodyPackets = new List<BodyDataPacket>();

    private UdpClient UDPclient = new UdpClient();
    private IPEndPoint ep;
    private string recievedString;

    //UDP Thread
    private Thread thread;

    void Start()
    {
        //ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);    //Initialize port LOOPBACK
        ep = new IPEndPoint(IPAddress.Parse("192.168.0.100"), port); // KHERI ADDRESS
        if (enConnection) StartConnection();                        //Start connection
    }

    // Update is called once per frame
    void Update()
    {
        //KEY: T -> Finish connection
        if (Input.GetKeyDown(KeyCode.T)) StopConnection();


    }
    private void RequestServer()
    {
        while (true)
        {
            print("Sending request data...");

            //REQUEST and STORE data
            foreach (BodyPartEnum bodyPart in bodyPartSequence)
            {
                foreach (DataReqEnum dataReq in dataReqSequence)
                {
                    SendRecieveData(bodyPart,dataReq);
                }
            }
        }
    }
    private void StartConnection()
    {
        thread = new Thread(new ThreadStart(RequestServer));
        UDPclient.Connect(ep);
        thread.Start();
    }
    private void StopConnection()
    {
        UDPclient.Close();
        thread.Abort();
    }
    private void sendRequestData(UdpClient client, string str)
    {
        byte[] message = Encoding.ASCII.GetBytes(str);
        client.Send(message, str.Length);
    }

    private void SendRecieveData(BodyPartEnum bodyPart, DataReqEnum dataReq)
    {
        
        sendRequestData(UDPclient, GetRequestString(bodyPart,dataReq));
        try
        {
            recievedString = Encoding.ASCII.GetString(UDPclient.Receive(ref ep));
            print("No string recieved");
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
                str += "c";
                break;
            case BodyPartEnum.footRight:
                str += "fr";
                break;
            case BodyPartEnum.footLeft:
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
    private void OnApplicationQuit()
    {
        thread.Abort();
    }
    private void OnDestroy()
    {
        thread.Abort();
    }
}
