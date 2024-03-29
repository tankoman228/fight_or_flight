using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeFragment : MonoBehaviour
{
    internal GameObject Target
    {
        get { return target; }
        set { 
            target = value; 
            if (EventsManager.currentPlayer.graphicsCherv.gameObject.activeSelf)
            {
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
    public float followSpeed = 5f;
    public float minDistance = 0.9f;
    public float maxDistance = 1.4f;
    
    private GameObject target;


    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.transform.position + new Vector3(0, 0, 0.1f);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);

        // Keep a minimum distance
        if (Vector3.Distance(transform.position, targetPos) < minDistance)
        {
            transform.position = targetPos - (targetPos - transform.position).normalized * minDistance;
        }

        // Keep a maximum distance
        if (Vector3.Distance(transform.position, targetPos) > maxDistance)
        {
            transform.position = targetPos - (targetPos - transform.position).normalized * maxDistance;
        }

        // Rotate to match the target's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, followSpeed * Time.deltaTime * 2);
    }
}
