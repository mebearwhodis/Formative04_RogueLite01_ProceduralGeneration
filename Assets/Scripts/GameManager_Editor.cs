using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gen = (GameManager)target;
        
        if (GUILayout.Button("Island"))
        {
            gen.SetGameState(GameManager.GameState.IslandState);
        }
        if (GUILayout.Button("Dungeon"))
        {
            gen.SetGameState(GameManager.GameState.DungeonState);
        }
    }
}