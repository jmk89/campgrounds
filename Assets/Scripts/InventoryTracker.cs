using UnityEngine;
using UnityEngine.UI;

public class InventoryTracker : MonoBehaviour
{
    [SerializeField] int totalCoins = 0;
    [SerializeField] int boostsAvailable = 0;
    [SerializeField] Text coinText;

    private static GameObject instance;
    

    void Awake() {
        DontDestroyOnLoad(this);
	    if (instance == null) {
		    instance = gameObject;
	    } else {
		    Destroy(gameObject);
	    }
    }
    
    public void UpdateCoins(int updateAmount) {
        totalCoins += updateAmount;
        Debug.Log("InventoryTracker totalCoins " + totalCoins);
        coinText.text = "Coins: " + totalCoins;
    }
    
    public int GetCoins() {
        return totalCoins;
    }

    public void UpdateBoostsAvailable(int updateAmount) {
        boostsAvailable += updateAmount;
    }

    public int GetBoostsAvailable() {
        return boostsAvailable;
    }

}
