using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Atributos del agente
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float visionRadius = 5.0f;
    // Otros atributos según necesidades

    // Estados del agente
    public enum AgentState { Flocking, SeekingFood, Evading };
    private AgentState currentState;

    // Objetivo actual del agente
    private Transform target;

    void Start()
    {
        // Inicializar estado y otros atributos
        currentState = AgentState.Flocking;
    }

    void Update()
    {
        // Actualizar el estado del agente
        UpdateState();

        // Ejecutar comportamiento según el estado
        ExecuteStateBehavior();
    }

    void UpdateState()
    {
        // Lógica para cambiar el estado según condiciones
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

        // Aplicar obstacle avoidance y visualización
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void FlockingBehavior()
    {
        // Lógica de flocking (Alignment, Cohesion, Separation)
    }

    void SeekFoodBehavior()
    {
        // Lógica para buscar y dirigirse hacia la comida
    }

    void EvadeBehavior()
    {
        // Lógica para aplicar evade cuando haya un NPC cazador cerca
    }

    void ApplyObstacleAvoidance()
    {
        // Lógica para obstacle avoidance (steering behavior)
    }

    void VisualizeBehavior()
    {
        // Lógica para visualizar el comportamiento en la escena
    }
}
