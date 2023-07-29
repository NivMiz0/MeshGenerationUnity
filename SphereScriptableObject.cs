using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum Shape
{
    Circle,
    Basketball,
    Sphere
}

[CreateAssetMenu]
public class SphereScriptableObject : ScriptableObject
{
    [Range(0, 100)]
    public int resolution;
    [Range(0, 100)]
    public int columns;
    [Range(0, 100)]
    public float radius;

    public Shape shape;
}
