using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{

    public List<Node> neighbours = new();

    [SerializeField]
    Color color;

    private void OnValidate()
    {
        //print("Cambie valor en: " + gameObject.name);
        if (TryGetComponent<MeshRenderer>(out var rend))
        {
            var block = new MaterialPropertyBlock();
            block.SetColor("_Color", color);
            rend.SetPropertyBlock(block);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        foreach (var node in neighbours)
        {
            var dir = (node.transform.position - transform.position);
            var add = .3f * Vector3.Cross(dir, Vector3.up).normalized;
            Gizmos.DrawLine(transform.position + add, node.transform.position + add);
        }
    }


    List<Node> Build(Dictionary<Node, Node> path, Node end)
    {
        var list = new List<Node>
        {
            end
        };
        Node current = end;
        while (path.TryGetValue(current, out var prev))
        {
            list.Add(prev);
            current = prev;
        }
        list.Reverse();
        return list;
    }

    public List<Node> BFS(Node target)
    {
        var pending = new Queue<Node>(); // Por Visitar
        var path = new Dictionary<Node, Node>(); // Camino
        pending.Enqueue(this);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            if (node == target)
                return Build(path, node);

            foreach (var next in node.neighbours)
            {
                // Ya visite este vecino ???
                if (path.ContainsKey(next) || next == this)
                    continue;

                path[next] = node;
                pending.Enqueue(next);
            }
        }

        return new List<Node>();
    }

    public List<Node> DFS(Node target)
    {
        var pending = new Stack<Node>();
        var path = new Dictionary<Node, Node>();
        pending.Push(this);

        while (pending.Count > 0)
        {
            var node = pending.Pop();
            if (node == target)
                return Build(path, node);

            foreach (var next in node.neighbours)
            {
                if (path.ContainsKey(next) || next == this)
                    continue;

                path[next] = node;
                pending.Push(next);
            }
        }

        return new List<Node>();
    }
}

public enum PathAlgorithm
{
    BFS, DFS,
}