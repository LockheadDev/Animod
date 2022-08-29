using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapVal
{
    public float min, max;
}

[CreateAssetMenu(fileName = "New Map Config", menuName = "Mapping configuration")]
public class SOMapConfig : ScriptableObject
{
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

    [Space]
    [Header ("MAP volume output - float")]
    public MapVal OutputVolumeMap;

    [Space]
    [Header("MAP pitch output - float")]
    public MapVal OutputPitchMap;
}
