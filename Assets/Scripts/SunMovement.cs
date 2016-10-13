using UnityEngine;
using System.Collections;

public class SunMovement : MonoBehaviour {
    [Tooltip("Set a value for the number of minutes per second that pass, try 60")]
    public float minutesPerSecond; // Number of minutes per second

    private Light sunLight;

    void Start()
    {
        sunLight = GetComponent<Light>();
    }
	// Update is called once per frame
	void Update () {
        var angleThisFrame = Time.deltaTime / 360 * minutesPerSecond;

        transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);
	    //print(transform.rotation.x);
	    sunLight.intensity = 2f * Mathf.Abs(transform.rotation.x);
	    
	}
}
