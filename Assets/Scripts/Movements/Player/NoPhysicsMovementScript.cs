using System;
using UnityEngine;

public class NoPhysicsMovementScript : MonoBehaviour
{
    [Header ("Movements")]
    [SerializeField] float rotationSpeed = 80f;
    [SerializeField] float translationSpeed = 50f;
    [SerializeField] float autoRotateSpeed = 1f;
    [SerializeField] float mouseSensitivity = 300f;
    [Header ("Up and Down")]
    [SerializeField] float downBrakes = 2000;
    [SerializeField] float upDraft = 5000f;
    [SerializeField] float upDraftSeconds = 1.5f;
    [SerializeField] float initialUpDraft = 10000f;


    [Header ("Continuous Thrust")]
    [SerializeField] float thrustContinuous = 8000f;
    [SerializeField] float boost = 30000f;
    [SerializeField] float boostSeconds = 3f;
    [SerializeField] float continuousDrag = 2f;
    [SerializeField] float continuousMass = 2f;

    
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
    bool initialZMovementInitiated = false;
    bool initialUpdraftApplied = false;
    bool initialUpdraftCompleted = false;
    bool autoThrustToggled = false;
    bool thrustEnabled = true;
    bool autoRotating = false;
    float slerpTime = 0f;
    float upDraftTime = 0f;
    float boostTime = 0f;
    bool processingBoost = false;
    bool processingUpdraft = false;
    enum MovementMode {Rotation, Translation};
    CameraManager cameraManagerScript;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        SetDragAndMass(continuousDrag, continuousMass);
        cameraManagerScript = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
    }

    void Update() {
        ProcessThrust();
        ProcessBoost();
        ProcessMovements();
        ProcessBrakes();
        ProcessUpdraft();
    }

    void ProcessBrakes() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            rb.AddForce(Vector3.down * downBrakes * Time.deltaTime, ForceMode.Force);
        }
    }

    void ProcessUpdraft() {
        float updraftForceToUse = initialUpdraftCompleted ? upDraft : initialUpDraft;
        if (Input.GetKeyDown(KeyCode.E) && upDraftTime == 0) {
            processingUpdraft = true;
            if (!initialUpdraftApplied) {
                initialUpdraftApplied = true;
                updraftForceToUse = initialUpDraft;
            } else {
                updraftForceToUse = upDraft;
            }            
        }
        
        if (processingUpdraft) {
            ApplyUpdraft(updraftForceToUse);
        }
    }

    void ApplyUpdraft(float updraftForce) {
        if (upDraftTime > upDraftSeconds) {
            upDraftTime = 0;
            processingUpdraft = false;
            if (updraftForce == initialUpDraft) {
                initialUpdraftCompleted = true;
            }
            return;
        }
        upDraftTime += Time.deltaTime;
        rb.AddForce(Vector3.up * updraftForce * Time.deltaTime);
    }

    void ProcessMovements() {
        if (movementMode == MovementMode.Translation) {
            ProcessTranslation();
        } else {
            ProcessRotation();
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
            cameraManagerScript.ApplyCameraShake(false);
            return;
        }
        boostTime += Time.deltaTime;
        rb.AddRelativeForce(Vector3.forward * boost * Time.deltaTime);
    }

    void ProcessBoost() {
        if (Input.GetKeyDown(KeyCode.F) && boostTime == 0) {
            processingBoost = true;
            cameraManagerScript.ApplyCameraShake(true);
        }

        if (processingBoost) {
            ApplyBoost();
        }
    }

    void ProcessThrust() {
        if (!thrustEnabled) {
            StopThrusting();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            autoThrustToggled = !autoThrustToggled;
        }

        if (autoThrustToggled || Input.GetKey(KeyCode.Space) || (Input.GetMouseButton(0) && Input.GetMouseButton(1))) {
            StartThrusting(thrustContinuous, ForceMode.Force);
        } else {
            StopThrusting();
        }
    }

    void SetDragAndMass(float drag, float mass) {
        rb.drag = drag;
        rb.mass = mass;
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
            return;
        } else {
            slerpTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, (Time.deltaTime + slerpTime) * autoRotateSpeed);
        }
    }

    private void StartRotating() {
        if (autoRotating) {
            return;
        }

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
            // audioSource.PlayOneShot(mainFartEngine);
        }
        if (!mainBoosterParticles.isPlaying) {
            mainBoosterParticles.Play();
        }

        Vector3 forceToAdd = Vector3.forward * thrustForce * Time.deltaTime;
        rb.AddRelativeForce(forceToAdd, forceMode);
    }

    public void ThrustEnabled(bool enable) {
        thrustEnabled = enable;
    }

    public void SetAutoRotateTo(Vector3 rotation) {
        autoRotateTo = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        autoRotating = true;
    }
}
