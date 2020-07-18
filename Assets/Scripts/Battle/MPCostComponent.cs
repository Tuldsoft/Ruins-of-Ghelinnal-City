using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to a tiny text object called MPText, on a SpellText in the MagicSubMenu.
/// Shows how much mana a spell costs and turns red if there is not enough mana to cast.
/// Can also make SpellClick's gameObject non-interactable.
/// Configured and maintained by a SpellClick.
/// </summary>
public class MPCostComponent : MonoBehaviour
{
    
    int mpRequirement = 5; // how much the spell costs to cast

    Button parentButton; // a reference to its parent

    // Configures based on the spell it is attached to
    public void SetMode(BattleMode mode)
    {
        // Listens for a call to RefreshMPCosts (after a spell is cast)
        EventManager.AddListener_Battle_RefreshMPCosts(UpdateStatus);

        // Retrieve cost of the spell from BattleAbilityData
        int? mp = BattleAbilityData.Data[mode].MP;

        // if ability.mp is null, make the cost 0
        mpRequirement = mp != null ? (int)mp : 0;
        // if the cost is 0, display nothing, otherwise display the cost
        string text = mp == 0 ? "" : mp.ToString();
        gameObject.GetComponent<Text>().text = text;

        // set parent for interactable
        parentButton = gameObject.transform.parent.gameObject.GetComponent<Button>();
    }

    /// <summary>
    /// Invoked any time a spell is cast
    /// </summary>
    /// <param name="mpThreshhold">The amount of mp the current hero has remaining.</param>
    void UpdateStatus(int mpThreshhold)
    {
        // Make the parent SpellText non-interactable (greyed out) if the mana is insufficient
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
