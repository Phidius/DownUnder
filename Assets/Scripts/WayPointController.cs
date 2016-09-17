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
        if (patrolable != null && patrolable.CurrentTarget().gameObject.tag != "Player")
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
            while (wayPoint == null && counter < 5)
            {
                // Try this 5 times... don't want to get stuck in an infinite loop!
                var index = Random.Range(0, waypoints.Length);
                if (patrolable.CurrentTarget() == null || waypoints[index].instanceId != patrolable.CurrentTarget().gameObject.GetInstanceID())
                {
                    wayPoint = waypoints[index];
                }

            }
            if (wayPoint == null)
            {
                // Don't worry about random - just set to the next consecutive waypoint.
                var currentIndex = 0; // First waypoint if not assigned one yet.
                if (patrolable.CurrentTarget())
                {
                    // Find the index of the current target
                    for (var index = 0; index < waypoints.Length; index++)
                    {
                        if (waypoints[index].instanceId == patrolable.CurrentTarget().gameObject.GetInstanceID())
                        {
                            currentIndex = index;
                        }
                    }
                }
                // Set to next waypoint
                if (currentIndex == waypoints.Length - 1)
                {
                    wayPoint = waypoints[0];
                }
                else
                {
                    wayPoint = waypoints[currentIndex + 1];
                }

            }
            patrolable.SetNextWaypoint(wayPoint);
            return wayPoint;
        }
        return null;
    }
}
