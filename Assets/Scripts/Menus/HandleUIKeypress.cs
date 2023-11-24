using UnityEngine;

public class HandleUIKeypress : MonoBehaviour
{

    private static GameObject instance;
    void Awake() {
	    if (instance == null) {
            DontDestroyOnLoad(this);
		    instance = gameObject;
	    } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        HandleKeypress();
    }

    void HandleKeypress() {
        if (Input.GetKeyDown(KeyCode.B)) {
            HandleBackpack();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            HandleShop();
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            HandleHideUI();
        }
    }

    void HandleBackpack() {

    }

    void HandleShop() {

    }

    void HandleHideUI() {
        
    }
}
