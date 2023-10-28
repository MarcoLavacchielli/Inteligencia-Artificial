using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitTarget : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    Agent target;

    public void Update()
    {
        agent.Accelerate(agent.Pursuit(target));
    }

}