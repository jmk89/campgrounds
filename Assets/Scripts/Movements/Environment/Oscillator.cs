using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{

    Vector3 startingPosition;
    [SerializeField] Vector3 movementVector;
    [SerializeField] [Range(0, 1)] float movementFactorX;
    [SerializeField] [Range(0, 1)] float movementFactorY;
    [SerializeField] float period = 2f;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Mathf.Epsilon is the smallest float possible
        if (period <= Mathf.Epsilon) { return; } // more reliable than comparing to zero
        float cycles = Time.time / period; // continually growing over time
        const float tau = Mathf.PI * 2; // constant value
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to 1
        float rawCosWave = Mathf.Cos(cycles * tau);

        movementFactorX = (rawSinWave + 1f) / 2f; // recalculate to go from 0 to 1 so its cleaner
        movementFactorY = (rawCosWave + 1f) / 2f;

        float offsetX = movementVector.x * movementFactorX;
        float offsetY = movementVector.y * movementFactorY;

        Vector3 offset = new Vector3(offsetX, offsetY, 0f);
        transform.position = startingPosition + offset;
    }
}
