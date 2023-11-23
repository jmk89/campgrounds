using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCharacter : MonoBehaviour
{
    [SerializeField] int characterToUnlock = 0;
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
        Unlock(characterToUnlock);
    }

    void Unlock(int characterToUnlock) {
        Debug.Log("Unlocking");
        GameObject.FindGameObjectWithTag("PlayerSelector").GetComponent<PlayerSelector>().UnlockCharacter(characterToUnlock);
    } 
}
