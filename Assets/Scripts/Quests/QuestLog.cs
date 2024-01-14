using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestLog : MonoBehaviour
{

    [Header("Quest System base objects")]
    [SerializeField][Tooltip("The quests that can populate the questlog.")]
    List<QuestData> questlog;

    [SerializeField][Tooltip("The quest log object that pops up with all quests")]
    GameObject questLogParent;

    [Space]
    [Header("Quest UI Components")]
    [SerializeField][Tooltip("The gameplay objective outside the quest menu")]
    TMP_Text objectiveInScreen;

    [Header("Sounds")]
    [SerializeField] AudioSource questCompleteSounds;

    private TMP_Text[] questListArray;

    private GameObject questPage;
    public TMP_Text centerPopup;
    
    void Awake() {

        questListArray = new TMP_Text[questlog.Count];
        // questPage = transform.GetChild(2).gameObject;

        for (int i = 0; i < questlog.Count; i++) {
            PopulateQuestLog(i);
            // UpdateQuestColor(i);
        }
    }

    public void ShowObjectiveTracking(Quest quest, int i) {
        objectiveInScreen.text = quest.name + " - " + questlog[i].ShowCurrentObjective();
    }

    public void UpdateQuestColor(int i) {
        if(questlog[i].inProgress) {
            questlog[i].questLogColor.color = Color.yellow;
        } else if (!questlog[i].inProgress && !questlog[i].hasCompleted) {
            questlog[i].questLogColor.color = Color.grey;
        } else if (questlog[i].hasCompleted) {
            questlog[i].questLogColor.color = Color.green;
        }
    }

    public void AddNewQuest(QuestData newQuestData) {
        newQuestData.inProgress = true;
        questlog.Add(newQuestData);
    }

    public void ObjectiveCompleted(QuestData questData) {
        questlog.Find(q => q.quest.name == questData.quest.name).hasCompleted = true;
        questlog.Find(q => q.quest.name == questData.quest.name).inProgress = false;
    }

    private void PopulateQuestLog(int i) {
        GameObject questList = Instantiate(questlog[i].quest.spawnableMesh);
        questList.transform.parent = questLogParent.transform;
        // TMP_Text spawnQuest = questList.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        // spawnQuest.text = questlog[i].quest.name;
        // spawnQuest.color = Color.magenta;

        // questListArray[i] = spawnQuest;
        // questlog[i].questLogColor = spawnQuest;
    }

    public void UpdateQuestProgress(Quest quest) {
        //questPage.SetActive(false);

        for (int i = 0; i < questlog.Count; i++) {
            if (questlog[i].quest == quest) {
                Debug.Log($"Quest Name: {quest.questName}");
                questlog[i].currentObjectiveIndex++;
                ShowObjectiveTracking(quest, i);
            }
        }
    }

    public void StartQuest(Quest quest) {
        QuestData questData = questlog.ToList().Find(questDataFromLog => questDataFromLog.quest == quest);
        questData.inProgress = true;
        int indexOf = questlog.ToList().IndexOf(questData);
        // UpdateQuestColor(indexOf);
        return;
    }

}
