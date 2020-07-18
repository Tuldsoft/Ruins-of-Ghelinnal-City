using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attached to BattleMenuOptions in the Battle Scene. Makes cheat buttons appear if IsCheating.
public class CheatLoader : MonoBehaviour
{
    [SerializeField]
    GameObject cheatButton1 = null, cheatButton2 = null, cheatButton3 = null, cheatButton4 = null, cheatButton5 = null;

    private void Start()
    {
        if (cheatButton1 != null) // Deal 10x damage
        {
            cheatButton1.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton2 != null) // Increase HP Max
        {
            cheatButton2.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton3 != null) // Add potion pack
        {
            cheatButton3.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton4 != null) // Placeholder (is null)
        {
            cheatButton4.SetActive(BattleLoader.IsCheating); 
        }
        if (cheatButton5 != null) // Placeholder (is null)
        {
            cheatButton5.SetActive(BattleLoader.IsCheating); 
        }
    }


}


