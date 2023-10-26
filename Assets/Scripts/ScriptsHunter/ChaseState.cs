using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HunterNPC;

public class ChaseState : IState
{
    public void EnterState(HunterNPC hunter)
    {
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
                Vector3 chaseDirection = hunter.Pursuit(agent.transform.position);
                hunter.GetComponent<Rigidbody>().velocity = chaseDirection * hunter.speed;
            }
        }
    }
}