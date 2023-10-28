using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    FlockingAgent[] agents;


    private void Awake()
    {
        agents = GetComponentsInChildren<FlockingAgent>();
    }

    void Update()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].Flock(agents, i);
            agents[i].transform.position =
                TeleportBox.UpdatePosition(agents[i].transform.position);
        }
    }
}
