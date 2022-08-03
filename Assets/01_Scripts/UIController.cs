using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private UDPAnalogComm UDPController;

    public void verifyUDP(bool boolean)
    {
       if(boolean) UDPController.StartConnection();
       else UDPController.StopConnection();
    }

    public void updateIP(string ip)
    {
        UDPController.ip = ip;
    }
    public void updatePort(string port)
    {
        int num_port = Int32.Parse(port);
        UDPController.port = num_port;
    }

}
