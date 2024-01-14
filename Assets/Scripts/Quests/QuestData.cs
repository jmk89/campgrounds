using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public Quest quest;
    public TMP_Text questLogColor;
    [Space]

    [Header("Quest Progress")]
    public bool inProgress;
    public bool hasCompleted;

    [Header("Objectives")]
    public GameObject[] objectives;
    public int currentObjectiveIndex;

    public QuestData(Quest quest) {
        this.quest = quest;
        inProgress = false;
        hasCompleted = false;
        currentObjectiveIndex = 0;
    }

    public Quest GetQuest() {
        return quest;
    }

    public string ShowCurrentObjective() {
        return quest.objectiveTasks[currentObjectiveIndex].ToString();
    }
}
