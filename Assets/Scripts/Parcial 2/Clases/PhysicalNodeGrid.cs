using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PhysicalNodeGrid : MonoBehaviour
{
    [SerializeField]
    public LayerMask unwalkable;

    [SerializeField]
    int width = 5, height = 5;

    [SerializeField]
    float spacing = 1.3f;

    [SerializeField]
    Node prefab;

    List<Node> nodesList;

    public static PhysicalNodeGrid Instance { get; private set; }

    public IEnumerable<Node> AllNodes => nodesList;

    public Node GetClosest(Vector3 worldPosition)
    {
        var relative = worldPosition - transform.position;

        int x = Mathf.RoundToInt(relative.x / spacing);
        int y = Mathf.RoundToInt(relative.z / spacing);

        Node n;
        if (TryGetNode(x, y, out n))
            return n;
        if (TryGetNode(x + 1, y, out n))
            return n;
        if (TryGetNode(x - 1, y, out n))
            return n;
        if (TryGetNode(x, y + 1, out n))
            return n;
        if (TryGetNode(x, y - 1, out n))
            return n;

        /*if (TryGetNode(x, y, out var n))
            return n;
        */
        return null;
    }

    public List<Node> GetNodesList()
    {
        return nodesList;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Generate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Generate()
    {
        if (nodesList != null)
        {
            foreach (var item in nodesList)
            {
                if (item)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        nodesList = new List<Node>(width * height);

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
                nodesList.Add(node);
            }
        }

        foreach (var node in nodesList)
        {
            var pos = node.transform.position;
            int x = Mathf.RoundToInt((pos.x - transform.position.x) / spacing);
            int y = Mathf.RoundToInt((pos.z - transform.position.z) / spacing);

            AddNeighbour(node, x + 1, y);
            AddNeighbour(node, x - 1, y);
            AddNeighbour(node, x, y + 1);
            AddNeighbour(node, x, y - 1);

            AddNeighbour(node, x + 1, y + 1);
            AddNeighbour(node, x - 1, y - 1);
            AddNeighbour(node, x - 1, y + 1);
            AddNeighbour(node, x + 1, y - 1);
        }
    }

    public void ClearGrid()
    {
        if (nodesList != null)
        {
            foreach (var item in nodesList)
            {
                if (item)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
        }

        nodesList.Clear();
    }

    private void AddNeighbour(Node node, int x, int y)
    {
        if (TryGetNode(x, y, out var n) && n != null)
        {
            var direction = n.transform.position - node.transform.position;
            var distance = direction.magnitude;
            var hit = Physics.Raycast(node.transform.position, direction.normalized, distance, unwalkable);

            if (!hit)
                node.neighbours.Add(n);
        }
    }

    bool TryGetNode(int x, int y, out Node node)
    {
        foreach (var n in nodesList)
        {
            var pos = n.transform.position;
            int posX = Mathf.RoundToInt((pos.x - transform.position.x) / spacing);
            int posY = Mathf.RoundToInt((pos.z - transform.position.z) / spacing);

            if (posX == x && posY == y)
            {
                node = n;
                return true;
            }
        }

        node = null;
        return false;
    }
}