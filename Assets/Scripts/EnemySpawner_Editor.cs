using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawner_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemySpawner spawner = (EnemySpawner)target;
        
        if (GUILayout.Button("Spawn a Bombite (Explodes)"))
        {
            spawner.SpawnBombite();
        }
        if (GUILayout.Button("Spawn a Leever (Hides)"))
        {
            spawner.SpawnLeever();
        }   
        if (GUILayout.Button("Spawn a Spiked Beetle (Rushes)"))
        {
            spawner.SpawnSpikedBeetle();
        }
        if (GUILayout.Button("Spawn a Stalfos (Attacks at range)"))
        {
            spawner.SpawnStalfos();
        }
    }
}