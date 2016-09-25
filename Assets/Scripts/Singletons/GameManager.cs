﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class GameManager : MonoBehaviour
{
    public bool showOptions = false;
    public bool showHasDied = false;

    private GameObject HUDisplay;
    private PlayerController _player;
    private GameObject _optionsPanel;
    private GameObject _hasDiedPanel;
    private bool _lockControllerState = false;

    private static GameManager _gameManager;
    public static GameManager Instance {  get { return _gameManager; } }

    private void Awake()
    {
        if (_gameManager != null && _gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameManager = this;
        }
    }
    // Use this for initialization
    void Start()
    {
        var displayHolder = Camera.main.transform.FindChild("DisplayHolder");
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

        HUDisplay.transform.parent = displayHolder.transform;

        HUDisplay.transform.localScale = new Vector3(1f, 1f, 1f);
        HUDisplay.transform.localPosition = new Vector3(0f, 0f, 0f);
        HUDisplay.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        
        if (CrossPlatformInputManager.GetButtonDown("Options"))
        {
            showOptions = !showOptions;
        }

        if (showHasDied)
        {
            showOptions = false;
        }
        
        if (showOptions != _optionsPanel.activeInHierarchy)
        {
            _optionsPanel.SetActive(showOptions);
        }
        if (showHasDied != _hasDiedPanel.activeInHierarchy)
        {
            _hasDiedPanel.SetActive(showHasDied);
        }

        var lockController = showOptions || showHasDied;

        if (lockController != _lockControllerState) // Change the state
        {
            if (lockController) // Lock the controls
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else // Unlock the controls
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            _lockControllerState = lockController;
        }

        
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
        if (showHasDied)
        {
            showHasDied = false;
        }
    }

}