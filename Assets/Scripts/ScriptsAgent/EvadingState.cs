using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadingState : AState
{
    public void EnterState(Agent agent)
    {
    }

    public void ExitState(Agent agent)
    {
    }

    public void UpdateState(Agent agent)
    {
        agent.EvadeBehavior();
    }
}