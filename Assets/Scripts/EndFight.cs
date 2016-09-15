using UnityEngine;
using System.Collections;

public class EndFight : MonoBehaviour
{

    public bool hasBegun = false;
    public Transform target = null;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            hasBegun = true;
            target = collider.transform;
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
