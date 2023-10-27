using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public interface IState
{
    void EnterState(HunterNPC hunter);
    void ExitState(HunterNPC hunter);
    void UpdateState(HunterNPC hunter);
    void ExecuteStateBehavior(HunterNPC hunter);
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

    private IState currentStateObject;

    [SerializeField] private Material patrolMat;
    [SerializeField] private Material restMat;
    [SerializeField] private Material chaseMat;

    private Dictionary<string, IState> stateDictionary = new Dictionary<string, IState>();

    public float originalSpeed;
    public float originalPatrolSpeed;

    [SerializeField] private float RingSpawnFood;

    public List<Transform> agentsToChase = new List<Transform>();
    [SerializeField] private Renderer render;
    public Rigidbody rb;

    void Start()
    {
        stateDictionary.Add("Rest", new RestState());
        stateDictionary.Add("Patrol", new PatrolState(agentsToChase, rb));
        stateDictionary.Add("Chase", new ChaseState(agentsToChase, rb));

        SetState("Rest");
    }

    public void Update()
    {
        ChangeMaterial(); // ya no tiene mas getcomponent
        CheckAndAdjustPosition();
        ApplyObstacleAvoidance();
    }

    public void FixedUpdate() // esto maneja cositas de velocidad, creo
    {
        currentStateObject.UpdateState(this);
    }

    public void SetState(string newState)
    {
        if (currentStateObject != null)
        {
            currentStateObject.ExitState(this);
        }

        currentStateObject = stateDictionary[newState];

        currentStateObject.EnterState(this);
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
        currentStateObject.ExecuteStateBehavior(this);
    }

    public void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-RingSpawnFood, RingSpawnFood), 0f, Random.Range(-RingSpawnFood, RingSpawnFood));
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }

    public void ApplyObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, visionRadius, LayerMask.GetMask("Wall")))
        {
            Vector3 avoidanceDirection = ObstacleAvoidance(hit.point);
            rb.velocity = avoidanceDirection * speed;
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
        if (currentStateObject is PatrolState)
        {
            render.material = patrolMat;
        }
        else if (currentStateObject is RestState)
        {
            render.material = restMat;
        }
        else if (currentStateObject is ChaseState)
        {
            render.material = chaseMat;
        }
    }

    public Vector3 Pursuit(Vector3 agentPosition)
    {
        Vector3 direction = (agentPosition - transform.position).normalized;
        return direction;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}