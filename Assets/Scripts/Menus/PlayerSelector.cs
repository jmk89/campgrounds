﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private static GameObject instance;
    private static int selectedCharacterIndex = 0;

    void Awake() {
        DontDestroyOnLoad(this);
	    if (instance == null) {
		    instance = gameObject;
	    } else {
		    Destroy(gameObject);
	    }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject c in inspectorCharacters) {
            characters.Add(new Character(c, false, false));
        }
        characters.ElementAt(0).SetUnlocked(true);
        characters.ElementAt(0).SetSelected(true);
        SpawnCharacterAtLaunchPad(0);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        if (!character) {
            SpawnCharacterAtLaunchPad(selectedCharacterIndex);
        }
    }

    public void UnlockCharacter(int characterNumber) {
        characters.ElementAt(characterNumber).SetUnlocked(true);
    }

    public void SelectCharacter(int characterNumber) {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CollisionHandler collisionHandler = player?.GetComponent<CollisionHandler>();
        if (!collisionHandler || collisionHandler.collidedWith != "Friendly") {
            Debug.Log("collision");
            return;
        }
        if (characters.Count <= (characterNumber - 1)) {
            Debug.Log("count");

            return;
        }
        if (!characters.ElementAt(characterNumber).IsUnlocked()) {
            Debug.Log("not unlocked");

            return;
        }

        Destroy(player);
        Character character = characters.ElementAt(characterNumber);
        character.SetSelected(true);
        selectedCharacterIndex = characterNumber;
        characters.ForEach(c => {
            if (c != characters.ElementAt(characterNumber)) {
                c.SetSelected(false);
            }
        });

        SpawnCharacterAtLaunchPad(characterNumber);
    }

    void SpawnCharacterAtLaunchPad(int characterNumber) {
        
        Character character = characters.ElementAt(characterNumber);
        GameObject launchPad = GameObject.FindGameObjectWithTag("Friendly");
        Vector3 launchPadPosition = launchPad.transform.position;
        Vector3 spawnPosition = new Vector3(launchPadPosition.x, -500, launchPadPosition.z);
        GameObject spawned = Instantiate(character.GetObject(), spawnPosition, Quaternion.identity);

        float spawnedHeight = spawned.GetComponent<BoxCollider>().bounds.size.y;
        float topOfLaunchPad = launchPadPosition.y + (launchPad.GetComponent<BoxCollider>().bounds.size.y / 2) ;
        float ySpawnPosition = topOfLaunchPad + (spawnedHeight / 2);
        
        spawned.transform.position = new Vector3(spawnPosition.x, ySpawnPosition, launchPadPosition.z);
        spawned.tag = "Player";
        spawned.SetActive(true);
        
        CameraManager cameraManagerScript = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        cameraManagerScript.UpdateAllCameraTargets(spawned);
        cameraManagerScript.UpdateActiveCamera(CameraManager.CameraType.FramingTransposer);        
    }
}

