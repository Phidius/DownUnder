using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject HUDisplay;

    // Use this for initialization
    void Start()
    {
        var displayHolder = Camera.main.transform.FindChild("DisplayHolder");
        HUDisplay = GameObject.Find("HUDisplay");
        
        HUDisplay.transform.parent = displayHolder.transform;

        HUDisplay.transform.localScale = new Vector3(1f, 1f, 1f);
        HUDisplay.transform.localPosition = new Vector3(0f, 0f, 0f);
        HUDisplay.transform.localRotation = Quaternion.identity;
    }
	



    
}
