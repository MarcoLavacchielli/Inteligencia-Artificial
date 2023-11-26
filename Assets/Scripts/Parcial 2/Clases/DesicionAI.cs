using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DesicionAI : MonoBehaviour
{
    [SerializeField]
    ViewDetection detect;
    [SerializeField]
    Character character;
    [SerializeField]
    PhysicalNodeGrid grid;

    [SerializeField]
    float moveSpeed = 4f;

    [SerializeField]
    Transform player;

    [SerializeField]
    Renderer render;

    [SerializeField]
    List<Node> path = new();

    int currentNodeIndex = 0;

    bool moving;

    MaterialPropertyBlock block;

    IDesicion tree;

    float alertLevel;

    [SerializeField]
    Pathfinder pathfinder;

    private bool playerInSight = false;
    private Vector3 lastKnownPlayerPosition;

    private void Awake()
    {
        tree = new Branch
        {
            Predicate = () => detect.InFieldOfView(player.position),
            OnTrue = new Branch
            {
                Predicate = () => detect.InLineOfSight(player.position),
                OnTrue = new Branch
                {
                    Predicate = () => Vector3.Distance(transform.position, player.position) < 3f,
                    OnTrue = new Leaf(Attack),
                    OnFalse = new Leaf(Chase),
                },
                OnFalse = new Leaf(Seek),
            },
            OnFalse = new Branch
            {
                Predicate = () => lastKnownPlayerPosition != Vector3.zero, // Agrega la condición para verificar si hay una última posición conocida
                OnTrue = new Leaf(ReturnToLastKnownPosition), // Agrega una nueva hoja para volver a la última posición conocida
                OnFalse = new Leaf(Patrol),
            },
        };

        //Tree nuevo del Profe
        /*tree = new Branch
        {
            Predicate = () => detect.InLineOfSight(player.position),
            OnTrue = new Branch
            {
                Predicate = () => Vector3.Distance(transform.position, player.position) < 3f,
                OnTrue = new Branch
                {
                    OnTrue = new Leaf(Attack),
                    OnFalse = new Leaf(Chase),
                },
                OnFalse = new Branch
                {
                    Predicate = () => alertLevel > .4f,
                    OnTrue = new Leaf(Investigate),
                    OnFalse = new Leaf(Patrol),
                }
            },
        };*/
        block = new MaterialPropertyBlock { };
    }

    private void ReturnToLastKnownPosition()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        GoTo(lastKnownPlayerPosition);

        // Espera hasta llegar a la posición o hasta que el jugador esté a la vista
        StartCoroutine(WaitForArrivalOrPlayerSight());
    }

    private IEnumerator WaitForArrivalOrPlayerSight()
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, lastKnownPlayerPosition) < 1f || detect.InFieldOfView(player.position));

        // Verifica si el jugador está a la vista después de llegar a la posición
        if (!detect.InFieldOfView(player.position))
        {
            // Jugador no encontrado, reanudar la patrulla
            lastKnownPlayerPosition = Vector3.zero;
            Patrol();
        }
    }
    void GetOthers()
    {

    }

    IEnumerator Start()
    {
        moving = false;
        yield return new WaitUntil(() => path != null && path.Count > 0);
        moving = true;

        yield return new WaitForSeconds(.1f);

        while (true)
        {
            if (playerInSight)
            {
                // Player is in sight, continue with the existing logic
                tree.Execute();
            }
            else
            {
                // Player is not in sight, move to the last known position or patrol
                if (lastKnownPlayerPosition != Vector3.zero)
                {
                    // Move to the last known position of the player
                    block.SetColor("_Color", Color.yellow);
                    render.SetPropertyBlock(block);
                    GoTo(lastKnownPlayerPosition);
                }
                else
                {
                    // Player not found, resume patrolling
                    Patrol();
                }
            }

            yield return null;
        }
    }

    private void Update()
    {
        tree.Execute();

        playerInSight = detect.InFieldOfView(player.position);

        if (playerInSight)
        {
            // Update the last known position of the player
            lastKnownPlayerPosition = player.position;
        }
    }

    private void Patrol()
    {
        block.SetColor("_Color", Color.blue);
        render.SetPropertyBlock(block);

        // Verifica si hay nodos en la ruta de patrulla
        if (path != null && path.Count > 0)
        {
            // Establece el destino como el nodo actual en la ruta de patrulla
            Node targetNode = path[currentNodeIndex];

            // Calcula la dirección hacia el nodo
            Vector3 dir = (targetNode.transform.position - transform.position);

            // Actualiza la velocidad del personaje hacia el nodo
            character.velocity = dir.normalized * moveSpeed;

            // Si el personaje llega lo suficientemente cerca al nodo actual, pasa al siguiente nodo
            if (Vector3.Distance(targetNode.transform.position, transform.position) < 1f)
            {
                // Actualiza el current al nodo actual
                pathfinder.current = targetNode;

                // Incrementa el índice del nodo actual
                currentNodeIndex++;

                // Si hay más nodos en la ruta, actualiza el target al siguiente nodo en DesicionAI
                if (currentNodeIndex < path.Count)
                {
                    pathfinder.target = path[currentNodeIndex];
                }
                else
                {
                    // Si ya no hay más nodos en la ruta, reinicia la patrulla
                    currentNodeIndex = 0;
                    pathfinder.target = path[currentNodeIndex];
                }
            }
        }
        else
        {
            character.velocity = Vector3.zero;  // si no hay waypoints
        }
    }

    private void Seek()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        GoTo(lastKnownPlayerPosition);
    }

    private void Chase()
    {
        //alertLevel += Time.deltaTime / 3;
        //alertLevel = Mathf.Clamp01(alertLevel);

        // Guarda el nodo actual antes de borrarlo
        Node previousTarget = pathfinder.target;

        // Elimina el current y el target del Pathfinder y vacía la lista de path
        pathfinder.current = null;
        pathfinder.target = null;
        pathfinder.path.Clear();

        // Continúa con la lógica de Chase usando previousTarget según sea necesario
        lastKnownPlayerPosition = player.position;
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);

        var dir = player.position - transform.position;
        character.velocity = dir.normalized * moveSpeed;

    }

    public void GoTo(Vector3 position)
    {
        if (moving) return;

        var start = grid.GetClosest(transform.position);
        var goal = grid.GetClosest(position);
        if (!start || !goal || start == goal) return;

        path = start.ThetaStar(goal);
        path.Reverse();
    }

    private void Attack()
    {
        block.SetColor("_Color", Color.red);
        render.SetPropertyBlock(block);

        character.velocity = Vector3.zero;
    }

    private void Investigate()
    {
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);

        lastKnownPlayerPosition = Vector3.zero;

        // Si esta muy cerca bajar el nivel de alerta
        //alertLevel -= Time.deltaTime;
        //alertLevel = Mathf.Clamp01(alertLevel);
    }
}

interface IDesicion
{
    void Execute();
}

class Branch : IDesicion
{
    public Func<bool> Predicate;
    public IDesicion OnTrue, OnFalse;

    public void Execute()
    {
        if (Predicate())
            OnTrue.Execute();
        else
            OnFalse.Execute();
    }
}

class Leaf : IDesicion
{
    public Action Action;

    public Leaf(Action action)
    {
        Action = action;
    }

    public void Execute()
    {
        Action();
    }
}