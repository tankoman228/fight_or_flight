using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// —ледование камеры за игроком (или любой другой выбранной целью)
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static Transform target; //«а чем следим
    public float smoothSpeed = 0.125f;
    public Vector3 offset; //scrollx + scrolly


    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

