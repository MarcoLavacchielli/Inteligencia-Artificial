using UnityEngine;
using UnityEngine;

public class SeekingFoodState : AState
{
    public void EnterState(Agent agent)
    {
        // L�gica de entrada al estado de seeking food
    }

    public void ExitState(Agent agent)
    {
        // L�gica de salida del estado de seeking food
    }

    public void UpdateState(Agent agent)
    {
        agent.SeekFoodBehavior();
    }
}