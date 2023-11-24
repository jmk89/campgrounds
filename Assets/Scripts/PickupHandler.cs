using System;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] int characterToUnlock = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != "Player") {
            return;
        }

        switch(gameObject.tag) {
            case "CharacterUnlock":
                UnlockCharacter();
                break;
        }
        gameObject.SetActive(false);
    }

    void UnlockCharacter() {
        GameObject.FindGameObjectWithTag("PlayerSelector").GetComponent<PlayerSelector>().UnlockCharacter(characterToUnlock);
    }
}
