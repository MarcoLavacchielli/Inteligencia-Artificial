using UnityEngine;

public class FlockingState : AState
{
    public void EnterState(Agent agent)
    {
        // L�gica de entrada al estado de flocking
    }

    public void ExitState(Agent agent)
    {
        // L�gica de salida del estado de flocking
    }

    public void UpdateState(Agent agent)
    {
        agent.FlockingBehavior();
    }
}