using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Data", menuName = "Effect Data")]
public class EffectData : ScriptableObject
{
    public float duration;
    public Color color;
    public int intensity;

    public bool isRandomColor = false;
    void OnEnable()
    {

        if (isRandomColor)
        {
            Color temp_color = new Color(
      Random.Range(0f, 1f),
      Random.Range(0f, 1f),
      Random.Range(0f, 1f)
        );
            color = temp_color;
        }
    }
}
