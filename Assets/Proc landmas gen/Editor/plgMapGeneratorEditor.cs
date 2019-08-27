using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(plgMapGenerator))]
public class plgMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        plgMapGenerator mapGen = (plgMapGenerator)target;

        if(DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }

    }
}
