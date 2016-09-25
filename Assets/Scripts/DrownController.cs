using UnityEngine;
using System.Collections;

public class DrownController : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        var player = collider.GetComponent<PlayerController>();
        if (player)
        {
            GameManager.Instance.showHasDied = true;
        }
    }
}
