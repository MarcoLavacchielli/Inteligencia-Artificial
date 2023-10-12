using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingState : AState
{
    public void EnterState(Agent agent)
    {
        // L�gica de entrada al estado de evasi�n
    }

    public void ExitState(Agent agent)
    {
        // L�gica de salida del estado de evasi�n
    }

    public void UpdateState(Agent agent)
    {
        agent.EvadeBehavior();
    }
}