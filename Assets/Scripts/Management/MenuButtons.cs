using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.IslandState);
    }

    public void ShowCredits()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.Credits);
    }
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}