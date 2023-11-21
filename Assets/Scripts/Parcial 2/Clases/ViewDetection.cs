using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDetection : MonoBehaviour
{
    [SerializeField]
    float viewRadius = 10f;

    [SerializeField]
    float senseRadius = 1.5f;

    [SerializeField]
    float viewAngle = 90f;

    [SerializeField]
    LayerMask wallMask;

    [SerializeField]
    PhysicalNodeGrid nodeGrid;

    private void Update()
    {
        // Tu lógica de detección y comportamiento aquí
    }

    public bool InFieldOfView(Vector3 point)
    {
        var dir = point - transform.position;
        if (dir.magnitude > viewRadius)
            return false;

        if (dir.magnitude < senseRadius)
            return true;

        var angle = Vector3.Angle(dir, transform.forward);
        return angle <= viewAngle / 2;
    }

    public bool InLineOfSight(Vector3 point)
    {
        if (!InFieldOfView(point))
            return false;

        var dir = point - transform.position;
        return !Physics.Raycast(transform.position, dir, dir.magnitude, wallMask);
    }

    public Node GetClosestNodeInView()
    {
        foreach (var node in nodeGrid.AllNodes)
        {
            if (InFieldOfView(node.transform.position) && InLineOfSight(node.transform.position))
            {
                return node;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, senseRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        var vector = transform.forward * viewRadius;

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, viewAngle / 2, 0) * vector);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -viewAngle / 2, 0) * vector);
    }
}