﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breathe : MonoBehaviour
{
    Vector3 startPos;
    public float amplitude = 10f;
    public float period = 5f;

    protected void Start()
    {
        startPos = transform.position;
    }

    protected void Update()
    {
        startPos = transform.parent.position;

        float theta = Time.timeSinceLevelLoad / period;
        float distance = amplitude * Mathf.Sin(theta);
        transform.position = startPos + Vector3.up * distance;
    }
}
