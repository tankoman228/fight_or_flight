using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooParticle : MonoBehaviour
{
    Vector3 delta = Vector3.zero;
    Vector3 deltaSpeed = Vector3.zero;
    public Transform startVector;

    private float maxChange = 0.11f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startVector.position + delta;
        delta += deltaSpeed;

        deltaSpeed += new Vector3(Random.Range(-Time.deltaTime, Time.deltaTime), Random.Range(-Time.deltaTime, Time.deltaTime), 0);
        while (System.Math.Abs(delta.x + delta.y) > maxChange)
        {
            delta *= 0.9f;
            deltaSpeed *= -0.4f;
        }

        if (System.Math.Abs(delta.x + delta.y) < 0.05)
            Update();
    }
}
