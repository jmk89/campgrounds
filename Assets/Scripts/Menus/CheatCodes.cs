using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatCodes : MonoBehaviour
{
    public void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int numberScenes = SceneManager.sceneCountInBuildSettings;
            currentSceneIndex++;
            if (currentSceneIndex >= numberScenes) {
                currentSceneIndex = 0;
            }
            SceneManager.LoadScene(currentSceneIndex);
    }

    public void ToggleCollisions() {
        CollisionHandler collisionHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CollisionHandler>();
        collisionHandler.toggleCollisionsEnabled();
    }

    public void CleanUpLeaves() {
        GameObject[] leaves = GameObject.FindGameObjectsWithTag("Leaf");
        foreach (GameObject leaf in leaves) {
            Destroy(leaf);
        }
    }
}
