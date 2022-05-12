using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extension;
using static UDPAnalogComm;

public class AnalogSceneSequencer : MonoBehaviour
{
    #region Classes and structs
    [System.Serializable]
    public class AnimationSlot
    {
        public string name;
        public GameObject go;
        [Space]
        [Header("Input data")]
        public BodyPartEnum bodyPart;
        public DataReqEnum dataReq;
        [Space]
        [Header("Animation Control")]
        public AnimationControlEnum control;
        public DirectionEnum direction;
    }
    [System.Serializable]
    public struct MapVal
    {
        public float min, max;
    }
    #endregion

    [Space]
    public List<AnimationSlot> animationSlots = new List<AnimationSlot>();
    
    [Space]
    [Header("MAP rotation input - deg")]
    //Degree mapping values
    public MapVal InputRotMapX;
    public MapVal InputRotMapY;

    [Space]
    [Header("MAP position output - float")]
    //World space mapping values
    public MapVal OutputPosMapX;
    public MapVal OutputPosMapY;
    public MapVal OutputPosMapZ;

    [Space]
    [Header("MAP rotation output - float")]
    //World rotation mapping values
    public MapVal OutputRotMapX;
    public MapVal OutputRotMapY;

    //Interface
    [SerializeField]
    public UDPAnalogComm udpanalogcomm { get; set; }
    //Create Singleton
    public static AnalogSceneSequencer Instance { get; private set; }


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

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
    private void UpdateAnimation()
    {
        
        foreach(BodyDataPacket packet in udpanalogcomm.bodyPackets)
        {
            foreach (AnimationSlot animslot in animationSlots)
            {
                if(packet.bodyPart==animslot.bodyPart)
                {
                    UpdateAnimationSlot(animslot,packet.GetValueByEnum(animslot.dataReq));
                }
            }
        }

        /*
            temp_x = Extension.Remap(udpanalogcomm.xValue,xMindegValue,xMaxdegValue,xMinmapValue,xMaxmapValue);
            temp_y = Extension.Remap(udpanalogcomm.yValue, yMindegValue, yMaxdegValue, yMinmapValue, yMaxmapValue);
        */

        //sphere.transform.SetPositionAndRotation(new Vector3(temp_x, temp_y), Quaternion.identity);
    }

    private void UpdateAnimationSlot(AnimationSlot slot,float value)
    {
        switch (slot.control)
        {
            case AnimationControlEnum.rotation:
                SetRotation(slot,value);
                break;
            case AnimationControlEnum.position:
                SetPosition(slot, value);
                break;
            case AnimationControlEnum.color:
                SetColor(slot, value);
                break;
        }
    }

    private void SetColor(AnimationSlot slot, float value)
    {
        float temp_value = Extension.Remap(value, getMinMax(slot.dataReq).Item1, getMinMax(slot.dataReq).Item2, 0, 255); //TODO HARDCODED TO RED
        slot.go.GetComponent<MeshRenderer>().material.color = new Color(temp_value, 200, 200);
    }

    private void SetPosition(AnimationSlot slot, float value)
    {
        float temp_value = 0f;
        float min=0f, max = 0f;

        min = getMinMax(slot.dataReq).Item1;
        max = getMinMax(slot.dataReq).Item2;

        switch (slot.direction)
        {
            case DirectionEnum.x:
                temp_value = Extension.Remap(value,min,max,OutputPosMapX.min,OutputPosMapX.max);
                slot.go.transform.SetPositionAndRotation(slot.go.transform.position + new Vector3(temp_value, 0, 0),Quaternion.identity);
                break;
            case DirectionEnum.y:
                temp_value = Extension.Remap(value, min, max, OutputPosMapY.min, OutputPosMapY.max);
                slot.go.transform.SetPositionAndRotation(slot.go.transform.position + new Vector3(0, temp_value, 0), Quaternion.identity);
                break;
            case DirectionEnum.z:
                temp_value = Extension.Remap(value, min, max, OutputPosMapZ.min, OutputPosMapZ.max);
                slot.go.transform.SetPositionAndRotation(slot.go.transform.position + new Vector3(0, 0, temp_value), Quaternion.identity);
                break;
        }
    }

    private void SetRotation(AnimationSlot slot, float value)
    {
        //TODO Debug and set mapping like in SetPosition() method...
        switch (slot.direction)
        {
            case DirectionEnum.x:
                slot.go.transform.Rotate(value, 0, 0);
                break;
            case DirectionEnum.y:
                slot.go.transform.Rotate(0, value, 0);
                break;
            case DirectionEnum.z:
                slot.go.transform.Rotate(0, 0, value);
                break;
        }

    }
    private Tuple<float,float> getMinMax(DataReqEnum dataReq)
    {
        float min=0f, max=0f;
        switch (dataReq)
        {
            case DataReqEnum.rotX:
                min = InputRotMapX.min;
                max = InputRotMapX.max;
                break;
            case DataReqEnum.rotY:
                max = InputRotMapY.max;
                min = InputRotMapY.min;
                break;
            case DataReqEnum.accX:
                //TODO IMPLEMENT ACC MAPPING
                break;
            case DataReqEnum.accY:
                //TODO IMPLEMENT ACC MAPPING
                break;
            case DataReqEnum.accZ:
                //TODO IMPLEMENT ACC MAPPING
                break;
            default:
                break;
        }
        return Tuple.Create(min, max);
    }
}