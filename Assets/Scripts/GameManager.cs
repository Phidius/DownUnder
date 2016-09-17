using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private bool _gvrViewer = false;
    // Use this for initialization
    void Start()
    {
        if (GvrViewer.Initialized)
        {
            _gvrViewer = true;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (_gvrViewer && GvrViewer.Instance.VRModeEnabled)
        {
            var triggered = GvrViewer.Instance.Triggered;
            if (triggered)
            {
                Application.Quit();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Application.Quit();
            }
        }
    }

    
}
