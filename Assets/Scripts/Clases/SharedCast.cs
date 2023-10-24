using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedCast : MonoBehaviour
{
    [SerializeField]
    float maxDistance = 5f, radius = 1f, weight = 2f;

    [SerializeField]
    LayerMask layerMask;

    RaycastHit hit;

    public bool IsHit { get; private set; }
    public RaycastHit Hit => hit;

    void Update()
    {
        IsHit = Physics.SphereCast(
            transform.position, radius,
            transform.forward, out hit,
            maxDistance, layerMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
