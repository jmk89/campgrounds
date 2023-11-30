using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{

    [SerializeField] float loadDelay = 2f;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip crash;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    public bool collisionsEnabled = true;
    public string collidedWith = "";

    AudioSource audioSource;
    bool isTransitioning = false;
    int gateToRemove = 0;
    GameObject inventoryManager;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager");
    }

    void Update()
    {
        ProcessGateRemoval();
    }

    void OnCollisionEnter(Collision other) {
        if (isTransitioning || !collisionsEnabled) {
            return;
        }
        isTransitioning = true;
        string tag = other.gameObject.tag;
        
        switch (tag) {
            case "Friendly":
                isTransitioning = false;
                break;
            case "Finish":
                StartLoadNextScene();
                break;
            case "Gate1Button":
                isTransitioning = false;
                RemoveGate(1);
                break;
            case "Gate2Button":
                isTransitioning = false;
                RemoveGate(2);
                break;
            case "Coin":
                UpdateCoins(other.gameObject.GetComponent<Coin>().GetValue());
                break;
            default:
                StartCrashSequence();
                break;
        }
        if (tag.Length > 0) {
            collidedWith = tag;
        }
    }

    void OnTriggerEnter(Collider other) {
        string tag = other.gameObject.tag;
        
        switch (tag) {
            case "Coin":
                UpdateCoins(other.gameObject.GetComponent<Coin>().GetValue());
                other.gameObject.SetActive(false);
                break;
            case "CharacterUnlock":
                // UnlockCharacter(other.gameObject.GetComponent);
                break;
        }
    }

    void OnCollisionExit(Collision other) {
        collidedWith = "";
    }

    public void toggleCollisionsEnabled() {
        collisionsEnabled = !collisionsEnabled;
    }

    void UnlockCharacter() {
        // GameObject.FindGameObjectWithTag("PlayerSelector").GetComponent<PlayerSelector>().UnlockCharacter(characterToUnlock);
    }

    void UpdateCoins(int updateAmount) {
        inventoryManager.GetComponent<InventoryTracker>().UpdateCoins(updateAmount);
    }

    void RemoveGate(int gateNumber) {
        GameObject gate = GameObject.FindGameObjectsWithTag("Gate" + gateNumber)[0];
        ParticleSystem[] particles = gate.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particles) {
            particleSystem.Play();
        }
        gateToRemove = gateNumber;
        List<Collider> colliders = gate.GetComponentsInChildren<Collider>().ToList<Collider>();
        foreach (Collider c in colliders) {
            c.enabled = false;
        }
    }

    private void ProcessGateRemoval() {
        if (gateToRemove > 0) {
            GameObject gate = GameObject.FindGameObjectsWithTag("Gate" + gateToRemove)[0];
            List<Renderer> renderers = gate.GetComponentsInChildren<Renderer>().ToList<Renderer>();
            renderers.RemoveAll(item => item.material.color.a <= 0);

            if (renderers.Count == 0) {
                GameObject.FindGameObjectsWithTag("Gate" + gateToRemove)[0].SetActive(false);
                gateToRemove = 0;
            } else {
                foreach (Renderer r in renderers) {
                    Color c = r.material.color;
                    float fadeAmount = c.a - (.5f * Time.deltaTime);
                    r.material.color = new Color(c.r, c.b, c.g, fadeAmount);
                }
            }
        }
    }

    void cleanUpLeaves() {
        GameObject[] leaves = GameObject.FindGameObjectsWithTag("Leaf");
        foreach (GameObject leaf in leaves) {
            Destroy(leaf);
        }
    }

    void StartCrashSequence() {
        audioSource.Stop();
        GetComponent<MovementRocket>().enabled = false;
        audioSource.PlayOneShot(crash);
        crashParticles.Play();
        Invoke("ReloadLevel", loadDelay);
    }

    void ReloadLevel() {
        cleanUpLeaves();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        isTransitioning = false;
    }

    void StartLoadNextScene() {
        audioSource.Stop();
        GetComponent<MovementRocket>().enabled = false;
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", loadDelay);
    }

    void LoadNextLevel() {
        cleanUpLeaves();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int numberScenes = SceneManager.sceneCountInBuildSettings;
        currentSceneIndex++;
        if (currentSceneIndex >= numberScenes) {
            currentSceneIndex = 0;
        }
        SceneManager.LoadScene(currentSceneIndex);
        isTransitioning = false;
    }

}
