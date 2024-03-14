using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandGenerator))]
public class IslandGenerator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        IslandGenerator gen = (IslandGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            gen.GenerateMap();
        }
    }
}