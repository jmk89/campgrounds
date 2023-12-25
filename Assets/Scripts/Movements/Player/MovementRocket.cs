using UnityEngine;

public class MovementRocket : MonoBehaviour
{
    [Header ("Movements")]
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float translationSpeed = 20000f;
    [SerializeField] float autoRotateSpeed = 3f;
    [SerializeField] float mouseSensitivity = 1f;

    [Header ("Continuous Thrust")]
    [SerializeField] float thrustContinuous = 3000f;
    [SerializeField] float continuousDrag = .5f;
    [SerializeField] float continuousMass = 1f;

    [Header ("Periodic Thrust")]
    [SerializeField] float thrustPeriodic = 5000f;
    [SerializeField] float thrustPause = 1f;
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
    bool autoRotating = false;
    float slerpTime = 0f;
    float thrustTimeSinceLast = 0f;
    enum MovementMode {Rotation, Translation};
    enum ThrustMode {Continuous, Periodic};
    Vector3 rightPosition;
    Vector3 leftPosition;
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

    private void ApplyTorqueFromPointsVectors(float rotationThisFrame, float modifier) {
        rightPosition = transform.Find("Steering").Find("RightPoint").transform.position;
        leftPosition = transform.Find("Steering").Find("LeftPoint").transform.position;
        // transform.Find("Steering").Find("LeftPoint").transform.position = new Vector3(leftPosition.x, rightPosition.y, leftPosition.z);
        transform.Find("Steering").Find("LeftPoint").transform.position = new Vector3(leftPosition.x, rightPosition.y, leftPosition.z);

        Vector3 newVector = (rightPosition - leftPosition).normalized * modifier;
        newVector.y = 0f;
        rb.AddTorque(newVector * rotationThisFrame * Time.deltaTime, ForceMode.Impulse);
    }

    private void ApplyTorque(float rotationThisFrame, Vector3 rotationVector) {
        rightPosition = transform.Find("Steering").Find("RightPoint").transform.position;
        leftPosition = transform.Find("Steering").Find("LeftPoint").transform.position;
        // transform.Find("Steering").Find("LeftPoint").transform.position = new Vector3(leftPosition.x, rightPosition.y, leftPosition.z);
        transform.Find("Steering").Find("LeftPoint").transform.position = new Vector3(leftPosition.x, rightPosition.y, leftPosition.z);

        rb.AddTorque(rotationVector * rotationThisFrame * Time.deltaTime, ForceMode.Impulse);
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
            // rb.useGravity = movementMode == MovementMode.Translation ? false : true;
            return;
        } else {
            slerpTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, (Time.deltaTime + slerpTime) * autoRotateSpeed);
        }
    }

    private void StartRotating() {
        if (Input.GetKey(KeyCode.A)) {
            ApplyTorque(rotationSpeed, Vector3.down);
            if (!rightBoosterParticles.isPlaying) {
                rightBoosterParticles.Play();
            }
            
        }
        else if (Input.GetKey(KeyCode.D)) {
            ApplyTorque(rotationSpeed, Vector3.up);
            if (!leftBoosterParticles.isPlaying) {
                leftBoosterParticles.Play();
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            ApplyTorqueFromPointsVectors(rotationSpeed, -1f);
        }
        else if (Input.GetKey(KeyCode.S)) {
            ApplyTorqueFromPointsVectors(rotationSpeed, 1f);
        }
        
        if (Input.GetMouseButton(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FollowNormalFOV);
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            ApplyTorque(mouseSensitivity, new Vector3(0f, xAxis, 0f));
            ApplyTorqueFromPointsVectors(mouseSensitivity, -yAxis);
        } 
        
        if (Input.GetMouseButtonUp(1)) {
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
