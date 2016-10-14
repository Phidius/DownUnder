using UnityEngine;
using System.Collections;

public class ZoomController : MonoBehaviour {

    private Transform _firstPerspective;
    private Transform _thirdPerspective;

    private PlayerController player;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        _firstPerspective = transform.root.FindChild("First");
        _thirdPerspective = transform.root.FindChild("Third");
    }

    // Update is called once per frame
    void Update ()
	{
	    var cameraTransform = transform;

        var zoomCamera = Input.GetAxis("ZoomCamera");
        if (zoomCamera != 0)
        {
            var cameraPosition = cameraTransform.localPosition; // Camera.main.transform.localPosition;
            cameraPosition.z += zoomCamera;
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, -10f, 0f);

            //z = 0 is the furthest forward... z = -1 is the limit to avoid seeing the avatar mesh
            if (cameraPosition.z > -1f && cameraTransform.parent.gameObject.name == "Third")
            {
                cameraTransform.parent = _firstPerspective; // Camera.main.transform.parent = _firstPerspective;
                cameraPosition.z = 0.0f;
            }
            else if (cameraPosition.z < 0f && cameraTransform.parent.gameObject.name == "First")
            {
                cameraTransform.parent = _thirdPerspective; //Camera.main.transform.parent = _thirdPerspective;
                cameraPosition.z = -1.1f;
            }
            cameraTransform.localPosition = cameraPosition; //Camera.main.transform.localPosition = cameraPosition;
        }

        //if (cameraTransform.parent == _thirdPerspective)
        //{
        //    var rotateCamera = Input.GetAxis("RotateController");
        //    if (rotateCamera != 0f)
        //    {
        //        _firstPersonController.rotationOffset += (rotateCamera*3);
        //    }
        //}
        var rotateCamera = Input.GetAxis("RotateController");
        if (rotateCamera != 0f)
        {
            player._firstPersonController.rotationOffset += (rotateCamera * 3);
        }
    }
}
