using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalogSceneSequencer : MonoBehaviour,ISequencer
{
    //Interface
    [SerializeField]
    public UDPComm udpcomm { get; set; }

    //Create Singleton
    public static AnalogSceneSequencer Instance { get; private set; }

    [SerializeField]
    private GameObject sphere;
    private float temp = 0;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (!udpcomm) udpcomm = GameObject.Find("UDPClient").GetComponent<UDPComm>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }
    public void UpdateAnimation()
    {
        if (temp != udpcomm.animationTag)
        {
            temp = udpcomm.animationTag;
            sphere.transform.SetPositionAndRotation(new Vector3(temp, 0), Quaternion.identity);
        }
    }
}
