using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeTarget : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    Transform target;

    private void Update()
    {
        agent.Accelerate(agent.Flee(target.position));
    }
}