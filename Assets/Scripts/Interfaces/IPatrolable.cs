using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public interface IPatrolable
{

    void SetNextWaypoint(WayPointController waypoint);
    Transform CurrentTarget();
    GameObject GetGameObject();
}
