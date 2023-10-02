using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Atributos del agente
    public float speed = 5.0f;
    public float rotationSpeed = 2.0f;
    public float visionRadius = 5.0f;
    public float arriveRadius = 2.0f;
    // Otros atributos según necesidades

    // Estados del agente
    public enum AgentState { Flocking, SeekingFood, Evading };
    public AgentState currentState;
    public Transform target;
    public Transform foodTarget;

    void Start()
    {
        // Inicializar estado y otros atributos
        //currentState = AgentState.Flocking;
    }

    void Update()
    {
        UpdateState();
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void UpdateState()
    {
        // Lógica para cambiar el estado según condiciones
        if (foodTarget != null)
        {
            float distanceToFood = Vector3.Distance(transform.position, foodTarget.position);
            if (distanceToFood < arriveRadius)
            {
                // Cuando llega a la comida, cambia al estado de flocking
                currentState = AgentState.Flocking;
                foodTarget = null;
            }
        }
    }

    void ExecuteStateBehavior()
    {
        // Ejecutar comportamiento según el estado actual
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
                // Otros casos según necesidades
        }
    }
    void SeekFoodBehavior()
    {
        // Lógica para buscar y dirigirse hacia la comida
        if (foodTarget != null)
        {
            Vector3 direction = (foodTarget.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            // Verificar si ha llegado a la comida
            float distanceToTarget = Vector3.Distance(transform.position, foodTarget.position);
            if (distanceToTarget < 1.0f)
            {
                // Lógica para cuando llega a la comida
                Destroy(foodTarget.gameObject);  // O esconde la comida, dependiendo de tus necesidades
                currentState = AgentState.Flocking;  // Cambia al estado de flocking
                foodTarget = null;
            }
        }
        else
        {
            // Si no hay objetivo, buscar uno
            SearchForFood();
        }
    }

    void SearchForFood()
    {
        if (foodTarget == null)
        {
            // Lógica para buscar comida en el escenario
            // Asigna el objeto comida al atributo 'foodTarget'
            // Por ejemplo, puedes hacer algo como:
            GameObject foodObject = GameObject.FindWithTag("Food");
            if (foodObject != null)
            {
                foodTarget = foodObject.transform;
            }
        }
    }

    void FlockingBehavior()
    {
        // Lógica de flocking (Alignment, Cohesion, Separation)
        // Lógica de flocking (Alignment, Cohesion, Separation)
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();

        // Aplicar fuerzas de flocking
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
        // Lógica para aplicar evade cuando haya un NPC cazador cerca
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        foreach (GameObject hunter in hunters)
        {
            float distanceToHunter = Vector3.Distance(transform.position, hunter.transform.position);

            if (distanceToHunter < visionRadius)
            {
                // El cazador está en el rango de visión, aplicar evade
                Vector3 evadeDirection = Evade(hunter.transform.position);
                transform.Translate(evadeDirection * speed * Time.deltaTime);
            }
        }
    }
    Vector3 Evade(Vector3 hunterPosition)
    {
        // Lógica para calcular la dirección de evade
        Vector3 direction = (transform.position - hunterPosition).normalized;
        return direction;
    }

    void ApplyObstacleAvoidance()
    {
        // Lógica para obstacle avoidance (steering behavior)
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, visionRadius))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                // Hay un obstáculo en frente, ajustar la dirección
                Vector3 avoidanceDirection = ObstacleAvoidance(hit.point);
                transform.Translate(avoidanceDirection * speed * Time.deltaTime);
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        // Lógica para calcular la dirección de evasión del obstáculo
        Vector3 avoidanceDirection = (transform.position - obstaclePosition).normalized;
        return avoidanceDirection;
    }

    void VisualizeBehavior()
    {
        // Lógica para visualizar el comportamiento en la escena
        Debug.DrawRay(transform.position, transform.forward * visionRadius, Color.green);  // Visualizar rango de visión
    }
}
