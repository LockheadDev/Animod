using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public SOInputData inputData;
        public SOOutputData outputData;
    }
    #endregion
    [Space]
    [SerializeField]
    private SOMapConfig mappingConfigurations;
    [Space]
    [SerializeField]
    private AudioManager audioManager;
    [Space]
    [SerializeField]
    private bool enLerpPos = true;
    [Space]
    public List<AnimationSlot> animationSlots = new List<AnimationSlot>();
    
    /*
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
    */

    //Interface
    [SerializeField]
    public UDPAnalogComm udpanalogcomm { get; set; }
    //Create Singleton
    public static AnalogSceneSequencer Instance { get; private set; }

    private void Awake() //Singleton
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        //Find audiomanager
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

    }
    void Start()
    {
        if (!udpanalogcomm) udpanalogcomm = GameObject.Find("UDPClient").GetComponent<UDPAnalogComm>();
    }

    private void FixedUpdate()
    {
        UpdateAnimation();
    }
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
                if(packet.bodyPart==animslot.inputData.bodyPart)
                {
                    UpdateAnimationSlot(animslot,packet.GetValueByEnum(animslot.inputData.variable));
                }
            }
        }
    }

    private void UpdateAnimationSlot(AnimationSlot slot,float value)
    {
        if (!slot.enable) return;
        switch (slot.outputData.control)
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
            case AnimationControlEnum.pitch:
                SetPitch(slot, value);
                break;
            case AnimationControlEnum.volume:
                SetVolume(slot, value);
                break;
        }
    }

    private void SetVolume(AnimationSlot slot, float value)
    {
        // Range of pitch goes from 0 to 1f
        //TODO set range
        audioManager.SetAudioVolume("Music",value);
    }

    private void SetPitch(AnimationSlot slot, float value)
    {

        // Range of pitch goes from 0.1 to 3f
        // TODO change ranges
        audioManager.SetAudioPitch("Music", value);
    }
    private void SetAcc(AnimationSlot slot, float value)
    {
        if(value>Math.Abs(2f))
        slot.go.GetComponent<Rigidbody>().AddForce(new Vector3(value * 10, 0, 0),ForceMode.Impulse);

    }

    private void SetScale(AnimationSlot slot, float value)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        float vec_value = Extension.Remap(value, min, max, mappingConfigurations.OutputScaleMap.min, mappingConfigurations.OutputScaleMap.max);
        slot.go.transform.localScale = new Vector3(vec_value, vec_value, vec_value);
    }
    private void SetColor(AnimationSlot slot, float value)
    {
        float temp_value = Extension.Remap(value, getMinMax(slot.inputData.variable).Item1, getMinMax(slot.inputData.variable).Item2, 0, 255); //TODO HARDCODED TO RED
        Color clr = slot.go.GetComponentInChildren<MeshRenderer>().material.color;
        switch (slot.outputData.color)
        {
            case ColorEnum.none: return;

            case ColorEnum.r:
                clr.r = temp_value / 255f;
                break;
            case ColorEnum.g:
                clr.g = temp_value / 255f;
                break;
            case ColorEnum.b:
                clr.b = temp_value/255f;
                break;
        }
        slot.go.GetComponentInChildren<MeshRenderer>().material.color = clr;
        List<ParticleSystem> ps = slot.go.GetComponentsInChildren<ParticleSystem>().ToList();
        foreach (ParticleSystem item in ps)
        {
            ParticleSystem.MainModule psMain = item.main;

            psMain.startColor = clr;
        }
    }

    private void SetPosition(AnimationSlot slot, float value)
    {
        
        float temp_value = 0f;
        float min=0f, max = 0f;
        Vector3 temp_vec3 = slot.go.transform.position;

        min = getMinMax(slot.inputData.variable).Item1;
        max = getMinMax(slot.inputData.variable).Item2;
       
        switch (slot.outputData.direction)
        {
            case DirectionEnum.none: return ;
            case DirectionEnum.x:
                temp_value = Extension.Remap(value,min,max, mappingConfigurations.OutputPosMapX.min, mappingConfigurations.OutputPosMapX.max);
                temp_vec3.x = -temp_value; //INVERTED WATCH OUT
                break;
            case DirectionEnum.y:
                temp_value = Extension.Remap(value, min, max, mappingConfigurations.OutputPosMapY.min, mappingConfigurations.OutputPosMapY.max);
                temp_vec3.y = temp_value;
                break;
            case DirectionEnum.z:
                temp_value = Extension.Remap(value, min, max, mappingConfigurations.OutputPosMapZ.min, mappingConfigurations.OutputPosMapZ.max);
                temp_vec3.z = temp_value;
                break;
        }
        if (enLerpPos) slot.go.transform.position = Vector3.Lerp(slot.go.transform.position, temp_vec3, Time.deltaTime * 10f);
        else slot.go.transform.position = temp_vec3;
        //print("num " + value + "->" + temp_value);
    }

    private void SetRotation(AnimationSlot slot, float value)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        float temp_val = 0f;
        float tiltX = slot.go.transform.rotation.x;
        float tiltY = slot.go.transform.rotation.y;
        float tiltZ = slot.go.transform.rotation.z;
        Quaternion targetRotation;
        //TODO Debug and set mapping like in SetPosition() method...
        switch (slot.outputData.direction)
        {
            case DirectionEnum.none: return;
            case DirectionEnum.x:
                temp_val = Extension.Remap(value, min, max, mappingConfigurations.OutputRotMapX.min, mappingConfigurations.OutputRotMapX.max);
                tiltX = temp_val;
                break;
            case DirectionEnum.y:
                temp_val = Extension.Remap(value, min, max, mappingConfigurations.OutputRotMapY.min, mappingConfigurations.OutputRotMapY.max);
                tiltY = temp_val;
                break;
            case DirectionEnum.z:
                temp_val = Extension.Remap(value, min, max, mappingConfigurations.OutputRotMapZ.min, mappingConfigurations.OutputRotMapZ.max);
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
                min = mappingConfigurations.InputRotMapX.min;
                max = mappingConfigurations.InputRotMapX.max;
                break;
            case DataReqEnum.rotY:
                max = mappingConfigurations.InputRotMapY.max;
                min = mappingConfigurations.InputRotMapY.min;
                break;
            case DataReqEnum.accX:
                min = mappingConfigurations.InputAccMapX.min;
                max = mappingConfigurations.InputAccMapX.max;
                break;
            case DataReqEnum.accY:
                min = mappingConfigurations.InputAccMapY.min;
                max = mappingConfigurations.InputAccMapY.max;
                break;
            case DataReqEnum.accZ:
                min = mappingConfigurations.InputAccMapZ.min;
                max = mappingConfigurations.InputAccMapZ.max;
                break;
            
            default:
        
                break;
        }
        return Tuple.Create(min, max);
    }
}