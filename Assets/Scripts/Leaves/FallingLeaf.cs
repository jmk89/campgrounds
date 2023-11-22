using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLeaf : MonoBehaviour
{

    Vector3 startingPosition;
    [SerializeField] Vector3 oscillatorVector = new Vector3(10f, 5f);
    [SerializeField] float fallingFactor = 5f;
    [SerializeField] [Range(0, 1)] float movementFactorX = 0f;
    [SerializeField] [Range(0, 1)] float movementFactorY = 0f;
    [SerializeField] float period = 2f;
    float startTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float lifespan = Time.time - startTime;
        //Mathf.Epsilon is the smallest float possible
        if (period <= Mathf.Epsilon) { return; } // more reliable than comparing to zero
        float cycles = lifespan / period; // continually growing over time
        const float tau = Mathf.PI * 2; // constant value
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to 1
        float rawCosWave = -Mathf.Abs(Mathf.Cos(cycles * tau));

        movementFactorX = (rawSinWave + 1f) / 2f; // recalculate to go from 0 to 1 so its cleaner
        movementFactorY = (rawCosWave + 1f) / 2f;

        float oscillatorOffsetX = oscillatorVector.x * movementFactorX;
        float oscillatorOffsetY = oscillatorVector.y * movementFactorY;
        float straightDownY = fallingFactor * lifespan;

        Vector3 offset = new Vector3(oscillatorOffsetX, straightDownY + oscillatorOffsetY, 0f);
        transform.position = startingPosition + offset;
    }
}
