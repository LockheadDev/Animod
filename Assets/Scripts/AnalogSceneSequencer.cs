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
        public bool enable;
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

    [Header("MAP acceleration input - deg")]
    //Degree mapping values
    public MapVal InputAccMapX;
    public MapVal InputAccMapY;
    public MapVal InputAccMapZ;

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
    public MapVal OutputRotMapZ;

    [Space]
    [Header("MAP scale output - float")]
    public MapVal OutputScaleMap;

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

    private void FixedUpdate()
    {
        UpdateAnimation();
    }
    // Update is called once per frame
    void Update()
    {
        //UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        
        foreach(BodyDataPacket packet in udpanalogcomm.bodyPackets)
        {
            foreach (AnimationSlot animslot in animationSlots)
            {
                if(packet.bodyPart==animslot.bodyPart)
                {
                    //print("AnalogSequencer: " + packet.bodyPart.ToString() + "=" + packet.GetValueByEnum(animslot.dataReq));
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
        if (!slot.enable) return;
        switch (slot.control)
        {
            case AnimationControlEnum.rotation:
                SetRotation(slot, value);
                break;
            case AnimationControlEnum.position:
                SetPosition(slot, value);
                break;
            case AnimationControlEnum.color:
                SetColor(slot, value);
                break;
            case AnimationControlEnum.scale:
                SetScale(slot, value);
                break;
            case AnimationControlEnum.acceleration:
                SetAcc(slot, value);
                break;
        }
    }

    private void SetAcc(AnimationSlot slot, float value)
    {
        if(value>Math.Abs(2f))
        slot.go.GetComponent<Rigidbody>().AddForce(new Vector3(value * 10, 0, 0),ForceMode.Impulse);

    }

    private void SetScale(AnimationSlot slot, float value)
    {
        float min = getMinMax(slot.dataReq).Item1;
        float max = getMinMax(slot.dataReq).Item2;
        float vec_value = Extension.Remap(value, min, max, OutputScaleMap.min, OutputScaleMap.max);
        slot.go.transform.localScale = new Vector3(vec_value, vec_value, vec_value);
    }
    private void SetColor(AnimationSlot slot, float value)
    {
        float temp_value = Extension.Remap(value, getMinMax(slot.dataReq).Item1, getMinMax(slot.dataReq).Item2, 0, 255); //TODO HARDCODED TO RED
        Color clr = slot.go.GetComponentInChildren<MeshRenderer>().material.color;
        switch (slot.direction)
        {
            case DirectionEnum.x_r:
                clr.r = temp_value / 255f;
                break;
            case DirectionEnum.y_g:
                clr.g = temp_value / 255f;
                break;
            case DirectionEnum.z_b:
                clr.b = temp_value/255f;
                break;
        }
        slot.go.GetComponentInChildren<MeshRenderer>().material.color = clr;
    }

    private void SetPosition(AnimationSlot slot, float value)
    {
        
        float temp_value = 0f;
        float min=0f, max = 0f;
        Vector3 temp_vec3 = slot.go.transform.position;

        min = getMinMax(slot.dataReq).Item1;
        max = getMinMax(slot.dataReq).Item2;
       
        switch (slot.direction)
        {
            case DirectionEnum.x_r:
                temp_value = Extension.Remap(value,min,max,OutputPosMapX.min,OutputPosMapX.max);
                temp_vec3.x = temp_value;
                break;
            case DirectionEnum.y_g:
                temp_value = Extension.Remap(value, min, max, OutputPosMapY.min, OutputPosMapY.max);
                temp_vec3.y = temp_value;
                break;
            case DirectionEnum.z_b:
                temp_value = Extension.Remap(value, min, max, OutputPosMapZ.min, OutputPosMapZ.max);
                temp_vec3.z = temp_value;
                break;
        }
        slot.go.transform.position = Vector3.Lerp(slot.go.transform.position,temp_vec3,Time.deltaTime*10f);
        //print("num " + value + "->" + temp_value);
    }

    private void SetRotation(AnimationSlot slot, float value)
    {
        float min = getMinMax(slot.dataReq).Item1;
        float max = getMinMax(slot.dataReq).Item2;
        float temp_val = 0f;
        float tiltX = slot.go.transform.rotation.x;
        float tiltY = slot.go.transform.rotation.y;
        float tiltZ = slot.go.transform.rotation.z;
        Quaternion targetRotation;
        //TODO Debug and set mapping like in SetPosition() method...
        switch (slot.direction)
        {
            case DirectionEnum.x_r:
                temp_val = Extension.Remap(value, min, max, OutputRotMapX.min, OutputRotMapX.max);
                tiltX = temp_val;
                break;
            case DirectionEnum.y_g:
                temp_val = Extension.Remap(value, min, max, OutputRotMapY.min, OutputRotMapY.max);
                tiltY = temp_val;
                break;
            case DirectionEnum.z_b:
                temp_val = Extension.Remap(value, min, max, OutputRotMapZ.min, OutputRotMapZ.max);
                tiltZ = temp_val;
                break;
        }
        targetRotation = Quaternion.Euler(tiltX, tiltY, tiltZ);
        slot.go.transform.rotation = Quaternion.Slerp(slot.go.transform.rotation,targetRotation,Time.deltaTime*10f);
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
                min = InputAccMapX.min;
                max = InputAccMapX.max;
                break;
            case DataReqEnum.accY:
                min = InputAccMapY.min;
                max = InputAccMapY.max;
                break;
            case DataReqEnum.accZ:
                min = InputAccMapZ.min;
                max = InputAccMapZ.max;
                break;
            
            default:
        
                break;
        }
        return Tuple.Create(min, max);
    }
}