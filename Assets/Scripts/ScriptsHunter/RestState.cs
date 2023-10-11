using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HunterNPC;

public class RestState : IState
{
    public void EnterState(HunterNPC hunter)
    {
        hunter.restTimer = 0.0f;
    }

    public void ExitState(HunterNPC hunter)
    {
        // Lógica de salida del estado de descanso
    }

    public void UpdateState(HunterNPC hunter)
    {
        hunter.restTimer += Time.deltaTime;
        if (hunter.restTimer >= hunter.restDuration)
        {
            hunter.restTimer = 0.0f;
            hunter.SetState(HunterNPC.HunterState.Patrol);
        }
    }
}
