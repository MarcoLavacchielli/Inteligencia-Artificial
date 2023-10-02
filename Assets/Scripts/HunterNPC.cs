using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : MonoBehaviour
{
    // Atributos del NPC cazador
    [SerializeField] private float energy = 100.0f;
    [SerializeField] private float speed = 5.0f;  // Agrega la velocidad aquí
    [SerializeField] private float patrolSpeed = 3.0f;  // Velocidad durante la patrulla
    [SerializeField] private float visionRadius = 5.0f;
    // Otros atributos según necesidades

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

        // Ejecutar comportamiento según el estado
        ExecuteStateBehavior();
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void UpdateState()
    {
        // Lógica para cambiar el estado según condiciones
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

    void ExecuteStateBehavior()
    {
        // Ejecutar comportamiento según el estado actual
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
                // Otros casos según necesidades
        }
    }

    void RestBehavior()
    {
        // Lógica para el estado de descanso
        // Reduce la energía con el tiempo y, cuando llega a cero, vuelve a patrullar
        energy -= Time.deltaTime;

        if (energy <= 0)
        {
            currentState = HunterState.Patrol;
            energy = 100.0f;  // Reiniciar la energía al llegar a cero
        }
    }

    void PatrolBehavior()
    {
        // Lógica para el estado de patrulla
        transform.Translate(Vector3.forward * patrolSpeed * Time.deltaTime);
    }

    void ChaseBehavior()
    {
        // Lógica para el estado de persecución

        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        foreach (GameObject agent in agents)
        {
            float distanceToAgent = Vector3.Distance(transform.position, agent.transform.position);

            if (distanceToAgent < visionRadius)
            {
                // El agente está en el rango de visión, perseguir
                Vector3 chaseDirection = Pursuit(agent.transform.position);
                transform.Translate(chaseDirection * speed * Time.deltaTime);
            }
        }
    }
    Vector3 Pursuit(Vector3 agentPosition)
    {
        // Lógica para calcular la dirección de persecución (pursuit)
        Vector3 direction = (agentPosition - transform.position).normalized;
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
    }
}
