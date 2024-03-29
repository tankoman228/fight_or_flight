using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public GameObject segmentPrefab; // Префаб фрагмента змеи
    public int numSegments = 13; // Количество сегментов

    public void InstantinateSegments()
    {
        SnakeFragment fragmentFirst = Instantiate(segmentPrefab).GetComponent<SnakeFragment>();
        fragmentFirst.Target = this.gameObject;

        var previous_fragment = fragmentFirst;

        for (int i = 1; i < numSegments; i++)
        {
            var fragment = Instantiate(segmentPrefab).GetComponent<SnakeFragment>();
            fragment.Target = previous_fragment.gameObject;
            //fragment.transform.localScale = Vector3.one / ((float)i);
            previous_fragment = fragment;
        }
    }
}
