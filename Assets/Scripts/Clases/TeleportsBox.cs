using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportsBox : MonoBehaviour
{
    static TeleportsBox instance;

    [SerializeField]
    Bounds bounds;

    [SerializeField]
    Color color;


    private void Awake()
    {
        instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = color * .3f;

        Gizmos.DrawCube(bounds.center, bounds.size);
    }

    public static Vector3 UpdatePosition(Vector3 position)
    {
        var max = instance.bounds.max;
        var min = instance.bounds.min;
        var size = instance.bounds.size;

        if (position.x > max.x) position.x -= size.x;
        if (position.x < min.x) position.x += size.x;
        if (position.y > max.y) position.y -= size.y;
        if (position.y < min.y) position.y += size.y;
        if (position.z > max.z) position.z -= size.z;
        if (position.z < min.z) position.z += size.z;

        return position;
    }
}