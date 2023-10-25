using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidWall : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    SharedCast cast;

    void Update()
    {
        if (cast.IsHit)
        {
            var hit = cast.Hit;
            var G = Vector3.Project(hit.point - transform.position, transform.forward) + transform.position;
            var steering = G - hit.point;

            Debug.DrawRay(hit.point, steering, Color.magenta);

            agent.Accelerate(steering);
        }
    }
}