using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public interface IState
{
    public void EnterState(HunterNPC hunter);
    public void ExitState(HunterNPC hunter);
    public void UpdateState(HunterNPC hunter);
}
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

    [SerializeField] private Material patrolMat;
    [SerializeField] private Material restMat;
    [SerializeField] private Material chaseMat;

    private IState currentStateObject;

    private Dictionary<HunterState, IState> stateDictionary = new Dictionary<HunterState, IState>();

    public float originalSpeed;
    public float originalPatrolSpeed;

    [SerializeField] private float RingSpawnFood;

    void Start()
    {
        stateDictionary.Add(HunterState.Rest, new RestState());
        stateDictionary.Add(HunterState.Patrol, new PatrolState());
        stateDictionary.Add(HunterState.Chase, new ChaseState());

        SetState(HunterState.Rest);
    }

    public void Update()
    {
        currentStateObject.UpdateState(this);
        ApplyObstacleAvoidance();
        ChangeMaterial();
        CheckAndAdjustPosition();
    }
    public void SetState(HunterState newState)
    {
        if (currentStateObject != null)
        {
            currentStateObject.ExitState(this);
        }

        currentStateObject = stateDictionary[newState];
        currentState = newState;

        currentStateObject.EnterState(this);
    }

    public void UpdateState()
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

    public void CheckAndAdjustPosition()
    {
        Vector3 newPosition = TeleportBox.UpdatePosition(transform.position);
        if (newPosition != transform.position)
        {
            transform.position = newPosition;
        }
    }

    public void ExecuteStateBehavior()
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

    public void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-RingSpawnFood, RingSpawnFood), 0f, Random.Range(-RingSpawnFood, RingSpawnFood));
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    public void RestBehavior()
    {
        //
    }

    public void PatrolBehavior()
    {
        if (patrolWaypoints.Length == 0)
        {
            Debug.LogError("No se han asignado waypoints de patrulla al cazador.");
            return;
        }

        Vector3 direction = (patrolWaypoints[currentWaypointIndex].position - transform.position).normalized;
        GetComponent<Rigidbody>().velocity = direction * patrolSpeed;

        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint < 1.0f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    public void ChaseBehavior()
    {
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        bool isChasing = false;  // Variable para verificar si estamos persiguiendo activamente a un agente

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius)
            {
                Vector3 chaseDirection = Pursuit(agent.transform.position);
                GetComponent<Rigidbody>().velocity = chaseDirection * speed;

                isChasing = true;  // Estamos persiguiendo activamente a un agente
                break;
            }
        }

        // Si no estamos persiguiendo a nadie, establecemos la velocidad en cero
        if (!isChasing)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public Vector3 Pursuit(Vector3 agentPosition)
    {
        Vector3 direction = (agentPosition - transform.position).normalized;
        return direction;
    }

    public void ApplyObstacleAvoidance()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                Vector3 avoidanceDirection = ObstacleAvoidance(collider.transform.position);
                GetComponent<Rigidbody>().velocity = avoidanceDirection * speed;
                break;
            }
        }
    }

    public Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        Vector3 toObstacle = obstaclePosition - transform.position;
        Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, toObstacle.normalized).normalized;

        return avoidanceDirection;
    }

    public void ChangeMaterial()
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}

