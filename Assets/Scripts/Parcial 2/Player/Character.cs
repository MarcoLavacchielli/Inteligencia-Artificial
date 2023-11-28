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
        // Calcular la dirección normalizada
        Vector3 direction = velocity.normalized;

        // Calcular la velocidad considerando la magnitud original
        Vector3 adjustedVelocity = direction * velocity.magnitude;

        var lvel = transform.InverseTransformDirection(adjustedVelocity);
        // animator.SetFloat("_MoveSpeed", adjustedVelocity.magnitude);
        //animator.SetFloat("_DirX", direction.x);
        //animator.SetFloat("_DirY", direction.z);

        transform.position += adjustedVelocity * Time.deltaTime;
        if (adjustedVelocity.sqrMagnitude > 0)
        {
            transform.forward = direction;
        }

        // Verificar si el jugador está sobre un nodo
        CheckNodeUnderPlayer();
    }

    // Hecho para checkeo, aparte que es algo visual y funciona, ahora no se ve, pero en el testing funcaba
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