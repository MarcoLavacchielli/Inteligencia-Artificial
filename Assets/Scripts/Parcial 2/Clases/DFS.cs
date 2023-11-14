using System;
using System.Collections.Generic;

public class DFS<T> : IPathfinder<T>
{
    public Func<T, T, bool> Satisfies;
    public Func<T, IEnumerable<T>> Neighbours;

    public List<T> FindPath(T start, T end, int limit = 200)
    {
        var pending = new Stack<T>();
        var path = new Dictionary<T, T>();
        pending.Push(start);

        while (pending.Count > 0)
        {
            limit--;
            if (limit <= 0) break;

            var node = pending.Pop();
            if (Satisfies(node, end))
                return Build(path, node);

            foreach (var next in Neighbours(node))
            {
                if (path.ContainsKey(next) || next.Equals(start))
                    continue;

                path[next] = node;
                pending.Push(next);
            }
        }

        return new List<T>();
    }

    public List<List<T>> FindPathSteps(T start, T end)
    {
        var steps = new List<List<T>>();

        var pending = new Stack<T>();
        var path = new Dictionary<T, T>();

        pending.Push(start);
        path.Add(start, default);

        while (pending.Count > 0)
        {
            var node = pending.Pop();
            steps.Add(Build(path, node));
            if (Satisfies(node, end))
                break;

            foreach (var child in Neighbours(node))
            {
                if (path.ContainsKey(child))
                    continue;

                path[child] = node;
                pending.Push(child);
            }
        }

        return steps;
    }

    static List<T> Build(Dictionary<T, T> path, T end)
    {
        var list = new List<T>();
        list.Add(end);
        while (path.TryGetValue(end, out T prev))
        {
            list.Add(prev);
            end = prev;
        }
        list.Reverse();
        return list;
    }
}