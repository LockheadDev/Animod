using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalogSceneSequencer : MonoBehaviour
{
    //Degree mapping values
    public float xMaxdegValue;
    public float xMindegValue;
    public float yMaxdegValue;
    public float yMindegValue;
    [Space]
    //World space mapping values
    public float xMaxmapValue;
    public float xMinmapValue;
    public float yMaxmapValue;
    public float yMinmapValue;
    //Interface
    [SerializeField]
    public UDPAnalogComm udpanalogcomm { get; set; }

    //Create Singleton
    public static AnalogSceneSequencer Instance { get; private set; }

    [SerializeField]
    private GameObject sphere;
    private float temp_x, temp_y = 0;

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
        if (!udpanalogcomm) udpanalogcomm = GameObject.Find("UDPClient").GetComponent<UDPAnalogComm>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }
    public void UpdateAnimation()
    {
            temp_x = Extension.Remap(udpanalogcomm.xValue,xMindegValue,xMaxdegValue,xMinmapValue,xMaxmapValue);
            temp_y = Extension.Remap(udpanalogcomm.yValue, yMindegValue, yMaxdegValue, yMinmapValue, yMaxmapValue);

        sphere.transform.SetPositionAndRotation(new Vector3(temp_x, temp_y), Quaternion.identity);
    }
}
