using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Atributos del agente
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float visionRadius = 5.0f;
    [SerializeField] private float arriveRadius = 2.0f;
    // Otros atributos seg�n necesidades

    // Estados del agente
    public enum AgentState { Flocking, SeekingFood, Evading };
    private AgentState currentState;
    private Transform target;
    private Transform foodTarget;

    void Start()
    {
        // Inicializar estado y otros atributos
        currentState = AgentState.Flocking;
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
        // L�gica para cambiar el estado seg�n condiciones
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
        // Ejecutar comportamiento seg�n el estado actual
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
                // Otros casos seg�n necesidades
        }
    }
    void SeekFoodBehavior()
    {
        // L�gica para buscar y dirigirse hacia la comida
        if (foodTarget != null)
        {
            Vector3 direction = (foodTarget.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            // Verificar si ha llegado a la comida
            float distanceToTarget = Vector3.Distance(transform.position, foodTarget.position);
            if (distanceToTarget < 1.0f)
            {
                // L�gica para cuando llega a la comida
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
        // L�gica para buscar comida en el escenario
        // Puedes usar raycasts, colisionadores, o alg�n otro m�todo
        // Asigna el objeto comida al atributo 'foodTarget'
        // Por ejemplo, puedes hacer algo como:
        foodTarget = GameObject.FindWithTag("Food").transform;
    }

    void FlockingBehavior()
    {
        // L�gica de flocking (Alignment, Cohesion, Separation)
        // L�gica de flocking (Alignment, Cohesion, Separation)
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
        // L�gica para calcular la alineaci�n con otros agentes
        // Devuelve la direcci�n promedio de los vecinos
        return Vector3.zero;  // Implementa tu l�gica aqu�
    }

    Vector3 CalculateCohesion()
    {
        // L�gica para calcular la cohesi�n con otros agentes
        // Devuelve la direcci�n hacia el centro de masa de los vecinos
        return Vector3.zero;  // Implementa tu l�gica aqu�
    }

    Vector3 CalculateSeparation()
    {
        // L�gica para calcular la separaci�n de otros agentes
        // Devuelve la direcci�n alej�ndose de los vecinos cercanos
        return Vector3.zero;  // Implementa tu l�gica aqu�
    }

    void EvadeBehavior()
    {
        // L�gica para aplicar evade cuando haya un NPC cazador cerca
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        foreach (GameObject hunter in hunters)
        {
            float distanceToHunter = Vector3.Distance(transform.position, hunter.transform.position);

            if (distanceToHunter < visionRadius)
            {
                // El cazador est� en el rango de visi�n, aplicar evade
                Vector3 evadeDirection = Evade(hunter.transform.position);
                transform.Translate(evadeDirection * speed * Time.deltaTime);
            }
        }
    }
    Vector3 Evade(Vector3 hunterPosition)
    {
        // L�gica para calcular la direcci�n de evade
        Vector3 direction = (transform.position - hunterPosition).normalized;
        return direction;
    }

    void ApplyObstacleAvoidance()
    {
        // L�gica para obstacle avoidance (steering behavior)
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, visionRadius))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                // Hay un obst�culo en frente, ajustar la direcci�n
                Vector3 avoidanceDirection = ObstacleAvoidance(hit.point);
                transform.Translate(avoidanceDirection * speed * Time.deltaTime);
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        // L�gica para calcular la direcci�n de evasi�n del obst�culo
        Vector3 avoidanceDirection = (transform.position - obstaclePosition).normalized;
        return avoidanceDirection;
    }

    void VisualizeBehavior()
    {
        // L�gica para visualizar el comportamiento en la escena
    }
}
