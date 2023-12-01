using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Vector3 velocity;

    private Node currentNode;

    private void Update()
    {
        Vector3 direction = velocity.normalized;

        Vector3 adjustedVelocity = direction * velocity.magnitude;

        var lvel = transform.InverseTransformDirection(adjustedVelocity);

        transform.position += adjustedVelocity * Time.deltaTime;
        if (adjustedVelocity.sqrMagnitude > 0)
        {
            transform.forward = direction;
        }

        CheckNodeUnderPlayer();
    }

    private void CheckNodeUnderPlayer()
    {
        Node nodeUnderPlayer = PhysicalNodeGrid.Instance.GetClosest(transform.position);

        if (nodeUnderPlayer != currentNode)
        {
            if (currentNode != null)
            {
                currentNode.SetColor(Color.white);
            }

            if (nodeUnderPlayer != null)
            {
                nodeUnderPlayer.SetColor(Color.green);
            }

            currentNode = nodeUnderPlayer;
        }
    }

    public void Talk()
    {
        velocity = Vector3.zero;
    }
}