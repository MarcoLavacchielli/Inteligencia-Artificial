using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveToTarget : MonoBehaviour
{
    [SerializeField]
    AgentProfe agent;

    [SerializeField]
    Transform target;

    [SerializeField, Min(.1f)]
    float arriveRadius = 10f;


    private void Update()
    {
        agent.Accelerate(agent.Arrive(target.position, arriveRadius));
    }

    private void OnDrawGizmosSelected()
    {
        if (!target) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(target.position, arriveRadius);
    }
}