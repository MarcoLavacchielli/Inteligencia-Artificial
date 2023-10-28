using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{

    [SerializeField]
    Transform target;

    [SerializeField]
    bool followTarget = false;

    [SerializeField]
    Vector3 offset, lookOffset;

    Vector3 velocity;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            followTarget = !followTarget;
    }

    private void LateUpdate()
    {
        var desiredPos = followTarget ? target.TransformPoint(offset) : startPosition;

        var lookAt = followTarget ? target.TransformPoint(lookOffset) : target.position;

        transform.SetPositionAndRotation(
            Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, .2f),
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookAt - transform.position), .2f));
    }


}