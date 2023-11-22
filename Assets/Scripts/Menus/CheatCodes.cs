using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatCodes : MonoBehaviour
{

    [SerializeField] CollisionHandler ch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCheatCodes();
    }

    private void ProcessCheatCodes() {
        if (Input.GetKey(KeyCode.L)) {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int numberScenes = SceneManager.sceneCountInBuildSettings;
            currentSceneIndex++;
            if (currentSceneIndex >= numberScenes) {
                currentSceneIndex = 0;
            }
            SceneManager.LoadScene(currentSceneIndex);
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            CollisionHandler collisionHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CollisionHandler>();
            collisionHandler.toggleCollisionsEnabled();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            GameObject[] leaves = GameObject.FindGameObjectsWithTag("Leaf");
            foreach (GameObject leaf in leaves) {
                Destroy(leaf);
            }
        }
    }
}
