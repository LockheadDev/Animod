using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPAnalogComm : MonoBehaviour
{
    [SerializeField]
    private bool enConnection = false;

    [SerializeField]
    private int port = 42069;

    [SerializeField]
    private String defString = "r";
    private String yRequestString= "y";
    private String xRequestString = "x";

    public float temp_input ;

    public float yValue = 0;
    public float xValue = 0;

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
            //Send Request Data
            //sendRequestData(UDPclient, defString);

            //REQUEST Y
            sendRequestData(UDPclient, yRequestString);
            //Receive Response Data Y
            try
            {
                recievedString = Encoding.ASCII.GetString(UDPclient.Receive(ref ep));
                print("No string recieved");
            }
            catch
            {

            }
            if (recievedString != null)
            {
                if (float.TryParse(recievedString, out temp_input))
                {
                    print("Recieved YValue: " + temp_input.ToString());
                    yValue = temp_input;
                    recievedString = null;
                }
                else print(recievedString); //Recibimos "w" y la imprimimos
            }
            else
            {
                print("No string recieved");
            }
            //REQUEST X
            sendRequestData(UDPclient, xRequestString);
            //Receive Response Data X
            try
            {
                recievedString = Encoding.ASCII.GetString(UDPclient.Receive(ref ep));
                print("No string recieved");
            }
            catch
            {

            }
            if (recievedString != null)
            {
                if (float.TryParse(recievedString, out temp_input))
                {
                    print("Recieved XValue: " + temp_input.ToString());
                    xValue = temp_input;
                    recievedString = null;
                }
                else print(recievedString); // Recibimos "w" y la imprimimos
            }
            else
            {
                print("No string recieved");
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
