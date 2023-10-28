using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestState : IState
{
    public void EnterState(HunterNPC hunter)
    {
        hunter.originalSpeed = hunter.speed;
        hunter.originalPatrolSpeed = hunter.patrolSpeed;
        hunter.restTimer = 0.0f;
        hunter.speed = 0f;
        hunter.patrolSpeed = 0f;
    }

    public void ExitState(HunterNPC hunter)
    {
        hunter.speed = hunter.originalSpeed;
        hunter.patrolSpeed = hunter.originalPatrolSpeed;
    }

    public void UpdateState(HunterNPC hunter)
    {
        hunter.restTimer += Time.deltaTime;
        if (hunter.restTimer >= hunter.restDuration)
        {
            hunter.restTimer = 0.0f;
            hunter.SetState("Patrol");
        }
    }

    public void ExecuteStateBehavior(HunterNPC hunter)
    {
        // Implement rest behavior if needed
    }
}