using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int coinsBaselineValue = 2;
    [SerializeField] int coinsRandomizeRange = 1;
    
    public int GetValue() {
        return coinsBaselineValue + UnityEngine.Random.Range(0, coinsRandomizeRange);
    }
}
