using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //[SerializeField] Animator animator;

    public Vector3 velocity;


    private void Update()
    {
        // animator.SetFloat("_MoveSpeed", velocity.magnitude);
        //animator.SetFloat("_DirX", velocity.normalized.x);
        //animator.SetFloat("_DirY", velocity.normalized.z);

        transform.position += velocity * Time.deltaTime;
    }

    public void Talk()
    {
        velocity = Vector3.zero;
        //animator.SetTrigger("_Talk");
    }
}