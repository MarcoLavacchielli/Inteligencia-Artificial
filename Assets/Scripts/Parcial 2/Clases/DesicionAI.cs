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
    [SerializeField] List<Node> path = new List<Node>();
    int currentNodeIndex = 0;
    bool moving;
    MaterialPropertyBlock block;
    IDecision tree;
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

        if (pathfinder.current == null)
        {
            pathfinder.current = grid.GetClosest(transform.position);
        }

        currentNodeIndex = 0;

        pathfinder.target = lastKnownPlayerNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownPlayerNode);

        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        character.velocity = Vector3.zero;

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
        int targetIndex = 0;

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position);
                character.velocity = dir.normalized * moveSpeed;

                yield return null;
            }

            character.velocity = Vector3.zero;

            pathfinder.current = targetNode;

            targetIndex++;

            if (targetIndex < pathfinder.path.Count)
            {
                pathfinder.target = pathfinder.path[targetIndex];
            }
            else
            {
                if (pathfinder.target == lastKnownPlayerNode)
                {
                    lastKnownPlayerNode = null;
                    Patrol();
                    yield break;
                }

                pathfinder.target = path[0];

                pathfinder.path = pathfinder.CallPathfind(pathfinder.target);

                if (pathfinder.path.Count == 0)
                {
                    yield break;
                }

                targetIndex = 0;
            }

            yield return new WaitForSeconds(0.1f);
        }

        character.velocity = Vector3.zero;
    }

    private void Start()
    {
        StartCoroutine(StartAI());
    }

    private IEnumerator StartAI()
    {
        moving = false;
        yield return new WaitUntil(() => path != null && path.Count > 0);
        moving = true;
        yield return new WaitForSeconds(0.1f);

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
        path = start.AStar(goal);
        path.Reverse();
    }

    private void Attack()
    {
        block.SetColor("_Color", Color.red);
        render.SetPropertyBlock(block);
        character.velocity = Vector3.zero;
    }
}

interface IDecision
{
    void Execute();
}

class Branch : IDecision
{
    public Func<bool> Predicate;
    public IDecision OnTrue, OnFalse;

    public void Execute()
    {
        if (Predicate())
            OnTrue?.Execute();
        else
            OnFalse?.Execute();
    }
}

class Leaf : IDecision
{
    public Action Action;

    public Leaf(Action action)
    {
        Action = action;
    }

    public void Execute()
    {
        Action?.Invoke();
    }
}