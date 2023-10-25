using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAccelerate : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    private void Update()
    {
        var velocity = agent.Velocity;
        var desired = velocity.normalized * agent.MaxSpeed;
        var add = Vector3.ClampMagnitude(desired - velocity, .1f);
        agent.Accelerate(add);
    }
}
