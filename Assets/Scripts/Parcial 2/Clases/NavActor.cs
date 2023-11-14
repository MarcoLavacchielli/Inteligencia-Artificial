using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NavActor : MonoBehaviour
{

    [SerializeField]
    PhysicalNodeGrid grid;

    [SerializeField]
    float moveTime = 2f;

    [SerializeField]
    Vector3 offset = new(0, 1.5f, 0);

    static WaitForSeconds time = new WaitForSeconds(.01f);



    Node GetCurrent()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out var hit, 9999f))
            return null;

        return hit.collider.GetComponent<Node>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);

        var start = GetCurrent();
        var goal = grid.GetRandom();

        if (!start)
            print("No Current");
        if (!goal)
            print("No Goal");

        yield return new WaitForSeconds(2.5f);
        SetGridColors(start, goal);

        yield return time;

        yield return BFSRoutine(start, goal);

        yield return new WaitForSeconds(2.5f);
        SetGridColors(start, goal);

        yield return DijkstraRoutine(start, goal);

        yield return new WaitForSeconds(2.5f);
        SetGridColors(start, goal);

        yield return GreedyBFSRoutine(start, goal);

        var path = start.GreedyBFS(goal);
        if (path.Count == 0)
        {
            Debug.LogError("No se encontro Camino!");
            yield break;
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            yield return Move(path[i], path[i + 1]);
        }

        StartCoroutine(Start());
    }

    private void SetGridColors(Node start, Node goal)
    {
        foreach (var item in grid.AllNodes)
        {
            item.SetColor(Color.white);
        }
        start.SetColor(Color.yellow);
        goal.SetColor(Color.green);
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

    public IEnumerator GreedyBFSRoutine(Node start, Node goal)
    {
        var frontier = new PriorityQueueMin<Node>();
        frontier.Enqueue(start, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);


        Node current = default;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();
            current.SetColor(Color.magenta * .4f);
            yield return time;

            if (current == goal)
            {
                //List<Node> path = new List<Node>();
                //Generación de camino
                while (current != start) //si quiero agregar el start lo cambio por != null
                {
                    // path.Add(current);
                    current.SetColor(Color.green);
                    current = cameFrom[current];
                    yield return time;
                }
                break;
            }

            foreach (var next in current.neighbours)
            {
                if (next.Blocked) continue;

                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next, next.Heuristic(goal));
                    cameFrom.Add(next, current);

                    next.SetColor(Color.blue);
                    yield return time;
                }
            }
        }
    }

    public IEnumerator DijkstraRoutine(Node start, Node goal)
    {
        PriorityQueueMin<Node> frontier = new PriorityQueueMin<Node>();
        frontier.Enqueue(start, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0);

        Node current = default;

        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                while (current != null)
                {
                    current.SetColor(Color.green);
                    current = cameFrom[current];
                    yield return time;
                }
                break; //terminamos de chequear, creamos el camino mas abajo
            }

            current.SetColor(Color.magenta * .6f);
            yield return time;


            foreach (var next in current.neighbours)
            {
                if (next.Blocked) continue;

                float newCost = costSoFar[current] + current.CostTo(next);

                if (!costSoFar.ContainsKey(next))
                {
                    costSoFar.Add(next, newCost);
                    frontier.Enqueue(next, newCost);
                    cameFrom.Add(next, current);
                    next.SetColor(Color.blue);
                }
                else if (newCost < costSoFar[current])
                {
                    frontier.Enqueue(next, newCost);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                    next.SetColor(Color.cyan);
                }
            }
            yield return time;
        }
    }

    public IEnumerator BFSRoutine(Node start, Node goal)
    {
        Queue<Node> pending = new Queue<Node>();
        pending.Enqueue(start);

        Dictionary<Node, Node> path = new Dictionary<Node, Node>();
        path.Add(start, null);


        Node current = default;
        while (pending.Count != 0)
        {
            current = pending.Dequeue();
            current.SetColor(Color.magenta * .6f);
            yield return time;

            if (current == goal)
            {
                //List<Node> path = new List<Node>();
                //Generación de camino
                while (current != start) //si quiero agregar el start lo cambio por != null
                {
                    // path.Add(current);
                    current.SetColor(Color.green);
                    current = path[current];
                    yield return time;
                }
                break;
            }

            foreach (var next in current.neighbours)
            {
                if (next.Blocked) continue;

                if (!path.ContainsKey(next))
                {
                    pending.Enqueue(next);
                    path.Add(next, current);

                    next.SetColor(Color.blue);
                    yield return time;
                }
            }
        }
    }
}