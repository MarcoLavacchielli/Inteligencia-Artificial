using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalNodeGrid : MonoBehaviour
{


    [SerializeField]
    int width = 5, height = 5;

    [SerializeField]
    float spacing = 1.3f;

    [SerializeField]
    Node prefab;

    Node[,] nodes;


    public IEnumerable<Node> AllNodes
    {
        get
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
    }

    public Node GetRandom()
    {
        if (nodes == null)
            return null;

        return nodes[
            Random.Range(0, nodes.GetLength(0)),
            Random.Range(0, nodes.GetLength(1))
            ];
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = transform.position;
                pos.x += x * spacing;
                pos.z += y * spacing;
                var node = Instantiate(prefab, transform);
                node.transform.position = pos;
                nodes[x, y] = node;
                //node.color = Random.ColorHSV(0f, 1f, .25f, .95f, .1f, .95f);
            }
        }

        // Setup neighbours
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
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

    private void AddNeighbour(Node node, int x, int y)
    {
        if (TryGetNode(x, y, out var n))
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