using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetecteAndChase : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    ViewDetection detection;

    [SerializeField]
    Transform chase;

    private void Update()
    {
        if (detection.InLineOfSight(chase.position))
            agent.Accelerate((chase.position - transform.position).normalized * 2f);
    }
}
