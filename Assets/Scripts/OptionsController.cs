using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionsController : MenuController
{
    private Text _quality;
    private Text _difficulty;

    public string[] _qualitySettings;
    public List<GameManager.GameDifficulty> _difficulties = new List<GameManager.GameDifficulty> ();

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
                _quality = component;
            }
            if (component.name == "Difficulty")
            {
                _difficulty = component;
            }
        }

        DisplayValues();
    }

    public void CycleQualitySettings()
    {
        //var _qualitySettings = QualitySettings.names;
        var qualityIndex = 0;
        for (qualityIndex = 0; qualityIndex < _qualitySettings.Length; qualityIndex++)
        {
            if (QualitySettings.GetQualityLevel() == qualityIndex)
            {
                if (qualityIndex == _qualitySettings.Length - 1)
                {
                    qualityIndex = -1; // If this is the last one, set it to -1 so that the "next" setting will be index = 0
                }
                break;
            }

        }
        // Get the next one
        qualityIndex++;
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
        _quality.text = _qualitySettings[QualitySettings.GetQualityLevel()];
        _difficulty.text = GameManager.Instance.GetDifficulty().ToString();
    }
}
