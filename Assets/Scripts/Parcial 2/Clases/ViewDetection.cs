using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDetection : MonoBehaviour
{

    [SerializeField]
    float viewRadius = 10f;

    [SerializeField]
    float senseRadius = 1.5f;

    [SerializeField]
    float viewAngle = 90f;

    [SerializeField]
    public LayerMask wallMask;

    private void Update()
    {

        //float minDistance = float.MaxValue;
        //Detecable min = null;
        //foreach (var item in Detecable.All)
        //{
        //    if (!InLineOfSight(item.transform.position))
        //        continue;

        //    float distance = Vector3.Distance(item.transform.position, transform.position);
        //    if (distance < minDistance)
        //    {
        //        minDistance = distance;
        //        min = item;
        //    }
        //}

        //if (min != null)
        //{
        //    // Persigo a este objeto
        //}
    }

    public bool InFieldOfView(Vector3 point)
    {
        var dir = point - transform.position;
        if (dir.magnitude > viewRadius)
            return false;

        if (dir.magnitude < senseRadius)
            return true;

        var angle = Vector3.Angle(dir, transform.forward);
        if (angle > viewAngle / 2)
            return false;

        return true;
    }

    public bool InLineOfSight(Vector3 point)
    {
        if (!InFieldOfView(point))
            return false;
        var dir = point - transform.position;

        return !Physics.Raycast(transform.position, dir, dir.magnitude, wallMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, senseRadius);


        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, viewRadius);

        var vector = transform.forward * viewRadius;

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, viewAngle / 2, 0) * vector);

        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -viewAngle / 2, 0) * vector);
    }

}

class Player : MonoBehaviour
{
    public static Player All;

}