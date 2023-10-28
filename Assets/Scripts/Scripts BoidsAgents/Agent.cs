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
        //Debug.DrawRay(transform.position, velocity, Color.yellow);
        transform.position = TeleportBox.UpdatePosition(transform.position);

        velocity += Vector3.ClampMagnitude(acceleration, maxAcceleration);
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        acceleration = Vector3.zero;

        transform.position += velocity * Time.deltaTime;    // Aplicar velocidad
        if (velocity.magnitude > .1f)
            transform.forward = velocity.normalized;        // Mirar hacia la velocidad
    }

    public Vector3 Seek(Vector3 target)
    {
        var desired = (target - transform.position).normalized * maxSpeed;
        //Debug.DrawRay(transform.position, desired, Color.black);

        var steering = desired - velocity;
        //Debug.DrawRay(transform.position + velocity, steering, Color.blue);

        return steering;
    }

    public Vector3 Flee(Vector3 target)
    {
        var desired = (transform.position - target).normalized * maxSpeed;
        //Debug.DrawRay(transform.position, desired, Color.black);

        var steering = desired - velocity;
        //Debug.DrawRay(transform.position + velocity, steering, Color.red);

        return steering;
    }

    /*
    target_offset = target - position
    distance = length (target_offset)
    ramped_speed = max_speed * (distance / slowing_distance)
    clipped_speed = minimum (ramped_speed, max_speed)
    desired_velocity = (clipped_speed / distance) * target_offset
    steering = desired_velocity - velocity
    
     */

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

    // rigidbody.AddForce(addVelocity, ForceMode.Acceleration);
    public void Accelerate(Vector3 addVelocity)
    {
        //var clamped = Vector3.ClampMagnitude(addVelocity, maxAcceleration);
        //velocity = Vector3.ClampMagnitude(velocity + clamped, maxSpeed);
        acceleration += addVelocity;
    }
}

#region Old
/*
var degrees = Vector3.Angle(velocity, wishDir) * Time.deltaTime;
var turnInDegrees = Mathf.Min(degrees, maxTurnAngle * Time.deltaTime);

var value = Mathf.InverseLerp(0, degrees, turnInDegrees);
velocity = Vector3.Slerp(velocity, wishDir, value); 

wishDir *= DesiredSpeed(.1f, maxSpeed, velocity.normalized, wishDir);

private static float DesiredSpeed(float minSpeed, float maxSpeed, Vector3 currentDir, Vector3 wishDir)
{
    var dot = Vector3.Dot(currentDir, wishDir);
        
    return Mathf.Lerp(minSpeed, maxSpeed, dot + .5f);
}

 */
#endregion