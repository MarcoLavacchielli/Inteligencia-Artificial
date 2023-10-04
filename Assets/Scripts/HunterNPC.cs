using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    //materiales//
    [SerializeField] private Material patrolMat;
    [SerializeField] private Material restMat;
    [SerializeField] private Material chaseMat;

    void Start()
    {
        currentState = HunterState.Rest;
    }

    void Update()
    {
        UpdateState();
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        ChangeMaterial();
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                Vector3 avoidanceDirection = ObstacleAvoidance(collider.transform.position);
                transform.Translate(avoidanceDirection * speed * Time.deltaTime);
                break; // Detenerse después de evitar el primer obstáculo
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        Vector3 toObstacle = obstaclePosition - transform.position;
        Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, toObstacle.normalized).normalized;

        return avoidanceDirection;
    }

    void ChangeMaterial()
    {
        Renderer render = GetComponent<Renderer>();
        if (currentState == HunterState.Patrol)
        {
            render.material = patrolMat;
        }
        else if (currentState == HunterState.Rest)
        {
            render.material = restMat;
        }
        else if (currentState == HunterState.Chase)
        {
            render.material = chaseMat;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}

