using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : MonoBehaviour
{
    public GameObject foodPrefab;
    public float energy = 100.0f;
    public float speed = 5.0f;
    public float patrolSpeed = 3.0f;
    public float visionRadius = 5.0f;
    public Transform[] patrolWaypoints;
    public int currentWaypointIndex = 0;
    public float restTimer = 0.0f;
    public float restDuration = 5.0f;

    public enum HunterState { Rest, Patrol, Chase };
    public HunterState currentState;

    void Start()
    {
        currentState = HunterState.Rest;
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
        switch (currentState)
        {
            case HunterState.Rest:
                restTimer += Time.deltaTime;
                if (restTimer >= restDuration)
                {
                    restTimer = 0.0f;
                    currentState = HunterState.Patrol;
                }
                break;

            case HunterState.Patrol:
                energy -= 5.0f * Time.deltaTime;

                if (energy <= 0)
                {
                    currentState = HunterState.Rest;
                    energy = 100.0f;
                    SpawnFood();
                }

                GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
                foreach (GameObject agent in agents)
                {
                    float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

                    if (distanceToAgent < visionRadius)
                    {
                        currentState = HunterState.Chase;
                        break;  // Salir del bucle ya que ya estamos persiguiendo a uno
                    }
                }
                break;

            case HunterState.Chase:
                energy -= 10.0f * Time.deltaTime;

                if (energy <= 0)
                {
                    currentState = HunterState.Rest;
                    energy = 100.0f;
                    SpawnFood();
                }
                break;
        }
    }

    void ExecuteStateBehavior()
    {
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
        }
    }

    void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-100f, 100f), 0f, Random.Range(-100f, 100f));
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    void RestBehavior()
    {
        // Lógica de descanso...
    }

    void PatrolBehavior()
    {
        if (patrolWaypoints.Length == 0)
        {
            Debug.LogError("No se han asignado waypoints de patrulla al cazador.");
            return;
        }

        Vector3 direction = (patrolWaypoints[currentWaypointIndex].position - transform.position).normalized;
        transform.Translate(direction * patrolSpeed * Time.deltaTime);

        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    void ChaseBehavior()
    {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius)
            {
                Vector3 chaseDirection = Pursuit(agent.transform.position);
                transform.Translate(chaseDirection * speed * Time.deltaTime);
            }
        }
    }

    Vector3 Pursuit(Vector3 agentPosition)
    {
        Vector3 direction = (agentPosition - transform.position).normalized;
        return direction;
    }

    void ApplyObstacleAvoidance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, visionRadius))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Vector3 avoidanceDirection = ObstacleAvoidance(hit.point);
                transform.Translate(avoidanceDirection * speed * Time.deltaTime);
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        Vector3 avoidanceDirection = (transform.position - obstaclePosition).normalized;
        return avoidanceDirection;
    }

    void VisualizeBehavior()
    {
        Debug.DrawRay(transform.position, transform.forward * visionRadius, Color.red);
    }
}

