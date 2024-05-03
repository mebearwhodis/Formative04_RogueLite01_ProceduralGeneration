using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{    
    public void Retry()
    {
        GameManager.Instance.SetGameState(GameManager.GameState.MainMenuState);
    }
}
