using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeTarget : MonoBehaviour
{
    [SerializeField]
    AgentProfe agent;

    [SerializeField]
    Transform target;

    private void Update()
    {
        agent.Accelerate(agent.Flee(target.position));
    }
}