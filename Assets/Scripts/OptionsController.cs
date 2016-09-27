using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class OptionsController : MenuController
{
    private Text _textQuality;
    private Text _textDifficulty;
    private Text _textExit;

    public string[] _qualitySettings;
    public List<GameManager.GameDifficulty> _difficulties = new List<GameManager.GameDifficulty> ();
    private string _confirmExit = "Press 'Fire' to confirm, or scroll to another choice";
    private string _initialExit;
    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        _qualitySettings = QualitySettings.names;
        foreach (GameManager.GameDifficulty difficulty in System.Enum.GetValues(typeof(GameManager.GameDifficulty)))
        {
            _difficulties.Add(difficulty);
        }

        foreach (var component in GetComponentsInChildren<Text>())
        {
            if (component.name == "Quality")
            {
                _textQuality = component;
            }
            if (component.name == "Difficulty")
            {
                _textDifficulty = component;
            }
            if (component.name == "Exit")
            {
                _textExit = component;
                _initialExit = _textExit.text;
            }
        }

        if (_textQuality == null)
        {
            throw new UnassignedReferenceException("Quality text object not found in " + name);
        }
        if (_textDifficulty == null)
        {
            throw new UnassignedReferenceException("Difficulty text object not found in " + name);
        }
        if (_textExit == null)
        {
            throw new UnassignedReferenceException("Exit text object not found in " + name);
        }

        DisplayValues();
    }

    protected override void Update()
    {
        base.Update();
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && _textExit.text == _confirmExit)
        {
            GameManager.Instance.ExitGame();
        }    
    }

    public override void ActionChanged()
    {
        _textExit.text = _initialExit;
    }

    public override void ActionUsed()
    {
        // May not be needed
    }

    public void ExitGame()
    {
        _textExit.text = _confirmExit;
    }

    public void CycleQualitySettings(int direction)
    {
        //var _qualitySettings = QualitySettings.names;
        var qualityIndex = 0;
        for (qualityIndex = 0; qualityIndex < _qualitySettings.Length; qualityIndex++)
        {
            if (QualitySettings.GetQualityLevel() == qualityIndex)
            {
                //if (qualityIndex == _qualitySettings.Length - 1)
                //{
                //    qualityIndex = -1; // If this is the last one, set it to -1 so that the "next" setting will be index = 0
                //}
                break;
            }

        }
        // Go in the indicated direction
        qualityIndex += direction;
        if (qualityIndex > _qualitySettings.Length - 1)
        {
            qualityIndex = 0;
        } else if (qualityIndex < 0)
        {
            qualityIndex = _qualitySettings.Length - 1;
        }

        QualitySettings.SetQualityLevel(qualityIndex, true);
        DisplayValues();
    }

    public void CycleDifficulty()
    {
        var selectedDifficulty = 0;
        for (var index = 0; index < _difficulties.Count; index++) {
            var difficulty = _difficulties[index];
            if (GameManager.Instance.GetDifficulty() == difficulty)
            {
                selectedDifficulty = index;
                break;
            }
        }
        selectedDifficulty++; // Move to the next one
        if (selectedDifficulty > _difficulties.Count - 1)
        {
            selectedDifficulty = 0;
        }
        ;
        GameManager.Instance.SetDifficulty((GameManager.GameDifficulty)selectedDifficulty);
        DisplayValues();
            
    }
    private void DisplayValues()
    {
        _textQuality.text = _qualitySettings[QualitySettings.GetQualityLevel()];
        _textDifficulty.text = GameManager.Instance.GetDifficulty().ToString();
    }
}
