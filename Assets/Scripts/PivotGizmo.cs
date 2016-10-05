using UnityEngine;
using System.Collections;

public class PivotGizmo : MonoBehaviour
{

    public float gizmoSize = .75f;
    public Color gizmoColor = Color.yellow;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
