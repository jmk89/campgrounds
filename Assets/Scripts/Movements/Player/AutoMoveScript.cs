using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveScript : MonoBehaviour
{

    Vector3 targetPosition = new Vector3(0f, 0f, 0f);
    Vector3 targetToLookAt = new Vector3(0f, 0f, 0f);
    bool reachedTargetDestination = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!reachedTargetDestination) {
            AutoMoveCharacter();
        }
        
    }

    public void SetAutoMoveParameters(Vector3 positionToMoveTo, Vector3 targetToLookAt) {
        targetPosition = positionToMoveTo;
        this.targetToLookAt = targetToLookAt;
        GetComponent<NoPhysicsMovementScript>().ThrustEnabled(false);
        // Debug.Log($"Setting rotation {rotation}");
        // GetComponent<NoPhysicsMovementScript>().SetAutoRotateTo(rotation);
        reachedTargetDestination = false;
    }

    public void AutoMoveCharacter() {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime);
        LookAt();

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f) {
            reachedTargetDestination = true;
            GetComponent<NoPhysicsMovementScript>().ThrustEnabled(true);
        }
    }

    public void LookAt() {
        transform.LookAt(this.targetToLookAt);
    }
}
