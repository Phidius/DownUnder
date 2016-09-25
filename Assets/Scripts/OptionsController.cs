using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MenuController
{
    private Text _quality;
    public string[] _qualitySettings;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        _qualitySettings = QualitySettings.names;

        foreach (var component in GetComponentsInChildren<Text>())
        {
            if (component.name == "Quality")
            {
                _quality = component;
            }
        }

        DisplayQualitySetting();
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
        DisplayQualitySetting();
    }

    private void DisplayQualitySetting()
    {
        _quality.text = _qualitySettings[QualitySettings.GetQualityLevel()];
        
    }
}
