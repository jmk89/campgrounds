using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UpdateCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateFollowTarget(GameObject gameObject) {
        CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();


        cam.Follow = gameObject.transform;
        cam.LookAt = gameObject.transform;
        

    }
}
