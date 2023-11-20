using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class Node : MonoBehaviour
{

    public List<Node> neighbours = new();

    public Vector3 WorldPosition => transform.position;

    [SerializeField]
    Color color;

    [SerializeField]
    LayerMask wallMask;

    public bool Blocked;

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

    private void Start()
    {
        OnValidate();
    }

    public void SetColor(Color color)
    {
        this.color = color;
        OnValidate();
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

    public float CostTo(Node other)
    {
        return Vector3.Distance(transform.position, other.transform.position);
    }

    public List<Node> Dijkstra(Node target)
    {
        var pending = new PriorityQueueMin<Node>();
        pending.Enqueue(this, 1f);

        var path = new Dictionary<Node, Node>(); // Camino
        var costs = new Dictionary<Node, float>();
        costs.Add(this, 0f);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            if (node == target)
                return Build(path, target);

            foreach (var next in node.neighbours)
            {
                if (next.Blocked) continue;

                if (next == this)
                    continue;

                float cost = costs[node] + CostTo(next);

                if (!costs.ContainsKey(next))
                {
                    costs.Add(next, cost);
                    pending.Enqueue(next, cost);
                    path.Add(next, node);
                }
                else if (cost < costs[node])
                {
                    pending.Enqueue(next, cost);
                    costs[node] = cost;
                    path[next] = node;
                }
            }
        }

        return new List<Node>();
    }

    public float Heuristic(Node target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public List<Node> GreedyBFS(Node target)
    {
        var pending = new PriorityQueueMin<Node>(); // Por Visitar
        var path = new Dictionary<Node, Node>(); // Camino
        pending.Enqueue(this, 0f);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            if (node == target)
                return Build(path, node);

            foreach (var next in node.neighbours)
            {
                if (next.Blocked) continue;

                // Ya visite este vecino ???
                if (path.ContainsKey(next) || next == this)
                    continue;

                path[next] = node;
                pending.Enqueue(next, next.Heuristic(target));
            }
        }

        return new List<Node>();
    }

    public List<Node> AStar(Node target)
    {
        var pending = new PriorityQueueMin<Node>();
        pending.Enqueue(this, 1f);

        var path = new Dictionary<Node, Node>(); // Camino
        var costs = new Dictionary<Node, float>();
        costs.Add(this, 0f);

        while (pending.Count > 0)
        {
            var node = pending.Dequeue();

            if (node == target)
                return Build(path, node);

            foreach (var next in node.neighbours)
            {
                if (next == this)
                    continue;

                float cost = costs[node] + CostTo(next);

                if (!costs.ContainsKey(next))
                {
                    costs.Add(next, cost);
                    pending.Enqueue(next, cost + next.Heuristic(target));
                    path.Add(next, node);
                }
                else if (cost < costs[node])
                {
                    pending.Enqueue(next, cost + next.Heuristic(target));
                    costs[node] = cost;
                    path[next] = node;
                }
            }
        }

        return new List<Node>();
    }

    public bool Skip(Node from, Node to)
    {
        var dir = (to.transform.position - from.transform.position);
        return !Physics.Raycast(
            from.transform.position, dir,
            dir.magnitude, wallMask
            );
    }

    public List<Node> ThetaStar(Node target)
    {
        var path = AStar(target);

        if (path.Count == 0) { return path; }

        int current = 0;

        while (current + 2 < path.Count)
        {
            if (Skip(path[current], path[current + 2]))
                path.RemoveAt(current + 1);
            else
                current++;
        }

        return path;
    }
}


class PriorityQueueMin<T>
{
    List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        if (elements.Count == 0 || priority < elements[^1].priority)
        {
            elements.Add((item, priority));
            return;
        }
        int index = elements.Count - 1;

        while (index > 0)
        {
            if (priority < elements[index - 1].priority)
            {
                break;
            }
            index--;
        }
        elements.Insert(index, (item, priority));
    }

    public T Dequeue()
    {
        var elem = elements[^1].item;
        elements.RemoveAt(elements.Count - 1);
        return elem;
    }
}

class PriorityQueueMax<T>
{
    List<(T item, float priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        if (elements.Count == 0 || priority > elements[^1].priority)
        {
            elements.Add((item, priority));
            return;
        }
        int index = elements.Count - 1;

        while (index > 0)
        {
            if (priority > elements[index - 1].priority)
            {
                break;
            }
            index--;
        }
        elements.Insert(index, (item, priority));
    }

    public T Dequeue()
    {
        var elem = elements[^1].item;
        elements.RemoveAt(elements.Count - 1);
        return elem;
    }
}

public enum PathAlgorithm
{
    BFS, DFS, Dijkstra,
}