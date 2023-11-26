using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField]
    public Node current, target;

    [SerializeField]
    PathAlgorithm algorithm;

    [SerializeField]
    float moveTime = 2f;

    [SerializeField]
    Vector3 offset = new(0, 1.5f, 0);

    [SerializeField]
    public List<Node> path = new();

    BFS<Vector2Int> bfs = new()
    {
        Satisfies = (a, b) => a == b,
        Neighbours = cell => new Vector2Int[]
        {
            cell + Vector2Int.up,
            cell + Vector2Int.down,
            cell + Vector2Int.left,
            cell + Vector2Int.right,
        },
    };

    private void Start()
    {
        if (target != null)
        {
            StartCoroutine(Pathfind(target));
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            Start();

    }

    public List<Node> CallPathfind(Node end)
    {
        return algorithm switch
        {
            PathAlgorithm.BFS => current.BFS(end),
            PathAlgorithm.DFS => current.DFS(end),
            _ => new List<Node>(),
        };
    }

    public IEnumerator Pathfind(Node end)
    {
        if (current == end)
            yield break;

        path = CallPathfind(end);
        if (path.Count == 0)
        {
            Debug.LogError("No se encontro Camino!");
            target = null;
            yield break;
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            yield return Move(path[i], path[i + 1]);
        }

        current = end;
        target = null;
    }

    IEnumerator Move(Node from, Node to)
    {
        for (float f = 0; f < 1; f += Time.deltaTime / moveTime)
        {
            yield return null;
            transform.position = Vector3.Lerp(
                from.transform.position, to.transform.position, f)
                + offset;
        }
    }
    public void UpdateTarget(Node newTarget)
    {
        target = newTarget;
    }
}