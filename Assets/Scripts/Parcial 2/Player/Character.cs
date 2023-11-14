using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private Rigidbody myRig;
    private float yVelocity;
    public float speed = 200f;

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        Vector3 horizontalMove = moveDirection * speed * Time.deltaTime;
        myRig.velocity = new Vector3(horizontalMove.x, myRig.velocity.y, horizontalMove.z);
    }
}
