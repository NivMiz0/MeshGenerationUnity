using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateMesh))]
public class MeshGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (GenerateMesh)target;

        if(GUILayout.Button("Generate", GUILayout.Height(20)))
        {
            script.GenerateShape();
        }
        
    }
}