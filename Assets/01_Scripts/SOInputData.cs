using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extension;

[CreateAssetMenu(fileName = "Input Data SO", menuName = "Input data configuration")]
public class SOInputData : ScriptableObject
{
    [Header("Input Data")]
    public BodyPartEnum bodyPart;
    public DataReqEnum variable;
}

