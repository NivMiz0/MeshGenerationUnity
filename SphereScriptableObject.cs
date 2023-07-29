using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SphereScriptableObject : ScriptableObject
{
    [Range(0, 100)]
    public int resolution;
    [Range(0, 100)]
    public int columns;
    [Range(0, 100)]
    public float radius;
}
