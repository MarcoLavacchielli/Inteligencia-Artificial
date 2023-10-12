using UnityEngine;

public class FlockingState : AState
{
    public void EnterState(Agent agent)
    {
        // Lógica de entrada al estado de flocking
    }

    public void ExitState(Agent agent)
    {
        // Lógica de salida del estado de flocking
    }

    public void UpdateState(Agent agent)
    {
        agent.FlockingBehavior();
    }
}