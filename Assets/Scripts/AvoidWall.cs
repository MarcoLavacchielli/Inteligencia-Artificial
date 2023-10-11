using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidWall : MonoBehaviour
{
    /*[SerializeField]
    Agent agent;

    [SerializeField]
    float maxDistance = 5f, radius = 1f, weight = 2f;

    [SerializeField]
    LayerMask layerMask;

    void Update()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward,
            out var hit, maxDistance, layerMask))
        {
            var G = Vector3.Project(hit.point - transform.position, transform.forward) + transform.position;
            var steering = G - hit.point;

            Debug.DrawRay(hit.point, steering, Color.magenta);

            agent.Accelerate(steering);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }*/
}
