using Cinemachine;
using UnityEngine;

public class MovementRocket : MonoBehaviour
{
    [Header ("Movements")]
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float translationSpeed = 20000f;
    [SerializeField] float autoRotateSpeed = 3f;
    [SerializeField] float mouseSensitivity = 1f;

    [Header ("Continuous Thrust")]
    [SerializeField] float thrustForceContinuous = 3000f;
    [SerializeField] float thrustModeContinuousDrag = .5f;
    [SerializeField] float thrustModeContinuousMass = 1f;

    [Header ("Periodic Thrust")]
    [SerializeField] float thrustForcePeriodic = 5000f;
    [SerializeField] float thrustPeriodicWait = 1f;
    [SerializeField] float thrustModePeriodicDrag = 5f;
    [SerializeField] float thrustModePeriodicMass = 1f;
    
    [Header ("Particles & Sound")]
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;
    [SerializeField] AudioClip mainFartEngine;

    Rigidbody rb;
    AudioSource audioSource;
    Vector3 into = new Vector3(1, 0, 0);
    Vector3 outOf = new Vector3(-1, 0, 0);
    Vector2 mouseTurn;
    Quaternion zRotation = new Quaternion(0.7f, 0f, 0f, 0.7f);
    Quaternion autoRotateTo = Quaternion.identity;
    MovementMode movementMode = MovementMode.Rotation;
    ThrustMode thrustMode = ThrustMode.Periodic;
    bool autoRotating = false;
    float slerpTime = 0f;
    float thrustTimeSinceLast = 0f;
    enum MovementMode {Rotation, Translation};
    enum ThrustMode {Continuous, Periodic};

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        thrustTimeSinceLast = thrustPeriodicWait;
        SetDragAndMass(thrustModePeriodicDrag, thrustModePeriodicMass);
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
        if (movementMode == MovementMode.Translation) {
            ProcessTranslation();
        }
            ProcessRotation();
    }

    void ProcessTranslation() {
        Input.GetAxisRaw("Horizontal");

        float xValue = Input.GetAxisRaw("Horizontal") * Time.deltaTime * translationSpeed;
        float yValue = Input.GetAxisRaw("Vertical") * Time.deltaTime * translationSpeed;
        Debug.Log("translating x: " + xValue + ". translating y: " + yValue);
        transform.Translate(xValue, 0f, -yValue);
    }

    void ProcessThrust() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (thrustMode == ThrustMode.Continuous) {
                thrustMode = ThrustMode.Periodic;
                SetDragAndMass(thrustModePeriodicDrag, thrustModePeriodicMass);
            } else {
                thrustMode = ThrustMode.Continuous;
                SetDragAndMass(thrustModeContinuousDrag, thrustModeContinuousMass);
            }
        }

        if (Input.GetKey(KeyCode.Space) && thrustMode == ThrustMode.Continuous) {
            StartThrusting(thrustForceContinuous, ForceMode.Force);
        } else if (Input.GetKey(KeyCode.Space) && thrustMode == ThrustMode.Periodic && thrustTimeSinceLast >= thrustPeriodicWait) {
            StartThrusting(thrustForcePeriodic, ForceMode.Impulse);
            thrustTimeSinceLast = 0f;
        } else {
            StopThrusting();
        }

        thrustTimeSinceLast += Time.deltaTime;
    }

    void SetDragAndMass(float drag, float mass) {
        rb.drag = drag;
        rb.mass = mass;
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
            rb.useGravity = movementMode == MovementMode.Translation ? false : true;
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
        
        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            rb.freezeRotation = false;

            mouseTurn.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseTurn.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
            Quaternion desiredRotation = Quaternion.Euler(-mouseTurn.y, mouseTurn.x, 0);
            
            transform.rotation = Quaternion.Euler(-mouseTurn.y, mouseTurn.x, 0);
        } 
        
        if (Input.GetMouseButtonUp(1)) {
            Cursor.lockState = CursorLockMode.None;
            rb.freezeRotation = true;
            rb.freezeRotation = false;
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

    private void StartThrusting(float thrustForce, ForceMode forceMode) {
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(mainFartEngine);
        }
        if (!mainBoosterParticles.isPlaying) {
            mainBoosterParticles.Play();
        }

        Vector3 forceToAdd = Vector3.up * thrustForce;
        forceToAdd *= forceMode == ForceMode.Force ? Time.deltaTime : 1;
        rb.AddRelativeForce(forceToAdd, forceMode);
    }
}
