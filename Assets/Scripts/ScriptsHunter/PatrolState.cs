using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private List<Transform> agentsToChase;
    private Rigidbody rb;

    public PatrolState(List<Transform> agentsToChase, Rigidbody rb)
    {
        this.agentsToChase = agentsToChase;
        this.rb = rb;
    }

    public void EnterState(HunterNPC hunter)
    {
        hunter.energy -= 5.0f * Time.deltaTime;
    }

    public void ExitState(HunterNPC hunter)
    {
    }

    public void UpdateState(HunterNPC hunter)
    {
        hunter.energy -= 10.0f * Time.deltaTime;

        if (hunter.energy <= 0)
        {
            hunter.SetState("Rest");
            hunter.energy = 100.0f;
            hunter.SpawnFood();
        }

        foreach (Transform agent in hunter.agentsToChase)
        {
            float distanceToAgent = Vector3.Distance(hunter.transform.position, agent.transform.position);

            if (distanceToAgent < hunter.visionRadius)
            {
                hunter.SetState("Chase");
                return;
            }
        }

        Vector3 avoidanceDirection = CalculateAvoidanceDirection(hunter);
        hunter.rb.velocity = avoidanceDirection * hunter.patrolSpeed;

        float distanceToWaypoint = Vector3.Distance(hunter.transform.position, hunter.patrolWaypoints[hunter.currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            hunter.currentWaypointIndex = (hunter.currentWaypointIndex + 1) % hunter.patrolWaypoints.Length;
        }
    }

    private Vector3 CalculateAvoidanceDirection(HunterNPC hunter)
    {
        RaycastHit hit;
        Vector3 direction = (hunter.patrolWaypoints[hunter.currentWaypointIndex].position - hunter.transform.position).normalized;

        int wallLayer = LayerMask.NameToLayer("Wall"); 

        if (Physics.Raycast(hunter.transform.position, direction, out hit, 3.0f, 1 << wallLayer))
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal);

            float smoothingFactor = 0.5f; // Ajustar el smooth para que no vaya muy pateado al girar
            return Vector3.Lerp(direction, reflectedDirection, smoothingFactor);
        }

        return direction;
    }

    public void ExecuteStateBehavior(HunterNPC hunter)
    {
    }
}