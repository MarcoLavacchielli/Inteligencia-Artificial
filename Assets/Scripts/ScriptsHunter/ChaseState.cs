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
        agentsToChase.RemoveAll(item => item == null);

        hunter.energy -= 10.0f * Time.deltaTime;

        if (hunter.energy <= 0)
        {
            hunter.SetState("Rest");
            hunter.energy = 100.0f;
            hunter.SpawnFood();
        }

        bool foundAgent = false;

        foreach (Transform agent in agentsToChase)
        {
            float distanceToAgent = Vector3.Distance(hunter.transform.position, agent.transform.position);

            if (distanceToAgent < hunter.visionRadius)
            {
                foundAgent = true;

                Vector3 chaseDirection = hunter.Pursuit(agent.transform.position);

                float smoothingFactor = 0.5f;
                Vector3 smoothedChaseDirection = Vector3.Lerp(hunter.rb.velocity.normalized, chaseDirection, smoothingFactor);

                Vector3 avoidanceDirection = CalculateAvoidanceDirection(hunter, smoothedChaseDirection);

                hunter.rb.velocity = avoidanceDirection * hunter.speed;

                // Verificar si hay colisión con el agente
                if (distanceToAgent < 1.0f)
                {
                    // Destruir el agente al colisionar
                    Object.Destroy(agent.gameObject);
                }
            }
        }

        // Si no se encontró ningún agente, salir del estado de persecución
        if (!foundAgent)
        {
            hunter.SetState("Patrol");
        }
    }

    private Vector3 CalculateAvoidanceDirection(HunterNPC hunter, Vector3 desiredDirection)
    {
        RaycastHit hit;
        Vector3 direction = desiredDirection.normalized;

        int wallLayer = LayerMask.NameToLayer("Wall");

        if (Physics.Raycast(hunter.transform.position, direction, out hit, 3.0f, 1 << wallLayer))
        {
            Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal);

            float smoothingFactor = 0.5f; // Ajustar el smooth para que no vaya muy pateado al girar
            return Vector3.Lerp(direction, reflectedDirection, smoothingFactor);
        }

        return direction;
    }

    public void ExecuteStateBehavior(HunterNPC hunter)
    {
    }
}