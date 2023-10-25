using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    readonly static List<Boid> boids = new();

    [SerializeField]
    Agent agent;

    [SerializeField]
    float
        separationWeight = 1f,
        cohesionWeight = 1f,
        alignmentWeight = 1f,
        globalWeight = .1f;

    [SerializeField]
    float radius = 8f;

    private void Awake()
    {
        boids.Add(this);
    }

    private void Update()
    {
        var separation = new Vector3();
        var averagePosition = new Vector3();
        var alignment = new Vector3();

        int count = 0;

        foreach (var boid in boids)
        {
            if (!boid || boid == this ||
                Vector3.Distance(boid.transform.position, transform.position) > radius)
                continue;

            float distance = Vector3.Distance(transform.position, boid.transform.position);

            separation += (transform.position - boid.transform.position).normalized
                * 1 / (distance * distance);
            averagePosition += boid.transform.position;

            alignment += boid.agent.Velocity;
            count++;
        }

        if (count == 0)
            return;

        averagePosition /= count;
        alignment /= count;

        Vector3 cohesion = agent.Seek(averagePosition);

        agent.Accelerate(globalWeight * separationWeight * separation);
        agent.Accelerate(cohesionWeight * globalWeight * cohesion);
        agent.Accelerate(alignmentWeight * globalWeight * alignment);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
