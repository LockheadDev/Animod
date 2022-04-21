using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SphereGizmo : MonoBehaviour
{
    public bool enNumber = true;
    public bool enSphere = false;

    public float size = 0.5f;
    public Color color = Color.cyan;
    public int number;
    private void OnDrawGizmos()
    {
        if (enSphere)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, size);
        }
        if (enNumber)
        {
            Handles.Label(transform.position, number.ToString());
        }
    }
}
