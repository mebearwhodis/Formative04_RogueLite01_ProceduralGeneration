using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.IslandState);
        SoundManager.Instance.PlaySound("GameStart");
    }

    public void ShowCredits()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.CreditsState);
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