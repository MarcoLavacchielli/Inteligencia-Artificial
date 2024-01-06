using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiderController : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] PhysicalNodeGrid grid;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Renderer render;
    [SerializeField] Pathfinder pathfinder;
    private Node lastKnownPlayerNode;

    Node clickedNode;

    int currentNodeIndex = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                clickedNode = grid.GetClosest(hit.point);

                if (clickedNode != null)
                {
                    // Cambia el color del nodo a verde
                    clickedNode.ChangeNodeColor(Color.green);
                }
            }

            MoveByPathFinder();
        }
    }

    private void MoveByPathFinder()
    {

        pathfinder.path.Clear();
        pathfinder.current = null;
        lastKnownPlayerNode = null;


        if (pathfinder.current == null)
        {
            lastKnownPlayerNode = grid.GetClosest(clickedNode.WorldPosition);
            pathfinder.current = grid.GetClosest(transform.position);
        }

        currentNodeIndex = 0;

        pathfinder.target = lastKnownPlayerNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownPlayerNode);

        if (pathfinder.path.Count > 0)
        {
            pathfinder.current = pathfinder.path[0];
        }
        else
        {
            // No hay camino disponible
            return;
        }

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownPlayerNode);
    }

    private IEnumerator FollowPathAndCheckForPlayer()
    {
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
                // Player llegó al nodo destino, limpiar el path
                /*pathfinder.path.Clear();
                pathfinder.current = null;
                lastKnownPlayerNode = null;*/

                yield break; // Salir del coroutine ya que no hay más nodos en el path
            }

            yield return new WaitForSeconds(0.1f);
        }

        character.velocity = Vector3.zero;
    }


}