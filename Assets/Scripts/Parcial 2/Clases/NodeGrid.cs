using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    [SerializeField, Range(2, 12)]
    int width = 4;
    [SerializeField, Range(2, 12)]
    int depth = 4;

    [SerializeField]
    Vector2Int start, end;

    BFS<Vector2Int> bfs;

    DFS<Vector2Int> dfs;

    List<Vector2Int> path_bfs = new();
    List<Vector2Int> path_dfs = new();

    private bool Satisfy(Vector2Int explorando, Vector2Int objetivo)
    {
        return explorando == objetivo;
    }

    IEnumerable<Vector2Int> Adjacentes(Vector2Int celda)
    {
        var posibles = new Vector2Int[]
        {
            celda + Vector2Int.up,
            celda + Vector2Int.down,
            celda + Vector2Int.left,
            celda + Vector2Int.right,
        };

        var list = new List<Vector2Int>();
        foreach (var pos in posibles)
        {
            if (Inside(pos))
                list.Add(pos);
        }
        return list;
    }

    bool Inside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width
            && cell.y >= 0 && cell.y < depth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(new Vector3(width * .5f, 0, depth * .5f),
            new Vector3(width, .1f, depth));

        Gizmos.color = Color.blue;

        for (int i = 0; i < path_bfs.Count - 1; i++)
        {
            var a = path_bfs[i];
            var b = path_bfs[i + 1];
            Gizmos.DrawLine(ToVec3(a), ToVec3(b));
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < path_dfs.Count - 1; i++)
        {
            var a = path_dfs[i];
            var b = path_dfs[i + 1];
            Gizmos.DrawLine(ToVec3(a), ToVec3(b));
        }
    }

    private void OnValidate()
    {
        bfs = new()
        {
            Satisfies = Satisfy,
            Neighbours = Adjacentes,
        };
        dfs = new()
        {
            Satisfies = Satisfy,
            Neighbours = Adjacentes,
        };

        path_bfs = bfs.FindPath(start, end, 100);
        path_dfs = dfs.FindPath(start, end, 100);
    }

    static Vector3 ToVec3(Vector2Int cell) => new Vector3(cell.x, 0, cell.y)
        + new Vector3(.5f, 0, .5f);

    private void Start()
    {
        OnValidate();
    }
}