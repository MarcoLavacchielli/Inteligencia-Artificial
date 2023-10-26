using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HunterNPC;

public class PatrolState : IState
{
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
            hunter.SetState(HunterNPC.HunterState.Rest);
            hunter.energy = 100.0f;
            hunter.SpawnFood();
        }

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(hunter.transform.position, agent.transform.position);

            if (distanceToAgent < hunter.visionRadius)
            {
                hunter.SetState(HunterNPC.HunterState.Chase);
                return;
            }
        }

        Vector3 direction = (hunter.patrolWaypoints[hunter.currentWaypointIndex].position - hunter.transform.position).normalized;
        hunter.GetComponent<Rigidbody>().velocity = direction * hunter.patrolSpeed;

        float distanceToWaypoint = Vector3.Distance(hunter.transform.position, hunter.patrolWaypoints[hunter.currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            hunter.currentWaypointIndex = (hunter.currentWaypointIndex + 1) % hunter.patrolWaypoints.Length;
        }
    }
}