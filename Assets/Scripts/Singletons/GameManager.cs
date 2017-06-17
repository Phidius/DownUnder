using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public enum GameState { Play, Options, Dead }

    public AudioClip gameMusic;
    public AudioClip winningSound;
    public AudioClip dieingSound;
    public GameState gameState = GameState.Play;

    private AudioSource _audioSource;
    private GameObject HUDisplay;
    private PlayerController _player;
    private GameObject _optionsPanel;
    private GameObject _hasDiedPanel;
    private bool _lockControllerState = false;
    private List<string> _beenPlayed = new List<string>();
    private GameDifficulty _difficulty = GameDifficulty.Normal;
    private bool _gamePaused;
    private bool _levelFinished = false;
    private int _targetFrameRate;

    private static GameManager _gameManager;
    public static GameManager Instance {  get { return _gameManager; } }

    public enum GameDifficulty { Easy, Normal, Hard }
    private void Awake()
    {
        _targetFrameRate = 300;
        if (GvrViewer.Instance)
        {
            _targetFrameRate = 30;
        }

        Application.targetFrameRate = _targetFrameRate;
        if (_gameManager != null && _gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameManager = this;
        }

        _audioSource = GetComponent<AudioSource>();
    }
    // Use this for initialization
    void Start()
    {
        var displayHolder = Camera.main.transform.Find("DisplayHolder");
        HUDisplay = GameObject.Find("HUDisplay");
        _player =(PlayerController) GameObject.FindObjectOfType<PlayerController>();
        _optionsPanel = GameObject.Find("Options");
        _hasDiedPanel = GameObject.Find("HasDied");

        if (_player == null)
        {
            throw new System.NotImplementedException("The scene must conatin a player object with the script PlayerController attached");
        }
        if (displayHolder == null)
        {
            throw new System.NotImplementedException("The main camera must contain an empty game object called \"DisplayHolder\" to contain the various menus");
        }
        if (HUDisplay == null)
        {
            throw new System.NotImplementedException("Unable to locate the object \"HUDisplay\" in the scene");
        }
        if (_optionsPanel == null)
        {
            throw new System.NotImplementedException("Unable to locate the transform \"Options\" inside the \"HUDisplay\"");
        }
        if (_hasDiedPanel == null)
        {
            throw new System.NotImplementedException("Unable to locate the transform \"HasDied\" inside the \"HUDisplay\"");
        }
        if (_hasDiedPanel == null)
        {
            throw new System.NotImplementedException("Unable to locate the transform \"Inventory\" inside the \"HUDisplay\"");
        }

        _optionsPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _hasDiedPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;


        HUDisplay.transform.parent = displayHolder.transform;

        HUDisplay.transform.localScale = new Vector3(1f, 1f, 1f);
        HUDisplay.transform.localPosition = new Vector3(0f, 0f, 0f);
        HUDisplay.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {

        if (CrossPlatformInputManager.GetButtonDown("Options"))
        {
            if (gameState == GameState.Options)
            {
                gameState = GameState.Play;
            }
            else
            {
                gameState = GameState.Options;
            }
            //showOptions = !showOptions;
        }
        
        //if (showHasDied)
        if (gameState == GameState.Dead)
        {
            if (_audioSource.clip == null || _audioSource.clip.name != dieingSound.name)
            {
                _audioSource.Stop();
                _audioSource.clip = dieingSound;
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource.clip == null || _audioSource.clip.name != gameMusic.name)
            {
                _audioSource.Stop();
                _audioSource.clip = gameMusic;
                _audioSource.Play();
            }
        }
        
        if (gameState == GameState.Options)
        {
            if (!_optionsPanel.activeInHierarchy)
            {
                _hasDiedPanel.SetActive(false);
            }
            _optionsPanel.SetActive(true);
        }
        if (gameState == GameState.Dead)
        {
            if (!_hasDiedPanel.activeInHierarchy)
            {
                _optionsPanel.SetActive(false);
            }
            _hasDiedPanel.SetActive(true);
        }
        if (gameState == GameState.Play)
        {
            if (_optionsPanel.activeInHierarchy)
            {
                _optionsPanel.SetActive(false);
            }
            if (_hasDiedPanel.activeInHierarchy)
            {
                _hasDiedPanel.SetActive(false);
            }
        }

        var lockController = gameState == GameState.Options || gameState == GameState.Dead;

        if (lockController != _lockControllerState) // Change the state
        {
            if (lockController) // Lock the controls
            {
                PauseGame(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else // Unlock the controls
            {
                PauseGame(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            _lockControllerState = lockController;
        }
    }

    private void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        _gamePaused = pause;
    }

    public void ExitGame()
    {
        // TODO: Confirm exiting?
        Debug.Log("Application.Quit();");
        Application.Quit();
    }

    public void RespawnPlayer()
    {
        _player._currentHealth = _player.startingHealth;
        _player.Hit(0);
        _player.transform.position = _player._startingPosition;
        _player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        gameState = GameState.Play;
    }

    public bool HasBeenPlayed(string name)
    {
        if (_beenPlayed.Contains(name))
        {
            return true;
        }
        return false;
    }

    public void BeenPlayed(string name)
    {
        if (!HasBeenPlayed(name))
        {
            _beenPlayed.Add(name);
        }
    }

    public GameDifficulty GetDifficulty()
    {
        return _difficulty;
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        _difficulty = difficulty;
    }

    public bool IsGamePaused()
    {
        return _gamePaused;
    }

    public void FinishLevel(PlayerController playerController)
    {
        if (!_levelFinished && playerController != null)
        {
            playerController.FinishLevel();
        }
    }

    public int TargetFrameRate()
    {
        return _targetFrameRate;
    }
}
