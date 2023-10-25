using UnityEngine;

public class EvadeTarget : MonoBehaviour
{
    [SerializeField]
    Agent agent;

    [SerializeField]
    Agent evade;

    [SerializeField]
    bool seek;

    private void Update()
    {
        var force = agent.Evade(evade);

        if (seek)
        {
            force += agent.Seek(evade.transform.position);
        }

        agent.Accelerate(force);
    }
}