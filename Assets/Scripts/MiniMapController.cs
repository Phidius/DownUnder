using UnityEngine;
using System.Collections;

public class MiniMapController : MonoBehaviour {

    public float height = 300f;

    private Transform _player;

	// Use this for initialization
	void Start () {
        _player = GameObject.FindObjectOfType<PlayerController>().transform;
	}
	
	// Update is called once per frame
	void Update () {
        var cameraPosition = _player.position;
        cameraPosition.y = height;
        transform.position = cameraPosition;
    }
}
