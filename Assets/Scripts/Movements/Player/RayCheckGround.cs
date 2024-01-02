using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayCheckGround : MonoBehaviour
{
    [SerializeField] Rigidbody character;
    [Header ("Downforces")]
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float firstDistance = 1f;
    [SerializeField] float firstDistanceDownForce = 150f;
    [SerializeField] float secondDistance = 2f;
    [SerializeField] float secondDistanceDownForce = 100;
    [Header ("Slope")]
    [SerializeField] float maxSlope = 45f;
    [Header ("Suspension")]
    [SerializeField] GameObject nosePoint;
    [SerializeField] float noseSuspensionBounceForce = 250f;
    [SerializeField] float noseSuspensionHeight = .5f;

    List<RaycastHit> maxDistanceRayCastHits;
    List<RaycastHit> firstDistanceRayCastHits;
    List<RaycastHit> secondDistanceRayCastHits;

    bool atMaxSlope = false;
    bool loadedNoseSuspension = false;


    // Update is called once per frame
    void Update()
    {
        CheckForTaggedHits();
        CheckForSlope();
        CheckForGround();
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

    void CheckForSlope() {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -transform.up, out hit, 100f)) {
            atMaxSlope = false;
            return;
        }
        float speed = hit.normal.y * 90f;
        atMaxSlope = speed < maxSlope && hit.transform.gameObject.tag == "DistanceCheckable";
    }

    void CheckForGround() {
        RaycastHit hit;
        if (!Physics.Raycast(nosePoint.transform.position, -transform.up, out hit, 10f)) {
            loadedNoseSuspension = false;
            return;
        }
        loadedNoseSuspension = hit.distance < noseSuspensionHeight && hit.transform.gameObject.tag == "DistanceCheckable";
    }

    void ApplyForces() {
        if (firstDistanceRayCastHits.Count < 1 || atMaxSlope) {
            character.AddForce(new Vector3(0f, -firstDistanceDownForce, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }
        if (secondDistanceRayCastHits.Count < 1 || atMaxSlope) {
            character.AddForce(new Vector3(0f, -secondDistanceDownForce, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }

        if (maxDistanceRayCastHits.Count < 1 || atMaxSlope) {
            character.useGravity = true;
        } else {
            character.useGravity = false;
        }

        if (loadedNoseSuspension) {
            character.AddForce(new Vector3(0f, noseSuspensionBounceForce, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }
    }

}
