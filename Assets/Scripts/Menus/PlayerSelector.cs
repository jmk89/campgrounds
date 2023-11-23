using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{

    private class Character {
        GameObject character;
        bool selected;
        bool unlocked;

        public Character(GameObject character, bool selected, bool unlocked)
        {
            this.character = character;
            this.selected = selected;
            this.unlocked = unlocked;
        }

        public GameObject GetObject() {
            return this.character;
        }

        public void SetSelected(bool selected) {
            this.selected = selected;
        }

        public bool IsSelected() {
            return this.selected;
        }

        public void SetUnlocked(bool unlocked) {
            this.unlocked = unlocked;
        }

        public bool IsUnlocked() {
            return this.unlocked;
        }
    }

    [SerializeField] GameObject[] inspectorCharacters;
    
    private List<Character> characters = new List<Character>();

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject c in inspectorCharacters) {
            characters.Add(new Character(c, false, false));
        }
        characters.ElementAt(0).SetUnlocked(true);
        characters.ElementAt(0).SetSelected(true);
 
        SelectInitialCharacter(0);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCharacterSelection();
    }

    public void UnlockCharacter(int characterNumber) {
        characters.ElementAt(characterNumber).SetUnlocked(true);
    }

    void ProcessCharacterSelection() {
        if (Input.GetKey(KeyCode.Alpha1)) {
            SelectCharacter(0);
        } else if (Input.GetKey(KeyCode.Alpha2)){
            SelectCharacter(1);
        }
    }

    void SelectCharacter(int characterNumber) {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CollisionHandler collisionHandler = player?.GetComponent<CollisionHandler>();
        if (!collisionHandler || collisionHandler.collidedWith != "Friendly") {
            Debug.Log("return collision");
            return;
        }
        if (characters.Count <= (characterNumber - 1)) {
            Debug.Log("return character count");
            return;
        }
        if (!characters.ElementAt(characterNumber).IsUnlocked()) {
            Debug.Log("return not unlocked");
            return;
        }

        Vector3 v = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        Destroy(player);

        Character character = characters.ElementAt(characterNumber);
        character.SetSelected(true);
        Instantiate(character.GetObject(), v, Quaternion.identity);
        characters.ForEach(c => {
            if (c != characters.ElementAt(characterNumber)) {
                c.SetSelected(false);
            }
        });
    }

    void SelectInitialCharacter(int characterNumber) {
        Character character = characters.ElementAt(characterNumber);
        character.SetSelected(true);
        character.SetUnlocked(true);
        character.GetObject().tag = "Player";

        GameObject launchPad = GameObject.FindGameObjectWithTag("Friendly");
        Vector3 launchPadPosition = launchPad.transform.position;

        Vector3 spawnPosition = new Vector3(launchPadPosition.x, -500, 0);
        GameObject spawned = Instantiate(character.GetObject(), spawnPosition, Quaternion.identity);

        float spawnedHeight = spawned.GetComponent<BoxCollider>().bounds.size.y;
        float topOfLaunchPad = launchPadPosition.y + (launchPad.GetComponent<BoxCollider>().bounds.size.y / 2) ;
        float ySpawnPosition = topOfLaunchPad + (spawnedHeight / 2);
        spawned.transform.position = new Vector3(spawnPosition.x, ySpawnPosition, 0);

        spawned.SetActive(true);
    }
}

