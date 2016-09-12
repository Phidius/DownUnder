using UnityEngine;
using System.Collections;

public class EndFight : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // Activate all idle spiders
            foreach (var spider in GameObject.FindObjectsOfType<SpiderController>())
            {
                if (spider.GetState() == SpiderController.SpiderState.Idle)
                {
                    spider.Spawn(collider.transform);
                }
            }
        }
    }
}
