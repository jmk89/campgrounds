using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    [SerializeField] GameObject[] characters;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCharacter(0);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCharacterChange();
        
    }

    void ProcessCharacterChange() {
        GameObject.FindGameObjectWithTag("Player");

        if (Input.GetKey(KeyCode.Alpha1)) {
            ChangeCharacter(0);
        } else if (Input.GetKey(KeyCode.Alpha2)){
            ChangeCharacter(1);
        }
    }

    void ChangeCharacter(int characterNumber) {
        if (!characters[characterNumber]) {
            return;
        }
        
        characters[characterNumber].SetActive(true);
        List<GameObject> charsList = new List<GameObject>(characters);
        charsList.RemoveAt(characterNumber);
        charsList.ForEach(character => {
            character.SetActive(false);
        });
    }
}
