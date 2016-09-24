using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class OptionsController : MonoBehaviour
{
    public bool showOptions = false;
    
    private Text _quality;

    private bool _gvrViewer = false;
    private FirstPersonController _firstPerson;
    private Button[] _actions;
    public string[] _qualitySettings;
    private int _currentAction = 0;
    private bool _verticalInUse = false;

    // Use this for initialization
    void Start ()
    {
        _qualitySettings = QualitySettings.names;

        _firstPerson = GameObject.FindObjectOfType<FirstPersonController>();
        
	    if (GvrViewer.Initialized)
	    {
	        print("GvrViewier is initialzed");
	        _gvrViewer = true;
	    }

        foreach (var component in GetComponentsInChildren<Text>())
        {
            if (component.name == "Quality")
            {
                _quality = component;
            }
        }
    }

    public void SetActive(bool active)
    {
        SetActive(active);
    }

    public bool GetActive()
    {
        return GetActive();
    }
	// Update is called once per frame
	void Update ()
	{
        var changeOptions = showOptions;

        if (showOptions)
        {
            if (_firstPerson != null)
            {
                _firstPerson.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            _actions = GetComponentsInChildren<Button>();
            HighlightAction();
        }
        else
        {
            if (_firstPerson != null)
            {
                _firstPerson.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

        if (CrossPlatformInputManager.GetAxisRaw("Vertical") != 0)
        {
            if (_verticalInUse == false)
            {
                _verticalInUse = true;
                _currentAction = _currentAction + 1;
                if (_currentAction > _actions.Length - 1)
                {
                    _currentAction = 0;
                }
                HighlightAction();
            }
        }
        if (CrossPlatformInputManager.GetAxisRaw("Vertical") == 0)
        {
            _verticalInUse = false;
        }
        if (CrossPlatformInputManager.GetButtonDown("Submit"))
        {
            UseAction();
        }
    }

    public static void CycleQualitySettings()
    {
        var _qualitySettings = QualitySettings.names;
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
    }

    private void DisplayQualitySetting()
    {
        _quality.text = _qualitySettings[QualitySettings.GetQualityLevel()];
        
    }

    private void HighlightAction()
    {
        if (_actions.Length > _currentAction)
        {
            _actions[_currentAction].Select();
        }

    }

    private void UseAction()
    {
        if (_actions.Length > _currentAction)
        {
            _actions[_currentAction].onClick.Invoke();
        }

    }
}
