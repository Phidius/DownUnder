using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class OptionsController : MonoBehaviour
{
    public bool showOptions = false;
    public GameObject panel;
    public Text quality;

    private bool _gvrViewer = false;
    private FirstPersonController _firstPerson;
    private Button[] _actions;
    private string[] _qualitySettings;
    private int _currentAction = 0;
    private bool _verticalInUse = false;

    // Use this for initialization
    void Start ()
    {
        _qualitySettings = QualitySettings.names;
        _firstPerson = GameObject.FindObjectOfType<FirstPersonController>();
        _actions = GetComponentsInChildren<Button>();
        HighlightAction();

	    if (GvrViewer.Initialized)
	    {
	        _gvrViewer = true;
	    }
        DisplayQualitySetting();
        ShowOptions();
	}
	
	// Update is called once per frame
	void Update ()
	{
        var changeOptions = showOptions;
	    if (_gvrViewer && GvrViewer.Instance.VRModeEnabled)
	    {
	        var triggered = GvrViewer.Instance.Triggered;
            if (triggered)
            {
                changeOptions = !showOptions;
            }
        }
	    else
	    {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                changeOptions = !showOptions;
            }
        }
        
	    if (changeOptions != showOptions)
	    {
	        showOptions = changeOptions;
	        ShowOptions();
	    }

        if (showOptions)
        {
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
    }

    private void ShowOptions()
    {
        if (showOptions)
        {
            _firstPerson.enabled = false;

            // Place the canvas directly in front of the camera at a depth of .45
            var point = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, .45f));
            transform.position = point;
            //transform.rotation = _firstPerson.transform.rotation;
            transform.rotation = Camera.main.transform.rotation;

            panel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            panel.SetActive(false);
            _firstPerson.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ExitGame()
    {
        // TODO: Confirm exiting?
        Application.Quit();
    }

    public void CycleQualitySettings()
    {
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
        quality.text = _qualitySettings[QualitySettings.GetQualityLevel()];
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
