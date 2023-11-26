using System;
using UnityEngine;

public class MovementRocket : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float translationSpeed = 20000f;
    [SerializeField] float fartForce = 1f;
    [SerializeField] float autoRotateSpeed = 3f;
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;
    
    [SerializeField] AudioClip mainFartEngine;

    Rigidbody rb;
    AudioSource audioSource;
    Vector3 into = new Vector3(1, 0, 0);
    Vector3 outOf = new Vector3(-1, 0, 0);
    Quaternion zRotation = new Quaternion(0.7f, 0f, 0f, 0.7f);
    Quaternion autoRotateTo = Quaternion.identity;
    MovementMode movementMode;
    bool autoRotating = false;
    float slerpTime = 0f;
    enum MovementMode {Rotation, Translation};

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        ProcessThrust();
        ProcessAutoRotation();
        ProcessMovements();

        if (Input.GetKeyDown(KeyCode.Q)) {
            Debug.Log("x: " + (autoRotateTo.x - zRotation.x));
            Debug.Log("y: " + (autoRotateTo.y - zRotation.y));
            Debug.Log("z: " + (autoRotateTo.z - zRotation.z));

        }
    }

    void ProcessMovements() {
        if (this.movementMode == MovementMode.Translation) {
        ProcessTranslation();
        }
        ProcessRotation();
    }

    void ProcessTranslation() {
        Input.GetAxisRaw("Horizontal");

        float xValue = Input.GetAxisRaw("Horizontal") * Time.deltaTime * translationSpeed * 30f;
        float yValue = Input.GetAxisRaw("Vertical") * Time.deltaTime * translationSpeed * 30f;
        // Debug.Log("translating x: " + xValue + ". translating y: " + yValue);
        transform.Translate(xValue, 0f, yValue);
    }

    void ProcessThrust() {
        if (Input.GetKey(KeyCode.Space)) {
            StartThrusting();
        } else {
            StopThrusting();
        }
    }

    void ProcessAutoRotation() {
        if (Input.GetKeyDown(KeyCode.X)) {
            autoRotating = true;
            autoRotateTo = Quaternion.identity;
            movementMode = MovementMode.Rotation;
        } else if (Input.GetKeyDown(KeyCode.T)) {
            autoRotating = true;
            autoRotateTo = zRotation;
            movementMode = MovementMode.Translation;
        }
    }

    void ProcessRotation() {
        Quaternion q = transform.rotation;

        if (autoRotating) {
            AutoRotate(autoRotateTo);
        } else if (movementMode == MovementMode.Rotation) {
            StartRotating();
            StopRotating();
        }
    }

    private void ApplyRotation(float rotationThisFrame, Vector3 rotationVector) {
        rb.freezeRotation = true;
        transform.Rotate(rotationVector * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false;
    }

    private void AutoRotate(Quaternion rotateTo) {
        autoRotating = true;
        float x = transform.rotation.x;
        float y = transform.rotation.y;
        float z = transform.rotation.z;

        if ((Mathf.Abs(x - rotateTo.x) < .01f) && (Mathf.Abs(y - rotateTo.y) < .01f) && (Mathf.Abs(z - rotateTo.z) < .01f)) {
            transform.rotation = rotateTo;
            rb.freezeRotation = true;
            rb.freezeRotation = false;
            autoRotating = false;
            slerpTime = 0f;
            if (movementMode == MovementMode.Translation) {
                rb.useGravity = false;
            }
            return;
        } else {
            slerpTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, (Time.deltaTime + slerpTime) * autoRotateSpeed);
        }
    }

    private void StartRotating() {
        if (Input.GetKey(KeyCode.A)) {
            ApplyRotation(rotationSpeed, Vector3.forward);
            if (!rightBoosterParticles.isPlaying) {
                rightBoosterParticles.Play();
            }
        }
        else if (Input.GetKey(KeyCode.D)) {
            ApplyRotation(rotationSpeed, Vector3.back);
            if (!leftBoosterParticles.isPlaying) {
                leftBoosterParticles.Play();
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            ApplyRotation(rotationSpeed, into);

        }
        else if (Input.GetKey(KeyCode.S)) {
            ApplyRotation(rotationSpeed, outOf);
        }

    }

    private void StopRotating() {
        if (!Input.GetKey(KeyCode.A) && rightBoosterParticles.isPlaying) {
            rightBoosterParticles.Stop();
        }
        if (!Input.GetKey(KeyCode.D) && leftBoosterParticles.isPlaying) {
            leftBoosterParticles.Stop();
        }
    }

    private void StopThrusting() {
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        if (mainBoosterParticles.isPlaying) {
            mainBoosterParticles.Stop();
        }
    }

    private void StartThrusting() {
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainFartEngine);
        }
        if (!mainBoosterParticles.isPlaying) {
            mainBoosterParticles.Play();
        }
        rb.AddRelativeForce(Vector3.up * fartForce * Time.deltaTime);
    }
}
