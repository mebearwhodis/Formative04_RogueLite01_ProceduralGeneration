using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Init,
        StartingState,
        MapState,
        IslandState,
        DungeonState
    }

    //public static GameManager instance = null;
    public GameState currentGameState = GameState.Init;

    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    protected override void Awake()
    {
        base.Awake();
        OnStateEnter();
    }

    private void Update()
    {
        Time.timeScale = _isPaused ? 0 : 1;
    }

    public void SetGameState(GameState newState)
    {
        OnStateExit();
        currentGameState = newState;
        OnStateEnter();
    }

    private void OnStateEnter()
    {
        switch (currentGameState)
        {
            case GameState.Init:
                //Initialize game
                break;
            case GameState.StartingState:
                break;
            case GameState.MapState:
                break;
            case GameState.IslandState:
                SceneManager.LoadScene("IslandGenerator");
                Debug.Log("Entering Island Scene");
                break;
            case GameState.DungeonState:
                SceneManager.LoadScene("DungeonGenerator");
                //Mettre ici les fonctions appelées dans l'ordre plutôt que dans leurs Starts respectifs ?
                //Ou alors on fait un "sous-manager" par scène, qu'on appelle ici et qui lui appelle les fonctions ?
                Debug.Log("Entering Dungeon Scene");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnStateExit()
    {
        switch (currentGameState)
        {
            case GameState.Init:
                break;
            case GameState.StartingState:
                break;
            case GameState.MapState:
                break;
            case GameState.IslandState:
                Debug.Log("Exiting Island Scene");
                break;
            case GameState.DungeonState:
                Debug.Log("Exiting Dungeon Scene");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetPause()
    {
        _isPaused = !_isPaused;
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerController.Instance.transform.position;
    }
}