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
        Vector3 targetDirection = target.right; // ѕолучаем направление, в котором смотрит игрок
        Vector3 desiredPosition = target.position + offset + targetDirection * 2f; // —двигаем цель камеры в направлении игрока
        desiredPosition.z = -10;
        
        //transform.position = desiredPosition;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

