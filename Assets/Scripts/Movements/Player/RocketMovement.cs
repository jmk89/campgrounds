using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float fartForce = 1f;
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;
    
    [SerializeField] AudioClip mainFartEngine;

    Rigidbody rb;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        ProcessThrust();
        ProcessRotation();
    }

    void ProcessThrust() {
        if (Input.GetKey(KeyCode.Space)) {
            StartThrusting();
        } else {
            StopThrusting();
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

    private void ApplyRotation(float rotationThisFrame) {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        rb.freezeRotation = false;
    }

    void ProcessRotation() {
        StartRotating();
        StopRotating();
    }

    private void StartRotating() {
        if (Input.GetKey(KeyCode.A)) {
            ApplyRotation(rotationSpeed);
            if (!rightBoosterParticles.isPlaying) {
                rightBoosterParticles.Play();
            }
        }
        else if (Input.GetKey(KeyCode.D)) {
            ApplyRotation(-rotationSpeed);
            if (!leftBoosterParticles.isPlaying) {
                leftBoosterParticles.Play();
            }
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
}
