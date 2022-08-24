using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extension;

//TODO HANDLE LISTS of variables for better handling
[System.Serializable]
public class PositionEffect 
{
    public AnimationControlEnum control = AnimationControlEnum.position;
    public DirectionEnum Direction;
}
[System.Serializable]
public class RotationEffect 
{
    public AnimationControlEnum control = AnimationControlEnum.rotation;
    public DirectionEnum Direction;
}
[System.Serializable]
public class ScaleEffect
{
    public AnimationControlEnum control = AnimationControlEnum.scale;
    public DirectionEnum Direction;
}
[System.Serializable]
public class AccelerationEffect 
{
    public AnimationControlEnum control = AnimationControlEnum.acceleration;
    public DirectionEnum Direction;
}
[System.Serializable]
public class ColorEffect
{
    public AnimationControlEnum control = AnimationControlEnum.color;
    public ColorEnum color;
}
[System.Serializable]
public class PitchEffect
{
    public AnimationControlEnum control = AnimationControlEnum.pitch;
    public AudioChannelEnum channel;
}
[System.Serializable]
public class VolumeEffect
{
    public AnimationControlEnum control = AnimationControlEnum.volume;
    public AudioChannelEnum channel;
}

[CreateAssetMenu(fileName = "Output Data SO", menuName = "Output data configuration")]
public class SOOutputData : ScriptableObject
{
    //TODO finish lists handling with AnalogSceneSequencer
    [Header("Output Data")]
    public List<PositionEffect> positionEffects = new List<PositionEffect>();   
    public List<RotationEffect> rotationEffects = new List<RotationEffect>();
    public List<ScaleEffect> scaleEffects = new List<ScaleEffect>();    
    public List<AccelerationEffect> accelerationEffects = new List<AccelerationEffect>();
    public List<ColorEffect> colorEffects = new List<ColorEffect>();
    public List<PitchEffect> pitchEffects = new List<PitchEffect>();
    public List<VolumeEffect> volumeEffects = new List<VolumeEffect>();
}
