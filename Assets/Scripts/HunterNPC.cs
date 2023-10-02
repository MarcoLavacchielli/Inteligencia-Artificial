using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : MonoBehaviour
{
    // Atributos del NPC cazador
    [SerializeField] private float energy = 100.0f;
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
        // Actualizar el estado del NPC cazador
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

        // Aplicar obstacle avoidance y visualización
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void RestBehavior()
    {
        // Lógica para el estado de descanso
    }

    void PatrolBehavior()
    {
        // Lógica para el estado de patrulla
    }

    void ChaseBehavior()
    {
        // Lógica para el estado de persecución o disparo
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
