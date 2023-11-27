using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesicionAI : MonoBehaviour
{
    [SerializeField] ViewDetection detect;
    [SerializeField] Character character;
    [SerializeField] PhysicalNodeGrid grid;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Transform player;
    [SerializeField] Renderer render;
    [SerializeField] List<Node> path = new();
    int currentNodeIndex = 0;
    bool moving;
    MaterialPropertyBlock block;
    IDesicion tree;
    [SerializeField] Pathfinder pathfinder;
    private bool playerInSight = false;
    private Node lastKnownPlayerNode;
    public static List<DesicionAI> allGuardians = new List<DesicionAI>();

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
                OnFalse = new Leaf(ReturnToLastKnownPosition),
            },
            OnFalse = new Branch
            {
                Predicate = () => lastKnownPlayerNode != null,
                OnTrue = new Leaf(ReturnToLastKnownPosition),
                OnFalse = new Leaf(Patrol),
            },
        };

        block = new MaterialPropertyBlock { };

        allGuardians.Add(this);
    }

    private void ReturnToLastKnownPosition()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        // Asignar el current solo si aún no se ha asignado
        if (pathfinder.current == null)
        {
            pathfinder.current = grid.GetClosest(transform.position);
        }

        // Reiniciar el índice del nodo actual
        currentNodeIndex = 0;

        // Crear un nuevo camino desde el current al target en el Pathfinder
        pathfinder.target = lastKnownPlayerNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownPlayerNode);

        // Actualizar el nodo actual en el Pathfinder
        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        // Anular el movimiento propio del guardia
        character.velocity = Vector3.zero;

        // Iniciar la espera para llegar al destino o ver al jugador nuevamente
        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownPlayerNode);

        foreach (DesicionAI guard in allGuardians)
        {
            guard.lastKnownPlayerNode = lastKnownPlayerNode;
            guard.StartCoroutine(guard.FollowPathAndCheckForPlayer());
        }
    }

    private IEnumerator FollowPathAndCheckForPlayer()
    {
        int targetIndex = 0; // Índice del nodo actual en el camino

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            // Moverse hacia el nodo objetivo si no está lo suficientemente cerca
            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position);
                character.velocity = dir.normalized * moveSpeed;

                yield return null;
            }

            // Detener el movimiento una vez que está lo suficientemente cerca del nodo
            character.velocity = Vector3.zero;

            // Actualizar el nodo actual en el Pathfinder
            pathfinder.current = targetNode;

            // Incrementar el índice del nodo objetivo
            targetIndex++;

            if (targetIndex < pathfinder.path.Count)
            {
                // Cambiar el target al siguiente nodo en el camino
                pathfinder.target = pathfinder.path[targetIndex];
            }
            else
            {
                // Llegó al final del camino
                if (pathfinder.target == lastKnownPlayerNode)
                {
                    lastKnownPlayerNode = null;
                    Patrol();
                    yield break;
                }

                // Cambiar el target al primer waypoint
                pathfinder.target = path[0];

                // Generar un nuevo camino desde el current al nuevo target
                pathfinder.path = pathfinder.CallPathfind(pathfinder.target);

                // Si el nuevo camino no se ha generado correctamente, salir de la función
                if (pathfinder.path.Count == 0)
                {
                    yield break;
                }

                // Resetear el índice del nodo objetivo
                targetIndex = 0;
            }

            // Esperar un breve momento antes de pasar al siguiente nodo
            yield return new WaitForSeconds(0.1f);
        }

        // Una vez que ha recorrido todo el camino, anular el movimiento
        character.velocity = Vector3.zero;

        // Iniciar la espera para llegar al destino o ver al jugador nuevamente
        StartCoroutine(WaitForArrivalOrPlayerSight());
    }

    private IEnumerator WaitForArrivalOrPlayerSight()
    {
        yield return new WaitUntil(() => lastKnownPlayerNode != null && (Vector3.Distance(transform.position, lastKnownPlayerNode.transform.position) < 1f || detect.InFieldOfView(player.position)));

        if (!detect.InFieldOfView(player.position))
        {
            if (path != null && currentNodeIndex == path.Count - 1)
            {
                character.velocity = Vector3.zero;
                moving = false;

                if (!detect.InFieldOfView(player.position))
                {
                    lastKnownPlayerNode = null;
                    Patrol();
                    yield break;
                }
            }

            lastKnownPlayerNode = null;
            Patrol();
        }
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
                tree.Execute();
            }
            else
            {
                if (lastKnownPlayerNode != null)
                {
                    block.SetColor("_Color", Color.yellow);
                    render.SetPropertyBlock(block);
                    GoTo(lastKnownPlayerNode);
                }
                else
                {
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
            lastKnownPlayerNode = grid.GetClosest(player.position);
        }
    }

    private void Patrol()
    {
        block.SetColor("_Color", Color.blue);
        render.SetPropertyBlock(block);

        if (path != null && path.Count > 0)
        {
            Node targetNode = path[currentNodeIndex];
            Vector3 dir = (targetNode.transform.position - transform.position);

            if (Vector3.Distance(targetNode.transform.position, transform.position) < 1f)
            {
                character.velocity = Vector3.zero;
                pathfinder.current = targetNode;
                currentNodeIndex++;

                if (currentNodeIndex < path.Count)
                {
                    pathfinder.target = path[currentNodeIndex];
                }
                else
                {
                    currentNodeIndex = 0;
                    pathfinder.target = path[currentNodeIndex];
                    pathfinder.UpdateTarget(path[currentNodeIndex]);
                }
            }
            else
            {
                character.velocity = dir.normalized * moveSpeed;
            }
        }
        else
        {
            character.velocity = Vector3.zero;
        }
    }

    private void Chase()
    {
        Node previousTarget = pathfinder.target;
        pathfinder.current = null;
        pathfinder.target = null;
        pathfinder.path.Clear();
        lastKnownPlayerNode = grid.GetClosest(player.position);
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);
        var dir = player.position - transform.position;
        character.velocity = dir.normalized * moveSpeed;
    }

    public void GoTo(Node node)
    {
        if (moving) return;
        var start = grid.GetClosest(transform.position);
        var goal = node;
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