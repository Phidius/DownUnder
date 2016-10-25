using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

    private bool isLoadingNext = false;

	// Use this for initialization
	void Start ()
	{
	    var audioSource = GetComponent<AudioSource>();
        var volume = PlayerPrefsManager.GetMasterVolume();

        if (volume > 0)
        {
            audioSource.volume = volume;
        }
        print(PlayerPrefsManager.GetMasterMute());
	    audioSource.Play();
	    Invoke("LoadStart", 3.5f);
	}
	
	// Update is called once per frame
	void LoadStart () {
        if (!isLoadingNext)
        {
            isLoadingNext = true;
            SceneManager.LoadScene(1);
        }
	}
}
