using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{

    //
    private List<Transform> agentsToChase;
    private Rigidbody rb;

    public ChaseState(List<Transform> agentsToChase, Rigidbody rb)
    {
        this.agentsToChase = agentsToChase;
        this.rb = rb;
    }
    //

    public void EnterState(HunterNPC hunter)
    {
    }

    public void ExitState(HunterNPC hunter)
    {
    }

    public void UpdateState(HunterNPC hunter)
    {
        agentsToChase.RemoveAll(item => item == null); // esto elimina a los agentes de la lista para evitar ese error mugroso

        hunter.energy -= 10.0f * Time.deltaTime;

        if (hunter.energy <= 0)
        {
            hunter.SetState("Rest");
            hunter.energy = 100.0f;
            hunter.SpawnFood();
        }

        foreach (Transform agent in agentsToChase)
        {
            float distanceToAgent = Vector3.Distance(hunter.transform.position, agent.transform.position);

            if (distanceToAgent < hunter.visionRadius)
            {
                Vector3 chaseDirection = hunter.Pursuit(agent.transform.position);
                hunter.rb.velocity = chaseDirection * hunter.speed;

                // Verificar si hay colisión con el agente
                if (distanceToAgent < 1.0f) // Ajusta este valor según sea necesario
                {
                    // Destruir el agente al colisionar
                    Object.Destroy(agent.gameObject);
                }
            }
        }
    }

    public void ExecuteStateBehavior(HunterNPC hunter)
    {
    }
}