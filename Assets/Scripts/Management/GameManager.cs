using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        InitState,
        MainMenu,
        Credits,
        StartingState,
        IslandState,
        DungeonState,
        GameOver,
        EndState
    }

    [SerializeField] private GameObject _pauseMenu;
    
    //public static GameManager instance = null;
    public GameState currentGameState = GameState.InitState;

    private bool _canPause = false;
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
            case GameState.InitState:
                //Initialize game
                _canPause = false;
                break;
            case GameState.MainMenu:
                _canPause = false;
                break;  
            case GameState.Credits:
                SceneManager.LoadScene("Credits");
                _canPause = false;
                break;
            case GameState.StartingState:
                break;
            case GameState.IslandState:
                SceneManager.LoadScene("IslandGenerator");
                _canPause = true;
                break;
            case GameState.DungeonState:
                SceneManager.LoadScene("DungeonGenerator");
                _canPause = true;
                break;
            case GameState.GameOver:
                SceneManager.LoadScene("GameOver");
                _canPause = false;
                break;
            case GameState.EndState:
                SceneManager.LoadScene("EndScreen");
                _canPause = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnStateExit()
    {
        switch (currentGameState)
        {
            case GameState.InitState:
                break;
            case GameState.MainMenu:
                break;  
            case GameState.Credits:
                break;
            case GameState.StartingState:
                break;
            case GameState.IslandState:
                break;
            case GameState.DungeonState:
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetPause()
    {
        if(!_canPause){Debug.Log("Can't pause here");return;}
        _isPaused = !_isPaused;
        _pauseMenu.SetActive(_isPaused);
        //de/Activate Pause Menu
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerController.Instance.transform.position;
    }
}