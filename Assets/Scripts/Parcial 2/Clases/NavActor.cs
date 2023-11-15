using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NavActor : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    PhysicalNodeGrid grid;

    [SerializeField]
    float moveTime = 1f;

    [SerializeField]
    Vector3 offset = new(0, 1.5f, 0);

    float[] times = new float[]
    {
        1f,
        .5f,
        .25f,
        .1f,
        .05f,
        .025f,
        .01f,
        .005f,
    };
    int currentTime = 0;

    Node start, goal;
    List<Node> path = new();

    bool moving;


    Node GetCurrent()
    {
        if (!Physics.Raycast(transform.position, -transform.up, out var hit, 9999f))
            return null;

        return hit.collider.GetComponent<Node>();
    }

    private WaitUntil GetTime()
    {
        float t = 0;
        return new WaitUntil(() =>
        {
            t += Time.deltaTime / times[currentTime];
            return t >= 1;
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            currentTime++;
        if (Input.GetKeyDown(KeyCode.S))
            currentTime--;
        currentTime = Mathf.Clamp(currentTime, 0, times.Length - 1);
    }

    public void GoTo(Vector3 position)
    {
        if (moving) return;

        start = grid.GetClosest(transform.position);
        goal = grid.GetClosest(position);
        if (!start || !goal || start == goal) return;

        path = start.ThetaStar(goal);
        path.Reverse();
    }

    IEnumerator Start()
    {

        /*while (true)
        {
            yield return null;

            if (Input.GetMouseButton(0))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(Vector3.up, .5f);
                if (plane.Raycast(ray, out float e))
                {
                    goal = grid.GetClosest(ray.GetPoint(e));
                }
                start = grid.GetClosest(transform.position);
                SetGridColors();
            }

            if (!start || !goal) continue;
            if (Input.GetKeyDown(KeyCode.G))
            {
                yield return GreedyBFSRoutine(start, goal);

                if (path.Count == 0)
                {
                    Debug.Log("No camino");
                    continue;
                }

                break;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                yield return DijkstraRoutine(start, goal);

                if (path.Count == 0)
                {
                    Debug.Log("No hay camino");
                    continue;
                }

                break;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                path = start.ThetaStar(goal);
                path.Reverse();

                foreach (var node in path)
                {
                    node.SetColor(Color.green);
                }

                if (path.Count == 0)
                {
                    Debug.LogError("No se encontro camino!");
                    continue;
                }

                break;
            }
        }*/
        moving = false;
        yield return new WaitUntil(() => path != null && path.Count > 0);
        moving = true;

        yield return new WaitForSeconds(.1f);
        for(int i = path.Count -1; i > 0; i--)
        {
            yield return MoveTo(path[i - 1]);
        }

        path = null;
        StartCoroutine(Start());
    }

    private void SetAllColors(Color color)
    {
        foreach(var item in grid.AllNodes)
        {
            item.SetColor(color);
        }
    }

    private void SetGridColors(Node start, Node goal)
    {
        SetAllColors(Color.white);
        if (start) start.SetColor(Color.yellow);
        if (goal) goal.SetColor(Color.green);
    }

    IEnumerator MoveTo(Node node)
    {
        var start = transform.position;
        var goal = node.transform.position;
        for (float f = 0; f < 1; f += Time.deltaTime / (goal-start).magnitude / moveTime)
        {
            yield return null;
            transform.position = Vector3.Lerp(
              start, goal, f)
                + offset;
        }
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