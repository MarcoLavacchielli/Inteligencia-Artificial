using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PhysicalNodeGrid : MonoBehaviour
{

    [SerializeField]
    LayerMask unwalkable;

    [SerializeField]
    int width = 5, height = 5;

    [SerializeField]
    float spacing = 1.3f;

    [SerializeField]
    Node prefab;

    Node[,] nodes;

    List<Node> nodesList;

    public IEnumerable<Node> AllNodes => nodesList;

    public Node GetClosest(Vector3 worldPosition)
    {
        var relative = worldPosition - transform.position;

        int x = Mathf.RoundToInt(relative.x / spacing);
        int y = Mathf.RoundToInt(relative.z / spacing);

        if (TryGetNode(x, y, out var n))
            return n;
        return null;
    }

    public List<Node> GetNodesList()
    {
        return nodesList;
    }

    private void Awake()
    {
        Generate();
    }

    void Generate()
    {
        if (nodes != null)
            foreach (var item in nodes)
                if (item)
                    Destroy(item);

        nodes = new Node[width, height];
        nodesList = new(width * height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = transform.position;
                pos.x += x * spacing;
                pos.z += y * spacing;

                if (Physics.BoxCast(pos + Vector3.up * 10, Vector3.one / 2, Vector3.down, Quaternion.identity, 20f, unwalkable))
                {
                    continue;
                }

                var node = Instantiate(prefab, transform);
                node.transform.position = pos;
                nodes[x, y] = node;
                nodesList.Add(node);
                //node.color = Random.ColorHSV(0f, 1f, .25f, .95f, .1f, .95f);
            }
        }

        // Setup neighbours
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (nodes[x, y] == null) continue;

                AddNeighbour(nodes[x, y], x + 1, y);
                AddNeighbour(nodes[x, y], x - 1, y);
                AddNeighbour(nodes[x, y], x, y + 1);
                AddNeighbour(nodes[x, y], x, y - 1);

                AddNeighbour(nodes[x, y], x + 1, y + 1);
                AddNeighbour(nodes[x, y], x - 1, y - 1);
                AddNeighbour(nodes[x, y], x - 1, y + 1);
                AddNeighbour(nodes[x, y], x + 1, y - 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        Gizmos.color = Color.white;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = transform.position;
                pos.x += x * spacing;
                pos.z += y * spacing;

                bool wall = Physics.BoxCast(pos + Vector3.up * 10, Vector3.one / 2, Vector3.down, Quaternion.identity, 20f, unwalkable);
                //bool wall = Physics.Raycast(pos + Vector3.up * 10, Vector3.down, 20f, unwalkable);
                //Gizmos.DrawRay(pos + Vector3.up * 10, Vector3.down * 20f);

                //var color = wall ? Color.red : Color.green;
                if (!wall)
                    Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

    private void AddNeighbour(Node node, int x, int y)
    {
        if (TryGetNode(x, y, out var n) && n != null)
            node.neighbours.Add(n);
    }

    bool TryGetNode(int x, int y, out Node node)
    {
        if (x < 0 || y < 0 || x >= nodes.GetLength(0) || y >= nodes.GetLength(1))
        {
            node = null;
            return false;
        }

        node = nodes[x, y];
        return true;
    }
}