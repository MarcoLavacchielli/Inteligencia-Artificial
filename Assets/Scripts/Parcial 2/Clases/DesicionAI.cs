using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DesicionAI : MonoBehaviour
{
    [SerializeField]
    ViewDetection detect;
    [SerializeField]
    Character character;
    [SerializeField]
    PhysicalNodeGrid grid;

    [SerializeField]
    float moveSpeed = 4f;

    [SerializeField]
    Transform player;

    [SerializeField]
    Renderer render;

    [SerializeField]
    List<Node> path = new();

    int currentNodeIndex = 0;

    bool moving;

    MaterialPropertyBlock block;

    IDesicion tree;

    private void Awake()
    {
        tree = new Branch
        {
            Predicate = () => detect.InFieldOfView(player.position),
            OnTrue = new Branch
            {
                Predicate = () => detect.InLineOfSight(player.position),
                OnTrue = new Branch
                {
                    Predicate = () => Vector3.Distance(transform.position, player.position) < 3f,
                    OnTrue = new Leaf(Attack),
                    OnFalse = new Leaf(Chase),
                },
                OnFalse = new Leaf(Seek),
            },
            OnFalse = new Leaf(Patrol),
        };
        /*
         
         */
        block = new MaterialPropertyBlock { };
    }

    IEnumerator Start()
    {

        moving = false;
        yield return new WaitUntil(() => path != null && path.Count > 0);
        moving = true;

        yield return new WaitForSeconds(.1f);
        for (int i = path.Count - 1; i > 0; i--)
        {
            Node target; Vector3 dir;
            do
            {
                target = path[i - 1];
                dir = (target.transform.position - transform.position);
                character.velocity = dir.normalized * moveSpeed;
                yield return null;
            } while (Vector3.Distance(target.transform.position, transform.position) > 3f);
        }

        character.velocity = Vector3.zero;
        path = null;

        StartCoroutine(Start());
    }

    private void Update()
    {
        tree.Execute();
    }

    private void Patrol()
    {
        block.SetColor("_Color", Color.blue);
        render.SetPropertyBlock(block);
        // Verifica si hay nodos en la ruta de patrulla
        if (path != null && path.Count > 0)
        {
            // Establece el destino como el nodo actual en la ruta de patrulla
            Node targetNode = path[currentNodeIndex];
            // Calcula la direcci�n hacia el nodo
            Vector3 dir = (targetNode.transform.position - transform.position);
            // Actualiza la velocidad del personaje hacia el nodo
            character.velocity = dir.normalized * moveSpeed;
            // Si el personaje llega lo suficientemente cerca al nodo actual, pasa al siguiente nodo
            if (Vector3.Distance(targetNode.transform.position, transform.position) < 1f)
            {
                currentNodeIndex = (currentNodeIndex + 1) % path.Count;
            }
        }
        else
        {
            character.velocity = Vector3.zero;  // si no hay waypoints
        }
    }

    private void Seek()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        GoTo(player.position);
    }

    private void Chase()
    {
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);

        var dir = player.position - transform.position;
        character.velocity = dir.normalized * moveSpeed;
    }

    public void GoTo(Vector3 position)
    {
        if (moving) return;

        var start = grid.GetClosest(transform.position);
        var goal = grid.GetClosest(position);
        if (!start || !goal || start == goal) return;

        path = start.ThetaStar(goal);
        path.Reverse();
    }

    private void Attack()
    {
        block.SetColor("_Color", Color.red);
        render.SetPropertyBlock(block);

        character.velocity = Vector3.zero;
    }
}

interface IDesicion
{
    void Execute();
}

class Branch : IDesicion
{
    public Func<bool> Predicate;
    public IDesicion OnTrue, OnFalse;

    public void Execute()
    {
        if (Predicate())
            OnTrue.Execute();
        else
            OnFalse.Execute();
    }
}

class Leaf : IDesicion
{
    public Action Action;

    public Leaf(Action action)
    {
        Action = action;
    }

    public void Execute()
    {
        Action();
    }
}