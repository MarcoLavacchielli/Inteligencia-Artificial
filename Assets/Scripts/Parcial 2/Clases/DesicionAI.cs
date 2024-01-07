using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesicionAI : MonoBehaviour
{
    [SerializeField] ViewDetection detect;
    [SerializeField] Character character;
    [SerializeField] PhysicalNodeGrid grid;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Transform enemy;
    [SerializeField] Renderer render;
    [SerializeField] List<Node> path = new List<Node>();
    int currentNodeIndex = 0;
    MaterialPropertyBlock block;
    public State currentState;
    [SerializeField] Pathfinder pathfinder;
    private bool playerInSight = false;
    private Node lastKnownEnemyNode;
    private Node clickedNode;
    public static List<DesicionAI> allGuardians = new List<DesicionAI>();
    float attackDistance = 2.0f;
    private static bool anyGuardInReturnState = false;
    bool cleanOne = false;
    [SerializeField] private bool isMoving = false;


    public enum State
    {
        LiderMove,
        Chase,
        Attack,
        ReturnToLastKnownPosition,
    }

    private void Awake()
    {
        block = new MaterialPropertyBlock { };
        allGuardians.Add(this);
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case State.LiderMove:
                LiderMove();
                if (detect.InLineOfSight(enemy.position))
                {
                    ChangeState(State.Chase);
                }
                break;
            case State.Chase:
                Chase();
                if (!detect.InLineOfSight(enemy.position))
                {
                    ChangeState(State.ReturnToLastKnownPosition);
                }
                else if (Vector3.Distance(transform.position, enemy.position) < attackDistance)
                {
                    ChangeState(State.Attack);
                }
                break;
            case State.Attack:
                Attack();
                if (Vector3.Distance(transform.position, enemy.position) > attackDistance)
                {
                    ChangeState(State.Chase);
                }
                break;
            case State.ReturnToLastKnownPosition:
                ReturnToLastKnownPosition();
                if (detect.InLineOfSight(lastKnownEnemyNode.WorldPosition) && Vector3.Distance(transform.position, lastKnownEnemyNode.WorldPosition) < 1.0f && !detect.InFieldOfView(enemy.position))
                {
                    ChangeState(State.LiderMove);
                }
                else if (detect.InLineOfSight(enemy.position))
                {
                    ChangeState(State.Chase);
                }
                break;
            default:
                break;
        }
    }

    private void LiderMove()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                clickedNode = grid.GetClosest(hit.point);

                if (clickedNode != null)
                {
                    clickedNode.ChangeNodeColor(Color.green);
                }
            }

            PathMove();
        }
    }

    private void PathMove()
    {
        if (pathfinder.current == null)
        {
            lastKnownEnemyNode = grid.GetClosest(clickedNode.WorldPosition);
            pathfinder.current = grid.GetClosest(transform.position);
        }

        currentNodeIndex = 0;

        pathfinder.target = lastKnownEnemyNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownEnemyNode);

        if (pathfinder.path.Count > 0)
        {
            pathfinder.current = pathfinder.path[0];
        }
        else
        {
            return;
        }

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownEnemyNode);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    /*private void HomeComing()
    {

        if (cleanOne == false)
        {
            pathfinder.path.Clear();
            Debug.Log("Clean");
            cleanOne = true;
        }

        block.SetColor("_Color", Color.magenta);
        render.SetPropertyBlock(block);

        if (pathfinder.current == null)
        {
            lastKnownEnemyNode = grid.GetClosest(path[0].WorldPosition);
            pathfinder.current = grid.GetClosest(transform.position);
        }
        lastKnownEnemyNode = grid.GetClosest(path[0].WorldPosition);

        currentNodeIndex = 0;

        pathfinder.target = lastKnownEnemyNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownEnemyNode);

        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        Vector3 current = transform.position;

        /*foreach (var node in pathfinder.path)  // Linea para checkear la vuelta
        {
            Debug.DrawLine(current, node.WorldPosition, Color.red, 99);
            current = node.WorldPosition;
        }

        character.velocity = Vector3.zero;

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownEnemyNode);
    }*/

    private void ReturnToLastKnownPosition()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        if (!anyGuardInReturnState)
        {
            anyGuardInReturnState = true;

            foreach (DesicionAI guard in allGuardians)
            {
                guard.ChangeState(State.ReturnToLastKnownPosition);
            }
        }

        if (pathfinder.current == null)
        {
            lastKnownEnemyNode = grid.GetClosest(enemy.position);
            pathfinder.current = grid.GetClosest(transform.position);
        }

        currentNodeIndex = 0;

        pathfinder.target = lastKnownEnemyNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownEnemyNode);

        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        Vector3 current = transform.position;

        /*foreach (var node in pathfinder.path)  //Linea para checkear la ida
        {
            Debug.DrawLine(current, node.WorldPosition, Color.blue, 99);
            current = node.WorldPosition;
        }*/

        character.velocity = Vector3.zero;

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownEnemyNode);

        foreach (DesicionAI guard in allGuardians)
        {
            guard.lastKnownEnemyNode = lastKnownEnemyNode;
        }
    }

    private IEnumerator FollowPathAndCheckForPlayer()
    {
        isMoving = true;
        int targetIndex = 1;

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position);
                character.velocity = dir.normalized * moveSpeed;

                yield return null;
            }

            character.velocity = Vector3.zero;

            if (targetIndex < pathfinder.path.Count - 1)
            {
                targetIndex++;
                pathfinder.current = pathfinder.path[targetIndex - 1];
            }
            else
            {
                pathfinder.path.Clear();
                pathfinder.current = null;
                pathfinder.target = null;
                lastKnownEnemyNode = null;
                isMoving = false;
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        character.velocity = Vector3.zero;
        isMoving = false;
    }

    /*private void Patrol()
    {
        block.SetColor("_Color", Color.blue);
        render.SetPropertyBlock(block);

        if (path != null && path.Count > 0)
        {
            Node targetNode = path[currentNodeIndex];
            Vector3 dir = (targetNode.transform.position - transform.position);

            if (Vector3.Distance(targetNode.transform.position, transform.position) < 1f)
            {
                character.velocity = Vector3.zero;
                pathfinder.current = targetNode;
                currentNodeIndex++;

                if (currentNodeIndex < path.Count)
                {
                    pathfinder.target = path[currentNodeIndex];
                }
                else
                {
                    currentNodeIndex = 0;
                    pathfinder.target = path[currentNodeIndex];
                    pathfinder.UpdateTarget(path[currentNodeIndex]);
                }
            }
            else
            {
                character.velocity = dir.normalized * moveSpeed;
            }
        }
        else
        {
            character.velocity = Vector3.zero;
        }
    }*/

    private void Chase()
    {
        Node previousTarget = pathfinder.target;
        pathfinder.current = null;
        pathfinder.target = null;
        pathfinder.path.Clear();
        lastKnownEnemyNode = grid.GetClosest(enemy.position);
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);
        var dir = enemy.position - transform.position;
        character.velocity = dir.normalized * moveSpeed;
        anyGuardInReturnState = false;
    }

    private void Attack()
    {
        block.SetColor("_Color", Color.red);
        render.SetPropertyBlock(block);
        character.velocity = Vector3.zero;
    }
}