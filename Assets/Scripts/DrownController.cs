using UnityEngine;
using System.Collections;

public class DrownController : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        var player = collider.GetComponent<PlayerController>();
        if (player)
        {
            player.Reset(PlayerController.PlayerState.Drowned);
        }
    }
}
