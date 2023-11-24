using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotateDegreesAroundXAxis = 0;
    [SerializeField] private float rotateDegreesAroundYAxis = 0;
    [SerializeField] private float rotateDegreesAroundZAxis = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(
                rotateDegreesAroundXAxis * Time.deltaTime, 
                rotateDegreesAroundYAxis * Time.deltaTime, 
                rotateDegreesAroundZAxis * Time.deltaTime));
    }
}
