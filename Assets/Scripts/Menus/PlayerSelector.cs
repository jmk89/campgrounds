using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{

    private class Character {
        GameObject character;
        bool unlocked;

        public Character(GameObject character, bool unlocked)
        {
            this.character = character;
            this.unlocked = unlocked;
        }

        public void SetUnlocked(bool unlocked) {
            this.unlocked = unlocked;
        }

        public bool IsUnlocked() {
            return this.unlocked;
        }

        public void SetActive(bool active) {
            this.character.SetActive(active);
        }

        public bool isActive() {
            return this.character.activeInHierarchy;
        }
    }

    [SerializeField] GameObject[] inspectorCharacters;
    
    private List<Character> characters = new List<Character>();

    void Awake() {
        // DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject c in inspectorCharacters) {
            c.SetActive(false);
            characters.Add(new Character(c, false));
            Debug.Log("Added character");
        }
        characters.ElementAt(0).SetUnlocked(true);
        characters.ElementAt(0).SetActive(true);
 
        ChangeCharacter(0);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCharacterChange();
        
    }

    public void UnlockCharacter(int characterNumber) {
        Debug.Log("Trying to unlock");
        characters.ElementAt(characterNumber).SetUnlocked(true);
    }

    void ProcessCharacterChange() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CollisionHandler collisionHandler = player?.GetComponent<CollisionHandler>();

        if (!collisionHandler || collisionHandler.collidedWith != "Friendly") {
            return;
        }

        if (Input.GetKey(KeyCode.Alpha1)) {
            ChangeCharacter(0);
        } else if (Input.GetKey(KeyCode.Alpha2)){
            ChangeCharacter(1);
        }
    }

    void ChangeCharacter(int characterNumber) {
        if (characters.Count <= (characterNumber - 1)) {
            return;
        }
        if (!characters.ElementAt(characterNumber).IsUnlocked()) {
            return;
        }

        characters.ElementAt(characterNumber).SetActive(true);
        characters.ForEach(c => {
            if (c != characters.ElementAt(characterNumber)) {
                c.SetActive(false);
            }
        });
        
        // inspectorCharacters[characterNumber].SetActive(true);
        // List<GameObject> charsList = new List<GameObject>(inspectorCharacters);
        // charsList.RemoveAt(characterNumber);
        // charsList.ForEach(character => {
        //     character.SetActive(false);
        // });
    }
}

