using UnityEngine;
interface ISequencer
{
    UDPComm udpcomm { get; set; }
    void UpdateAnimation();
}