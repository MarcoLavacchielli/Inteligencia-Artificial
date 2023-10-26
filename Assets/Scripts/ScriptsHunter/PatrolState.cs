using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{

    //
    private List<Transform> agentsToChase;
    private Rigidbody rb;

    public PatrolState(List<Transform> agentsToChase, Rigidbody rb)
    {
        this.agentsToChase = agentsToChase;
        this.rb = rb;
    }
    //

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

        foreach (Transform agent in agentsToChase)
        {
            float distanceToAgent = Vector3.Distance(hunter.transform.position, agent.transform.position);

            if (distanceToAgent < hunter.visionRadius)
            {
                hunter.SetState("Chase");
                return;
            }
        }

        Vector3 direction = (hunter.patrolWaypoints[hunter.currentWaypointIndex].position - hunter.transform.position).normalized;
        hunter.rb.velocity = direction * hunter.patrolSpeed;

        float distanceToWaypoint = Vector3.Distance(hunter.transform.position, hunter.patrolWaypoints[hunter.currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            hunter.currentWaypointIndex = (hunter.currentWaypointIndex + 1) % hunter.patrolWaypoints.Length;
        }
    }

    public void ExecuteStateBehavior(HunterNPC hunter)
    {
        // Implement patrol behavior if needed
    }
}