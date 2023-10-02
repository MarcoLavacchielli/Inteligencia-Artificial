using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : MonoBehaviour
{
    public GameObject foodPrefab;
    // Atributos del NPC cazador
    [SerializeField] private float energy = 100.0f;
    [SerializeField] private float speed = 5.0f;  // Agrega la velocidad aqu�
    [SerializeField] private float patrolSpeed = 3.0f;  // Velocidad durante la patrulla
    [SerializeField] private float visionRadius = 5.0f;
    [SerializeField] private Transform[] patrolWaypoints;
    private int currentWaypointIndex = 0;
    private float restTimer = 0.0f;
    [SerializeField] private float restDuration = 5.0f;
    // Otros atributos seg�n necesidades

    // Estados del NPC cazador
    public enum HunterState { Rest, Patrol, Chase };
    private HunterState currentState;

    // Objetivo actual del NPC cazador
    private Transform target;

    void Start()
    {
        // Inicializar estado y otros atributos
        currentState = HunterState.Rest;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        // Actualizar el estado del NPC cazador
        UpdateState();

        // Ejecutar comportamiento seg�n el estado
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void UpdateState()
    {
        // L�gica para cambiar el estado seg�n condiciones
        restTimer += Time.deltaTime;

        if (restTimer >= restDuration)
        {
            currentState = HunterState.Rest;
            restTimer = 0.0f;  // Reiniciar el temporizador al cambiar al estado de descanso
        }
        else
        {
            GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

            foreach (GameObject agent in agents)
            {
                float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

                if (distanceToAgent < visionRadius)
                {
                    currentState = HunterState.Chase;
                    return;
                }
            }
        }
    }

    void ExecuteStateBehavior()
    {
        // Ejecutar comportamiento seg�n el estado actual
        switch (currentState)
        {
            case HunterState.Rest:
                RestBehavior();
                break;
            case HunterState.Patrol:
                PatrolBehavior();
                break;
            case HunterState.Chase:
                ChaseBehavior();
                break;
                // Otros casos seg�n necesidades
        }
    }
    void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    void RestBehavior()
    {
        // L�gica de descanso...

        if (energy <= 0)
        {
            currentState = HunterState.Patrol;
            energy = 100.0f;
            SpawnFood(); // Llama al m�todo para spawnear comida
        }
    }

    void PatrolBehavior()
    {
        // L�gica para el estado de patrulla
        if (patrolWaypoints.Length == 0)
        {
            Debug.LogError("No se han asignado waypoints de patrulla al cazador.");
            return;
        }

        // Moverse hacia el waypoint actual
        Vector3 direction = (patrolWaypoints[currentWaypointIndex].position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // Verificar si ha llegado al waypoint actual
        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            // Cambiar al siguiente waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    void ChaseBehavior()
    {
        // L�gica para el estado de persecuci�n

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius)
            {
                // El agente est� en el rango de visi�n, perseguir
                Vector3 chaseDirection = Pursuit(agent.transform.position);
                transform.Translate(chaseDirection * speed * Time.deltaTime);
            }
        }
    }
    Vector3 Pursuit(Vector3 agentPosition)
    {
        // L�gica para calcular la direcci�n de persecuci�n (pursuit)
        Vector3 direction = (agentPosition - transform.position).normalized;
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
        Debug.DrawRay(transform.position, transform.forward * visionRadius, Color.red);  // Visualizar rango de visi�n
    }
}
