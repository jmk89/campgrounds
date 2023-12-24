using UnityEngine;

public class RayCheckGround : MonoBehaviour
{
    [SerializeField] float maxHeight = 10f;
    [SerializeField] float forceHeight = 2f;
    [SerializeField] float down = 25f;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask castingMask;

    bool autoThrustEnabled = false;

    // Update is called once per frame
    void Update()
    {
        CheckHeight();
    }

    void CheckHeight() {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit ray_result;
        bool underMaxHeight = Physics.Raycast(ray, out ray_result, maxHeight);
        bool underForceHeight = Physics.Raycast(ray, out ray_result, forceHeight);
        
        if (!underMaxHeight) {
            rb.useGravity = true;
        } else {
            rb.useGravity = false;
        }

        if (!underForceHeight) {
            rb.AddForce(new UnityEngine.Vector3(0f, -down, 0f) * Time.deltaTime, ForceMode.Acceleration);
        }
    }
    
}
