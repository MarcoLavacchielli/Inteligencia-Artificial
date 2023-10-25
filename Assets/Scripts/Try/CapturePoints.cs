using UnityEngine;

public class CapturePoints : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField, Min(0f)]
    float detectRadius = 16f, pickupRadius = 1f;

    [SerializeField]
    LayerMask layerMask, wallMask;

    [SerializeField]
    float weight = 1f;

    Collider[] points = new Collider[30];

    private void Update()
    {
        int len = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, points, layerMask);

        if (len <= 0) return;

        int closest = -1;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < len; i++)
        {
            if (Physics.Raycast(transform.position,
                (points[i].transform.position - transform.position), detectRadius, wallMask))
                continue;

            var distance = Vector3.Distance(transform.position, points[i].transform.position);
            if (distance < closestDistance)
            {
                closest = i;
                closestDistance = distance;
            }
        }

        if (closest == -1) return;

        var currentTransform = points[closest].transform;
        var currentDistance = Vector3.Distance(transform.position, currentTransform.position);

        if (currentDistance < pickupRadius)
            Destroy(points[closest].gameObject);

        print("GOTO");

        agent.Accelerate(agent.Seek(currentTransform.position) * weight);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(.9f, .6f, 0);
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}