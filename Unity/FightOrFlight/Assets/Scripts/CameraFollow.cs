using UnityEngine;

/// <summary>
/// ���������� ������ �� ������� (��� ����� ������ ��������� �����)
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static Transform target; //�� ��� ������
    public float smoothSpeed = 0.125f;
    public Vector3 offset; //scrollx + scrolly


    void LateUpdate()
    {
        Vector3 targetDirection = target.right; // �������� �����������, � ������� ������� �����
        Vector3 desiredPosition = target.position + offset + targetDirection * 2f; // �������� ���� ������ � ����������� ������
        desiredPosition.z = -10;
        
        //transform.position = desiredPosition;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

