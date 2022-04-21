using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPComm : MonoBehaviour
{
    [SerializeField]
    private bool enConnection = false;

    [SerializeField]
    private int port = 42069;

    public int animationTag = -1;

    private UdpClient UDPclient = new UdpClient();
    private IPEndPoint ep;
    private string recievedString;

    //UDP Thread
    private Thread thread;

    void Start()
    {
        ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);    //Initialize port
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
            try
            {//Send Request Data
                sendRequestData(UDPclient, "r");

                //Receive Response Data
                recievedString = Encoding.ASCII.GetString(UDPclient.Receive(ref ep));
                if (recievedString != null)
                {
                    print(ep.ToString() + "- Mensaje:" + recievedString);
                    if (int.TryParse(recievedString, out animationTag))
                    {
                        print("Recieved animation tag: " + animationTag.ToString());
                        SceneSequencer.Instance.EnableAnimation(animationTag);
                        //TODO Call Scene Sequencer.EnableAnimation
                    }
                }
                else
                {
                    print("No string recieved");
                }
            }
            catch
            {
                print("Fatal UDP connection failure");
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
}