using UnityEngine;

public class EvadeTarget : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    Agent evade;

    [SerializeField]
    bool seek;

    [SerializeField]
    float evadeRadius = 5f;

    private void Update()
    {
        if (IsWithinRadius(evade.transform.position, transform.position, evadeRadius))
        {
            var force = agent.Evade(evade);

            if (seek)
            {
                force += agent.Seek(evade.transform.position);
            }

            agent.Accelerate(force);
        }
    }

    private bool IsWithinRadius(Vector3 position1, Vector3 position2, float radius)
    {
        // Comprueba si la distancia entre el position 1 y 2 es menor o igual al radio
        return Vector3.Distance(position1, position2) <= radius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, evadeRadius);
    }
}