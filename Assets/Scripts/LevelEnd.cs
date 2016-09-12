using UnityEngine;
using System.Collections;

public class LevelEnd : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        var playerController = collider.GetComponent <PlayerController>();

        if (playerController)
        {
            playerController.FinishLevel();
        }

    }
}
