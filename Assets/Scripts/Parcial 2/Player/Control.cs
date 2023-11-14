using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{

    public Character character;
    public Transform cam;
    public float moveSpeed;

    public void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        var move = (Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up) * y + Vector3.ProjectOnPlane(cam.transform.right, Vector3.up) * x).normalized;

        character.velocity = move * moveSpeed;
    }

}