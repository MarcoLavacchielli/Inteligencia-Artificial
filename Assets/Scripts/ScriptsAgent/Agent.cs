using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AState
{
    void EnterState(Agent agent);
    void ExitState(Agent agent);
    void UpdateState(Agent agent);
}

public class Agent : MonoBehaviour
{
    public float speed = 5.0f;
    public float visionRadius = 5.0f;

    public enum AgentState { Flocking, SeekingFood, Evading };
    public AgentState currentState;
    public Transform foodTarget;
    public float evadeDuration = 2.0f;
    private float evadeTimer = 0.0f;

    [SerializeField] private Material flockingMat;
    [SerializeField] private Material seekingMat;
    [SerializeField] private Material evadingMat;

    private Dictionary<AgentState, AState> stateDictionary = new Dictionary<AgentState, AState>();
    private AState currentStateObject;

    void Start()
    {
        stateDictionary.Add(AgentState.Flocking, new FlockingState());
        stateDictionary.Add(AgentState.SeekingFood, new SeekingFoodState());
        stateDictionary.Add(AgentState.Evading, new EvadingState());

        SetAgentState(AgentState.Flocking);
    }

    void SetAgentState(AgentState newState)
    {
        if (currentStateObject != null)
        {
            currentStateObject.ExitState(this);
        }

        currentStateObject = stateDictionary[newState];
        currentState = newState;

        currentStateObject.EnterState(this);
    }

    void Update()
    {
        currentStateObject.UpdateState(this);
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        MaterialChanger();
        CheckAndAdjustPosition();
        UpdateState();
    }

    public void UpdateState()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        if (currentState != AgentState.Evading)
        {
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

        if (currentState == AgentState.Evading)
        {
            evadeTimer -= Time.deltaTime;
            if (evadeTimer <= 0.0f)
            {
                currentState = AgentState.Flocking;
            }
        }

        if (currentState == AgentState.Evading && hunters.Length == 0)
        {
            currentState = AgentState.Flocking;
        }

        if (currentState != AgentState.Evading && currentState != AgentState.SeekingFood && foodTarget == null)
        {
            GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
            if (foodObjects.Length > 0)
            {
                float minDistance = float.MaxValue;
                foreach (GameObject foodObject in foodObjects)
                {
                    float distanceToFood = Vector3.Distance(transform.position, foodObject.transform.position);
                    if (distanceToFood < minDistance)
                    {
                        minDistance = distanceToFood;
                        foodTarget = foodObject.transform;
                    }
                }

                currentState = AgentState.SeekingFood;
            }
        }
    }

    void CheckAndAdjustPosition()
    {
        Vector3 newPosition = TeleportBox.UpdatePosition(transform.position);
        if (newPosition != transform.position)
        {
            transform.position = newPosition;
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

    public void SeekFoodBehavior()
    {
        if (foodTarget != null)
        {
            Vector3 direction = (foodTarget.position - transform.position).normalized;
            GetComponent<Rigidbody>().velocity = direction * speed;

            float distanceToTarget = Vector3.Distance(transform.position, foodTarget.position);
            if (distanceToTarget < 1.0f)
            {
                Destroy(foodTarget.gameObject);
                SetAgentState(AgentState.Flocking);
                Agent[] allAgents = FindObjectsOfType<Agent>();
                foreach (Agent agent in allAgents)
                {
                    agent.SetAgentState(AgentState.Flocking);
                }

                //currentState = AgentState.Flocking;
                foodTarget = null;

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
        }else if(foodTarget== null && currentState == AgentState.SeekingFood)
        {
            SetAgentState(AgentState.Flocking);
            Agent[] allAgents = FindObjectsOfType<Agent>();
            foreach (Agent agent in allAgents)
            {
                agent.SetAgentState(AgentState.Flocking);
            }
        }
    }

    public void FlockingBehavior()
    {
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 separation = CalculateSeparation();

        Vector3 totalForce = alignment + cohesion + separation;
        totalForce = totalForce.normalized * speed;
        GetComponent<Rigidbody>().velocity = totalForce;

        if(foodTarget != null)
        {
            SetAgentState(AgentState.Flocking);
            Agent[] allAgents = FindObjectsOfType<Agent>();
            foreach (Agent agent in allAgents)
            {
                agent.SetAgentState(AgentState.SeekingFood);
            }
        }
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

    public void EvadeBehavior()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");

        foreach (GameObject hunter in hunters)
        {
            float distanceToHunter = Vector3.Distance(transform.position, hunter.transform.position);

            if (distanceToHunter < visionRadius)
            {
                Vector3 evadeDirection = Evade(hunter.transform.position);
                GetComponent<Rigidbody>().velocity = evadeDirection * speed;

                float distanceToAgent = Vector3.Distance(transform.position, hunter.transform.position);
                if (distanceToAgent < 1.0f)
                {
                    Destroy(gameObject);
                }
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
                GetComponent<Rigidbody>().velocity = avoidanceDirection * speed;
                break;
            }
        }
    }

    Vector3 ObstacleAvoidance(Vector3 obstaclePosition)
    {
        Vector3 toObstacle = obstaclePosition - transform.position;
        Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, toObstacle.normalized).normalized;

        return avoidanceDirection;
    }

    void MaterialChanger()
    {
        Renderer render = GetComponent<Renderer>();
        if (currentState == AgentState.Flocking)
        {
            render.material = flockingMat;
        }
        else if (currentState == AgentState.SeekingFood)
        {
            render.material = seekingMat;
        }
        else if (currentState == AgentState.Evading)
        {
            render.material = evadingMat;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}