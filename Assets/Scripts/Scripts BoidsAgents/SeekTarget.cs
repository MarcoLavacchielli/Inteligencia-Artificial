using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekTarget : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    Transform target;

    private void Update()
    {
        agent.Accelerate(agent.Seek(target.position));
    }
}