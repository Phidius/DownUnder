using UnityEngine;
using System.Collections;

public class SpiderSense : MonoBehaviour
{

    private SpiderController _spider;

    void Start()
    {
        _spider = GetComponentInParent<SpiderController>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (_spider.GetState() == SpiderController.SpiderState.Idle)
            {
                _spider.Spawn(collider.transform);
            }
            else
            {
                _spider.SetTarget(collider.transform);
            }
        }
    }
}
