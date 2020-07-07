using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPCostComponent : MonoBehaviour
{
    //BattleMode mode = BattleMode.none;

    int mpRequirement = 5;

    Button parentButton;

    public void SetMode(BattleMode mode)
    {
        EventManager.AddListener_Battle_RefreshMPCosts(UpdateStatus);

        //this.mode = mode;
        
        int? mp = BattleAbilityData.Data[mode].MP;

        mpRequirement = mp != null ? (int)mp : 0;
        string text = mp == 0 ? "" : mp.ToString();
        gameObject.GetComponent<Text>().text = text;

        parentButton = gameObject.transform.parent.gameObject.GetComponent<Button>();
        
    }

    void UpdateStatus(int mpThreshhold)
    {
        if (mpRequirement > mpThreshhold)
        {
            parentButton.interactable = false;
            GetComponent<Text>().color = Color.red;
        }
        else
        {
            parentButton.interactable = true;
            GetComponent<Text>().color = Color.green;
        }

    }
}
