using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingState : AState
{
    public void EnterState(Agent agent)
    {
        // Lógica de entrada al estado de evasión
    }

    public void ExitState(Agent agent)
    {
        // Lógica de salida del estado de evasión
    }

    public void UpdateState(Agent agent)
    {
        agent.EvadeBehavior();
    }
}