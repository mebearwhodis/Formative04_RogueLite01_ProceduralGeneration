using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        InitState,
        MainMenuState,
        CreditsState,
        IslandState,
        DungeonState,
        GameLostState,
        GameWonState
    }

    [SerializeField] private GameObject _pauseMenu;

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
                if (PlayerController.Instance is not null)
                {
                    PlayerController.Instance.gameObject.SetActive(false);
                    PlayerController.Instance.RemainingHealth = PlayerController.Instance.MaxPlayerHealth;
                    PlayerController.Instance.CoinAmount = 0;
                }

                _canPause = false;
                break;
            case GameState.MainMenuState:
                _isPaused = false;
                _pauseMenu.SetActive(false);
                if (PlayerController.Instance is not null)
                {
                    PlayerController.Instance.Destroy();
                }

                SceneManager.LoadScene("MainMenu");
                _canPause = false;
                break;
            case GameState.CreditsState:
                SceneManager.LoadScene("Credits");
                _canPause = false;
                break;
            case GameState.IslandState:
                SceneManager.LoadScene("IslandGenerator");
                _canPause = true;
                break;
            case GameState.DungeonState:
                SceneManager.LoadScene("DungeonGenerator");
                _canPause = true;
                break;
            case GameState.GameLostState:
                PlayerController.Instance.gameObject.SetActive(false);
                SceneManager.LoadScene("GameLost");
                _canPause = false;
                break;
            case GameState.GameWonState:
                PlayerController.Instance.gameObject.SetActive(false);
                SceneManager.LoadScene("GameWon");
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
            case GameState.MainMenuState:
                break;
            case GameState.CreditsState:
                break;
            case GameState.IslandState:
                break;
            case GameState.DungeonState:
                break;
            case GameState.GameLostState:
                break;
            case GameState.GameWonState:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetPause()
    {
        if (!_canPause)
        {
            return;
        }

        _isPaused = !_isPaused;
        _pauseMenu.SetActive(_isPaused);
    }

    public Vector3 GetPlayerPosition()
    {
        return PlayerController.Instance.transform.position;
    }
}