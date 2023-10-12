using UnityEngine;
using UnityEngine;

public class SeekingFoodState : AState
{
    public void EnterState(Agent agent)
    {
        // Lógica de entrada al estado de seeking food
    }

    public void ExitState(Agent agent)
    {
        // Lógica de salida del estado de seeking food
    }

    public void UpdateState(Agent agent)
    {
        agent.SeekFoodBehavior();
    }
}