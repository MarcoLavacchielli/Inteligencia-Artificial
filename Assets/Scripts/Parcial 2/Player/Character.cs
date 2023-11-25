using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //[SerializeField] Animator animator;
    public Vector3 velocity;

    private Node currentNode; // Nodo actual debajo del jugador

    private void Update()
    {
        var lvel = transform.InverseTransformDirection(velocity);
        // animator.SetFloat("_MoveSpeed", velocity.magnitude);
        //animator.SetFloat("_DirX", velocity.normalized.x);
        //animator.SetFloat("_DirY", velocity.normalized.z);

        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > 0)
        {
            transform.forward = velocity.normalized;
        }

        // Verificar si el jugador está sobre un nodo
        CheckNodeUnderPlayer();
    }

    private void CheckNodeUnderPlayer()
    {
        // Obtener el nodo debajo del jugador usando el script PhysicalNodeGrid
        Node nodeUnderPlayer = PhysicalNodeGrid.Instance.GetClosest(transform.position);

        // Verificar si el nodo ha cambiado
        if (nodeUnderPlayer != currentNode)
        {
            // Restaurar el color del nodo anterior
            if (currentNode != null)
            {
                currentNode.SetColor(Color.white);
            }

            // Cambiar el color del nuevo nodo
            if (nodeUnderPlayer != null)
            {
                nodeUnderPlayer.SetColor(Color.green);
            }

            // Actualizar el nodo actual
            currentNode = nodeUnderPlayer;
        }
    }

    public void Talk()
    {
        velocity = Vector3.zero;
        //animator.SetTrigger("_Talk");
    }
}