using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    float
        separationWeight = 1f,
        cohesionWeight = 1f,
        alignmentWeight = 1f;

    public void Flock(FlockingAgent[] agents, int self)
    {
        var separation = new Vector3();
        var averagePosition = new Vector3();
        var alignment = new Vector3();

        for (int i = 0; i < agents.Length; i++)
        {
            if (i == self)
                continue;

            float distance = Vector3.Distance(transform.position, agents[i].transform.position);

            separation += (transform.position - agents[i].transform.position).normalized
                * 1 / (distance * distance);
            averagePosition += agents[i].transform.position;

            alignment += agents[i].agent.Velocity;
        }

        //if (separation != Vector3.zero)
        //    separation = -separation.normalized * agent.MaxSpeed;

        averagePosition /= agents.Length - 1;
        alignment /= agents.Length - 1;

        Vector3 cohesion = agent.Seek(averagePosition);

        agent.Accelerate(
            separation * separationWeight
            + cohesion * cohesionWeight
            + alignment * alignmentWeight);
    }
}