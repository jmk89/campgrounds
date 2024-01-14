using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{

    [SerializeField] QuestData questData;
    [SerializeField] GameObject questManager;

    private void Start() {
        transform.Find("Prompt").gameObject.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G) && transform.Find("Prompt").gameObject.activeInHierarchy) {
            HandleInteractButton();
        }
    }

    private void HandleInteractButton() {
        this.questManager.GetComponent<QuestLog>().AddNewQuest(this.questData);
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") {
            return;
        }

        transform.Find("Prompt").gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag != "Player") {
            return;
        }

        transform.Find("Prompt").gameObject.SetActive(false);
    }
}
