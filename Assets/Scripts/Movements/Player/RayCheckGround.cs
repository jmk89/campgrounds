using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayCheckGround : MonoBehaviour
{
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float firstDistance = 1f;
    [SerializeField] float firstDistanceDownForce = 150f;
    [SerializeField] float secondDistance = 2f;
    [SerializeField] float secondDistanceDownForce = 100;
    [SerializeField] Rigidbody rb;

    List<RaycastHit> maxDistanceRayCastHits;
    List<RaycastHit> firstDistanceRayCastHits;
    List<RaycastHit> secondDistanceRayCastHits;


    // Update is called once per frame
    void Update()
    {
        CheckForTaggedHits();
        ApplyForces();
    }

    void CheckForTaggedHits() {
        Ray ray = new Ray(transform.position, Vector3.down);
        maxDistanceRayCastHits = Physics.SphereCastAll(ray, maxDistance, 0.01f)
            .ToList().FindAll(hit => hit.collider?.gameObject?.tag == "DistanceCheckable");
        firstDistanceRayCastHits = Physics.SphereCastAll(ray, firstDistance, 0.01f)
            .ToList().FindAll(hit => hit.collider?.gameObject?.tag == "DistanceCheckable");
        secondDistanceRayCastHits = Physics.SphereCastAll(ray, secondDistance, 0.01f)
            .ToList().FindAll(hit => hit.collider?.gameObject?.tag == "DistanceCheckable");
    }

    void ApplyForces() {
        if (firstDistanceRayCastHits.Count < 1) {
            Debug.Log("Applying Down Force from SphereCast");
            rb.AddForce(new Vector3(0f, -firstDistanceDownForce, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }
        if (secondDistanceRayCastHits.Count < 1) {
            Debug.Log("Applying Down Force from SphereCast");
            rb.AddForce(new Vector3(0f, -secondDistanceDownForce, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }

        if (maxDistanceRayCastHits.Count < 1) {
            Debug.Log("Using Gravity");
            rb.useGravity = true;
        } else {
            rb.useGravity = false;
        }
    }

}
