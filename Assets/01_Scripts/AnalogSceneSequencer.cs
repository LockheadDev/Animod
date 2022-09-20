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
    private SOMapConfig mapConfig;
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
            foreach (AnimationSlot slot in animationSlots)
            {
                if(packet.bodyPart==slot.inputData.bodyPart)
                {
                    UpdateAnimationSlot(slot,packet.GetValueByEnum(slot.inputData.variable));
                }
            }
        }
    }

    private void UpdateAnimationSlot(AnimationSlot slot,float value)
    {
        //ENABLE LATCH
        if (!slot.enable) return;
        //CHECK IF CONFIGURATION OF INPUTS ITS COMPLETE
        if (slot.outputData==null || slot.inputData == null || slot.go == null)
        {
            Debug.LogError("!! - SLOT: " + slot.name + " needs to be fully configured!");
            return;
        }
        //CHECK AND UPDATE ALL CONFIGURED ANIMATIONS
        foreach (RotationEffect posEffect in slot.outputData.rotationEffects)
        {
            SetRotation(slot, value, posEffect.Direction);
        }

        foreach (PositionEffect effect in slot.outputData.positionEffects)
        {
            SetPosition(slot, value, effect.Direction);
        }

        foreach (ScaleEffect effect in slot.outputData.scaleEffects)
        {
            SetScale(slot, value, effect.Direction);
        }
        foreach (AccelerationEffect effect in slot.outputData.accelerationEffects)
        {
            //TODO Implement
            //SetAcceleration(slot, value, effect.Direction);
        }
        foreach (ColorEffect effect in slot.outputData.colorEffects)
        {
            SetColor(slot, value, effect.color);
        }
        foreach (PitchEffect effect in slot.outputData.pitchEffects)
        {
            SetPitch(slot, value, effect.channel);
        }
        foreach (VolumeEffect effect in slot.outputData.volumeEffects)
        {
            SetVolume(slot, value, effect.channel);
        }
        foreach (PanEffect effect in slot.outputData.panEffects)
        {
            SetPan(slot, value, effect.channel);
        }
        foreach (LPFEffect effect in slot.outputData.lpfEffects)
        {
            SetLPF(slot, value, effect.channel);
        }

    }
    private void SetLPF(AnimationSlot slot, float value, AudioChannelEnum channel)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        value = Extension.Remap(value, min, max, mapConfig.OutputLPFmap.min, mapConfig.OutputLPFmap.max);
        audioManager.SetlpfValues(value);
    }
    private void SetVolume(AnimationSlot slot, float value, AudioChannelEnum channel)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        value = Extension.Remap(value, min, max, mapConfig.OutputVolumeMap.min, mapConfig.OutputVolumeMap.max);
        audioManager.SetAudioVolume("Music",value);
    }
    private void SetPan(AnimationSlot slot, float value, AudioChannelEnum channel)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        value = Extension.Remap(value, min, max, mapConfig.OutputPanMap.min, mapConfig.OutputPanMap.max);
        audioManager.SetAudioPan("Music",value);
    }
    private void SetPitch(AnimationSlot slot, float value, AudioChannelEnum channel )
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        value = Extension.Remap(value, min, max, mapConfig.OutputPitchMap.min, mapConfig.OutputPitchMap.max);
        audioManager.SetAudioPitch("Music", value);
    }
    private void SetAcc(AnimationSlot slot, float value)
    {
        if(value>Math.Abs(2f))
        slot.go.GetComponent<Rigidbody>().AddForce(new Vector3(value * 10, 0, 0),ForceMode.Impulse);
    }

    private void SetScale(AnimationSlot slot, float value, DirectionEnum direction)
    {
        //TODO Split (Right now we controll the absolute magnitude)
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        float vec_value = Extension.Remap(value, min, max, mapConfig.OutputScaleMap.min, mapConfig.OutputScaleMap.max);
        slot.go.transform.localScale = new Vector3(vec_value, vec_value, vec_value);
    }
    private void SetColor(AnimationSlot slot, float value, ColorEnum color)
    {
        float temp_value = Extension.Remap(value, getMinMax(slot.inputData.variable).Item1, getMinMax(slot.inputData.variable).Item2, 0, 255); //TODO HARDCODED TO RED
        Color clr = slot.go.GetComponentInChildren<MeshRenderer>().material.color;
        switch (color)
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

    private void SetPosition(AnimationSlot slot, float value, DirectionEnum direction)
    {
        
        float temp_value = 0f;
        float min=0f, max = 0f;
        Vector3 temp_vec3 = slot.go.transform.position;

        min = getMinMax(slot.inputData.variable).Item1;
        max = getMinMax(slot.inputData.variable).Item2;
       
        switch (direction)
        {
            case DirectionEnum.none: return ;
            case DirectionEnum.x:
                temp_value = Extension.Remap(value,min,max, mapConfig.OutputPosMapX.min, mapConfig.OutputPosMapX.max);
                temp_vec3.x = -temp_value; //INVERTED WATCH OUT
                break;
            case DirectionEnum.y:
                temp_value = Extension.Remap(value, min, max, mapConfig.OutputPosMapY.min, mapConfig.OutputPosMapY.max);
                temp_vec3.y = temp_value;
                break;
            case DirectionEnum.z:
                temp_value = Extension.Remap(value, min, max, mapConfig.OutputPosMapZ.min, mapConfig.OutputPosMapZ.max);
                temp_vec3.z = temp_value;
                break;
        }
        if (enLerpPos) slot.go.transform.position = Vector3.Lerp(slot.go.transform.position, temp_vec3, Time.deltaTime * 10f);
        else slot.go.transform.position = temp_vec3;
        //print("num " + value + "->" + temp_value);
    }

    private void SetRotation(AnimationSlot slot, float value,DirectionEnum direction)
    {
        float min = getMinMax(slot.inputData.variable).Item1;
        float max = getMinMax(slot.inputData.variable).Item2;
        float temp_val = 0f;
        float tiltX = slot.go.transform.rotation.x;
        float tiltY = slot.go.transform.rotation.y;
        float tiltZ = slot.go.transform.rotation.z;
        Quaternion targetRotation;
        //TODO Debug and set mapping like in SetPosition() method...
        switch (direction)
        {
            case DirectionEnum.none: return;
            case DirectionEnum.x:
                temp_val = Extension.Remap(value, min, max, mapConfig.OutputRotMapX.min, mapConfig.OutputRotMapX.max);
                tiltX = temp_val;
                break;
            case DirectionEnum.y:
                temp_val = Extension.Remap(value, min, max, mapConfig.OutputRotMapY.min, mapConfig.OutputRotMapY.max);
                tiltY = temp_val;
                break;
            case DirectionEnum.z:
                temp_val = Extension.Remap(value, min, max, mapConfig.OutputRotMapZ.min, mapConfig.OutputRotMapZ.max);
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
                min = mapConfig.InputRotMapX.min;
                max = mapConfig.InputRotMapX.max;
                break;
            case DataReqEnum.rotY:
                max = mapConfig.InputRotMapY.max;
                min = mapConfig.InputRotMapY.min;
                break;
            case DataReqEnum.accX:
                min = mapConfig.InputAccMapX.min;
                max = mapConfig.InputAccMapX.max;
                break;
            case DataReqEnum.accY:
                min = mapConfig.InputAccMapY.min;
                max = mapConfig.InputAccMapY.max;
                break;
            case DataReqEnum.accZ:
                min = mapConfig.InputAccMapZ.min;
                max = mapConfig.InputAccMapZ.max;
                break;
            
            default:
        
                break;
        }
        return Tuple.Create(min, max);
    }
}