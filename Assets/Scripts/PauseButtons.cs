using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtons : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.SetPause();
    }
    
    public void MainMenu()
    {
    GameManager.Instance.SetGameState(GameManager.GameState.MainMenuState);
    }
}
