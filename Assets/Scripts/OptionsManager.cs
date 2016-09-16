using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour {

    private bool showQualitySettings = false;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void QualitySettings()
    {
        showQualitySettings = !showQualitySettings;
        print(showQualitySettings);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        // TODO: confirm exiting the game
        print("Exit");
        Application.Quit();
    }

}
