using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WayPointController : MonoBehaviour
{
    public int instanceId;
    
    // Use this for initialization

    void Start ()
	{
        instanceId = gameObject.GetInstanceID();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        var patrolable = (IPatrolable)collider.GetComponent(typeof (IPatrolable));
        if (patrolable != null)
        {
            SetWayPoint(patrolable);
        }
    }

    public static WayPointController SetWayPoint(IPatrolable patrolable)
    {
        var waypoints = GameObject.FindObjectsOfType<WayPointController>();
        if (waypoints != null && waypoints.Length > 0)
        {
            WayPointController wayPoint = null;
            var counter = 1;
            while (wayPoint == null && counter < 20)
            {
                // Try this 20 times... don't want to get stuck in an infinite loop!
                var index = Random.Range(0, waypoints.Length);
                if (patrolable.CurrentTarget() == null || waypoints[index].instanceId != patrolable.CurrentTarget().gameObject.GetInstanceID())
                {
                    wayPoint = waypoints[index];
                }

            }
            if (wayPoint == null)
            {
                // Don't worry about random - just set to the next consecutive waypoint.
                for (counter = 0; counter < waypoints.Length; counter++)
                {
                    var waypoint = waypoints[counter];
                    if (waypoint.instanceId == patrolable.CurrentTarget().gameObject.GetInstanceID())
                    {
                        // Set to next waypoint
                        if (counter == waypoints.Length - 1)
                        {
                            waypoint = waypoints[0];
                        }
                        else
                        {
                            waypoint = waypoints[counter + 1];
                        }
                    }
                }
            }
            patrolable.SetNextWaypoint(wayPoint);
            return wayPoint;
        }
        return null;
    }
}
