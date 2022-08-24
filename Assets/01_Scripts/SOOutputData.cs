using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extension;
[CreateAssetMenu(fileName = "Output Data SO", menuName = "Output data configuration")]
public class SOOutputData : ScriptableObject
{
    [Header("Output Data")]

    [Space]
    [Header("GameObject")]
    public AnimationControlEnum control;
    public DirectionEnum direction;
    public ColorEnum color;

    [Space]
    [Header("Audio")]
    public AudioEffectEnum audioEffect;
    public AudioChannelEnum channel;

}
