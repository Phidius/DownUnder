using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class OptionsController : MonoBehaviour
{
    public bool showOptions = false;
    //public Vector2 layoutOffset;
    public GameObject panel;
    public Text quality;

    private bool _gvrViewer = false;
    private FirstPersonController _firstPerson;
    
    private string[] _qualitySettings;


    // Use this for initialization
    void Start ()
    {
        _qualitySettings = QualitySettings.names;
        _firstPerson = GameObject.FindObjectOfType<FirstPersonController>();
	    //panel = transform.Find("Panel").gameObject;

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
        
	    if (changeOptions != this.showOptions)
	    {
	        showOptions = changeOptions;
	        ShowOptions();
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

    //void OnGUI()
    //{
    //    if (panel.activeInHierarchy)
    //    {
    //        var viewPort = new UnityEngine.Rect(layoutOffset.x, layoutOffset.y, Screen.width - 2*layoutOffset.x, Screen.height - 2*layoutOffset.y);
    //        GUILayout.BeginArea(viewPort);
    //        GUILayout.BeginVertical();
    //        GUILayout.BeginHorizontal();
    //        GUILayout.Label("Current Settings: " + _qualitySettings[QualitySettings.GetQualityLevel()]);
            
    //        int i = 0;
    //        while (i < _qualitySettings.Length)
    //        {
    //            if (GUILayout.Button(_qualitySettings[i]))
    //                QualitySettings.SetQualityLevel(i, true);

    //            i++;
    //        }
    //        GUILayout.EndHorizontal();
            

    //        if (GUILayout.Button("Exit"))
    //        {
    //            ExitGame();
    //        }
    //        GUILayout.EndVertical();
    //        GUILayout.EndArea();


    //    }
    //}

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
}
