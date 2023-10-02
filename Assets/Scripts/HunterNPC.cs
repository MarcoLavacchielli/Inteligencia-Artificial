using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : MonoBehaviour
{
    // Atributos del NPC cazador
    [SerializeField] private float energy = 100.0f;
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
        // Actualizar el estado del NPC cazador
        UpdateState();

        // Ejecutar comportamiento seg�n el estado
        ExecuteStateBehavior();
    }

    void UpdateState()
    {
        // L�gica para cambiar el estado seg�n condiciones
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

        // Aplicar obstacle avoidance y visualizaci�n
        ApplyObstacleAvoidance();
        VisualizeBehavior();
    }

    void RestBehavior()
    {
        // L�gica para el estado de descanso
    }

    void PatrolBehavior()
    {
        // L�gica para el estado de patrulla
    }

    void ChaseBehavior()
    {
        // L�gica para el estado de persecuci�n o disparo
    }

    void ApplyObstacleAvoidance()
    {
        // L�gica para obstacle avoidance (steering behavior)
    }

    void VisualizeBehavior()
    {
        // L�gica para visualizar el comportamiento en la escena
    }
}
