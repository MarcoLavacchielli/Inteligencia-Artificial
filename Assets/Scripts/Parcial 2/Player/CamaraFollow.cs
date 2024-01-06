using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{

    [SerializeField] Transform target; // target es el character
    Vector3 offset; //diferencia de camara y enemy
    [SerializeField] Vector3 offsetPublico; // cambio publico a la camara
    Vector3 final;
    //[SerializeField] float lerpspeed = 1;

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {

        transform.position = target.transform.position + offset + offsetPublico;

    }

}