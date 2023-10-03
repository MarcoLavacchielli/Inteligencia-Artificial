using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float speed = 5.0f;
    public float visionRadius = 5.0f;

    public enum AgentState { Flocking, SeekingFood, Evading };
    public AgentState currentState;
    public Transform foodTarget;
    public float evadeDuration = 2.0f;
    private float evadeTimer = 0.0f;

    void Update()
    {
        UpdateState();
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        VisualizeBehavior();

        GameObject[] objetosDeComida = GameObject.FindGameObjectsWithTag("Food");

        if (objetosDeComida.Length > 0 && currentState != Agent.AgentState.Evading)
        {
            // Se encontr� al menos un objeto con el tag "Food"
            //Debug.Log("�Se detect� comida en la escena!");
            currentState = AgentState.SeekingFood;
        }
        else if ( currentState != Agent.AgentState.Evading)
        {
            currentState = AgentState.Flocking;
        }
    }

    void UpdateState()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        if (currentState != AgentState.Evading)
        {
            // Cambio de Flocking a Evading si hay un cazador
            foreach (GameObject hunter in hunters)
            {
                float distanceToHunter = Vector3.Distance(transform.position, hunter.transform.position);
                if (distanceToHunter < visionRadius)
                {
                    currentState = AgentState.Evading;
                    evadeTimer = evadeDuration;
                    break;
                }
            }
        }

        // Si est� evadiendo, verificar si ha pasado el tiempo de evasi�n
        if (currentState == AgentState.Evading)
        {
            evadeTimer -= Time.deltaTime;
            if (evadeTimer <= 0.0f)
            {
                currentState = AgentState.Flocking;
            }
        }

        // Cambio de Evading a Flocking si ya no hay cazadores en el rango
        if (currentState == AgentState.Evading && hunters.Length == 0)
        {
            currentState = AgentState.Flocking;
        }

        // Cambio de Flocking a SeekingFood si hay comida en el mapa
        if (currentState != AgentState.Evading && currentState != AgentState.SeekingFood && foodTarget == null)
        {
            GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
            if (foodObjects.Length > 0)
            {
                foodTarget = foodObjects[Random.Range(0, foodObjects.Length)].transform;
                currentState = AgentState.SeekingFood;
            }
        }
    }

    void ExecuteStateBehavior()
    {
        switch (currentState)
        {
            case AgentState.Flocking:
                FlockingBehavior();
                break;
            case AgentState.SeekingFood:
                SeekFoodBehavior();
                break;
            case AgentState.Evading:
                EvadeBehavior();
                break;
        }
    }

    void SeekFoodBehavior()
    {
        if (foodTarget != null)
        {
            Vector3 direction = (foodTarget.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(transform.position, foodTarget.position);
            if (distanceToTarget < 1.0f)
            {
                Destroy(foodTarget.gameObject);
                foodTarget = null;

                // Verifica si hay m�s alimentos despu�s de destruir la comida actual
                GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
                if (foodObjects.Length > 0)
                {
                    foodTarget = foodObjects[Random.Range(0, foodObjects.Length)].transform;
                    currentState = AgentState.SeekingFood;
                }
                else
                {
                    currentState = AgentState.Flocking;
                }
            }
        }
    }

    void FlockingBehavior()
    {
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();

        Vector3 totalForce = alignment + cohesion + separation;
        totalForce = totalForce.normalized * speed;

        transform.Translate(totalForce * Time.deltaTime);
    }

    Vector3 CalculateAlignment()
    {
        Vector3 averageAlignment = Vector3.zero;
        int neighborCount = 0;

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius && agent != this.gameObject)
            {
                averageAlignment += agent.transform.forward;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            averageAlignment /= neighborCount;
            averageAlignment.Normalize();
        }

        return averageAlignment;
    }

    Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int neighborCount = 0;

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius && agent != this.gameObject)
            {
                centerOfMass += agent.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            centerOfMass /= neighborCount;
            return Seek(centerOfMass);
        }

        return Vector3.zero;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius && agent != this.gameObject)
            {
                separation += (transform.position - agent.transform.position) / distanceToAgent;
            }
        }

        return separation.normalized;
    }

    Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredDirection = (targetPosition - transform.position).normalized;
        return desiredDirection;
    }

    void EvadeBehavior()
    {
        // Logic for applying evade when there is a hunter nearby
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        foreach (GameObject hunter in hunters)
        {
            float distanceToHunter = Vector3.Distance(transform.position, hunter.transform.position);

            if (distanceToHunter < visionRadius)
            {
                Vector3 evadeDirection = Evade(hunter.transform.position);
                transform.Translate(evadeDirection * speed * Time.deltaTime);
            }
        }
    }

    Vector3 Evade(Vector3 hunterPosition)
    {
        Vector3 direction = (transform.position - hunterPosition).normalized;
        return direction;
    }

    void ApplyObstacleAvoidance()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                Vector3 avoidanceDirection = ObstacleAvoidance(collider.transform.position);
                transform.Translate(avoidanceDirection * speed * Time.deltaTime);
                break; // Detenerse despu�s de evitar el primer obst�culo
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        Vector3 toObstacle = obstaclePosition - transform.position;
        Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, toObstacle.normalized).normalized;

        return avoidanceDirection;
    }

    void VisualizeBehavior()
    {
        //Debug.DrawSphere(transform.position, visionRadius, Color.green);
    }
}