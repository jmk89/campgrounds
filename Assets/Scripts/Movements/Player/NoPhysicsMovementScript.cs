using UnityEngine;

public class NoPhysicsMovementScript : MonoBehaviour
{
    [Header ("Movements")]
    [SerializeField] float rotationSpeed = 80f;
    [SerializeField] float translationSpeed = 50f;
    [SerializeField] float autoRotateSpeed = 1f;
    [SerializeField] float mouseSensitivity = 300f;

    [Header ("Continuous Thrust")]
    [SerializeField] float thrustContinuous = 10000f;
    [SerializeField] float boost = 30000f;
    [SerializeField] float boostSeconds = 3f;
    [SerializeField] float continuousDrag = 2f;
    [SerializeField] float continuousMass = 2f;

    [Header ("Periodic Thrust")]
    [SerializeField] float thrustPeriodic = 80f;
    [SerializeField] float thrustPause = 0.75f;
    [SerializeField] float periodicDrag = 5f;
    [SerializeField] float periodicMass = 1f;
    
    [Header ("Particles & Sound")]
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;
    [SerializeField] AudioClip mainFartEngine;

    Rigidbody rb;
    AudioSource audioSource;
    Quaternion zRotation = new Quaternion(0.7f, 0f, 0f, 0.7f);
    Quaternion autoRotateTo = Quaternion.identity;
    MovementMode movementMode = MovementMode.Rotation;
    ThrustMode thrustMode = ThrustMode.Continuous;
    bool initialZMovementInitiated = false;
    bool autoRotating = false;
    float slerpTime = 0f;
    float thrustTimeSinceLast = 0f;
    float boostTime = 0f;
    bool processingBoost = false;
    enum MovementMode {Rotation, Translation};
    enum ThrustMode {Continuous, Periodic};
    CameraManager cameraManagerScript;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        thrustTimeSinceLast = thrustPause;
        SetDragAndMass(continuousDrag, continuousMass);
        cameraManagerScript = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
    }

    void Update() {
        ProcessThrustMode();
        ProcessThrust();
        ProcessBoost();
        ProcessAutoRotation();
        ProcessMovements();
    }

    void ProcessMovements() {
        if (movementMode == MovementMode.Translation) {
            ProcessTranslation();
        } else {
            ProcessRotation();
        }
    }

    void ProcessThrustMode() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (thrustMode == ThrustMode.Continuous) {
                thrustMode = ThrustMode.Periodic;
                SetDragAndMass(periodicDrag, periodicMass);
            } else {
                thrustMode = ThrustMode.Continuous;
                SetDragAndMass(continuousDrag, continuousMass);
            }
        }
    }

    void ProcessTranslation() {
        float xValue = Input.GetAxisRaw("Horizontal") * Time.deltaTime * translationSpeed;
        float yValue = Input.GetAxisRaw("Vertical") * Time.deltaTime * translationSpeed;
        transform.Translate(xValue, 0f, -yValue);
    }

    void ApplyBoost() {
        if (boostTime > boostSeconds) {
            boostTime = 0f;
            processingBoost = false;
            return;
        }
        boostTime += Time.deltaTime;
        rb.AddRelativeForce(Vector3.up * boost * Time.deltaTime);
    }

    void ProcessBoost() {
        if (Input.GetKeyDown(KeyCode.F) && thrustMode == ThrustMode.Continuous && boostTime == 0) {
            processingBoost = true;
        }

        if (processingBoost) {
            ApplyBoost();
        }
    }

    void ProcessThrust() {
        if (Input.GetKey(KeyCode.Space) && thrustMode == ThrustMode.Continuous) {
            StartThrusting(thrustContinuous, ForceMode.Force);
        } else if (Input.GetKey(KeyCode.Space) && thrustMode == ThrustMode.Periodic && thrustTimeSinceLast >= thrustPause) {
            StartThrusting(thrustPeriodic, ForceMode.Impulse);
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
        } else if (Input.GetKeyDown(KeyCode.R)) {
            transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0f, transform.rotation.w);
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

    private void ApplyWorldRotation(float rotationThisFrame, Vector3 rotationVector) {
        transform.Rotate(rotationVector * rotationThisFrame * Time.deltaTime, Space.World);
    }

    private void ApplyRotation(float rotationThisFrame, Vector3 rotationVector) {
        transform.Rotate(rotationVector * rotationThisFrame * Time.deltaTime);
    }

    private void AutoRotate(Quaternion rotateTo) {
        autoRotating = true;
        float x = transform.rotation.x;
        float y = transform.rotation.y;
        float z = transform.rotation.z;

        if ((Mathf.Abs(x - rotateTo.x) < .01f) && (Mathf.Abs(y - rotateTo.y) < .01f) && (Mathf.Abs(z - rotateTo.z) < .01f)) {
            transform.rotation = rotateTo;
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
            ApplyWorldRotation(rotationSpeed, Vector3.down);
            if (!rightBoosterParticles.isPlaying) {
                rightBoosterParticles.Play();
            }
            
        }
        else if (Input.GetKey(KeyCode.D)) {
            ApplyWorldRotation(rotationSpeed, Vector3.up);
            if (!leftBoosterParticles.isPlaying) {
                leftBoosterParticles.Play();
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            if (!initialZMovementInitiated) {
                cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FollowNormalFOV);
                initialZMovementInitiated = true;
            }
            ApplyRotation(rotationSpeed, Vector3.left);
        }
        else if (Input.GetKey(KeyCode.S)) {
            if (!initialZMovementInitiated) {
                cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FollowNormalFOV);
                initialZMovementInitiated = true;
            }
            ApplyRotation(rotationSpeed, Vector3.right);
        }
        
        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FollowNormalFOV);
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            ApplyWorldRotation(mouseSensitivity, new Vector3(0f, xAxis, 0f));
            ApplyRotation(mouseSensitivity, new Vector3(-yAxis, 0f, 0f));
        } else if (Input.GetMouseButtonUp(1)) {
            Cursor.lockState = CursorLockMode.None;
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
