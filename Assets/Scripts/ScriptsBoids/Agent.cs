using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{

    [SerializeField]
    float maxAcceleration = .2f, maxSpeed = 4f;

    Vector3 velocity, acceleration;

    public Vector3 Velocity => velocity;
    public float MaxSpeed => maxSpeed;

    private void Awake()
    {
        velocity = transform.forward * maxSpeed;
    }

    private void Update()
    {
        transform.position = TeleportBox.UpdatePosition(transform.position);

        velocity += Vector3.ClampMagnitude(acceleration, maxAcceleration);
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        acceleration = Vector3.zero;

        transform.position += velocity * Time.deltaTime;  
        if (velocity.magnitude > .1f)
            transform.forward = velocity.normalized; // creo que esto rota al cubo
    }

    public Vector3 Seek(Vector3 target)
    {
        var desired = (target - transform.position).normalized * maxSpeed;

        var steering = desired - velocity;

        return steering;
    }

    public Vector3 Flee(Vector3 target)
    {
        var desired = (transform.position - target).normalized * maxSpeed;

        var steering = desired - velocity;

        return steering;
    }
    public Vector3 Arrive(Vector3 target, float slowingDistance)
    {
        Vector3 offset = target - transform.position;
        float distance = offset.magnitude;
        float rampedSpeed = maxSpeed * (distance / slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);
        Vector3 desiredVelocity = (clippedSpeed / distance) * offset;

        return desiredVelocity - velocity;
    }

    public Vector3 Evade(Agent agent)
    {
        return -Pursuit(agent);
    }

    public Vector3 Pursuit(Agent agent)
    {
        var futurePosition = agent.transform.position + agent.velocity;

        if (Vector3.Distance(transform.position, futurePosition) < agent.velocity.magnitude)
        {
            Debug.DrawLine(transform.position, agent.transform.position, Color.green);
            return Seek(agent.transform.position);
        }

        return Seek(futurePosition);
    }

    public void Accelerate(Vector3 addVelocity)
    {
        acceleration += addVelocity;
    }
}