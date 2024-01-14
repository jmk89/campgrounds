using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Create New Quest", order = 1)]
public class Quest : ScriptableObject
{
    [Header("Quest Info")]
    public string questName;
    public string questDescription;
    
    [Header("Quest Rewards")]
    public int currencyReward;
    public GameObject spawnableMesh;

    [Header("Quest Objectives")]
    public string[] objectiveTasks;

    public Quest[] requiredQuests;
}
