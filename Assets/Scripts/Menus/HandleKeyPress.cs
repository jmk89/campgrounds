using UnityEngine.SceneManagement;
using UnityEngine;

public class HandleKeyPress : MonoBehaviour
{

    CameraManager cameraManagerScript;
    PlayerSelector playerSelectorScript;
    CheatCodes cheatCodesScript;

    void Start() {
        cameraManagerScript = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        playerSelectorScript = GameObject.FindGameObjectWithTag("PlayerSelector").GetComponent<PlayerSelector>();
    }

    void Update()
    {
        HandleCamera();
        HandlePlayerSelector();
        HandleCheatCodes();
        HandleQuit();
    }

    void HandleCamera() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            cameraManagerScript.CycleCameraType();
        }

        if (Input.GetMouseButtonDown(0)) {
            cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FreeLook);
            // cameraManagerScript.FreeCamActivate();
        } else if (Input.GetMouseButtonUp(0)) {
            cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FollowNormalFOV);
            // cameraManagerScript.FreeCamDeactivate();
        }
    }

    void HandlePlayerSelector() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            playerSelectorScript.SelectCharacter(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            playerSelectorScript.SelectCharacter(1);
        }
    }

    void HandleCheatCodes() {
        cheatCodesScript = GameObject.FindGameObjectWithTag("Player").GetComponent<CheatCodes>();
        if (Input.GetKeyDown(KeyCode.L)) {
            cheatCodesScript.LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            cheatCodesScript.ToggleCollisions();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            cheatCodesScript.CleanUpLeaves();
        }
    }

    void HandleQuit() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("you pooped your pants");
            Application.Quit();
        }
    }
}
