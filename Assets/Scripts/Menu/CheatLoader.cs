using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatLoader : MonoBehaviour
{
    [SerializeField]
    GameObject cheatButton1 = null, cheatButton2 = null, cheatButton3 = null, cheatButton4 = null, cheatButton5 = null;

    private void Start()
    {
        if (cheatButton1 != null)
        {
            cheatButton1.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton2 != null)
        {
            cheatButton2.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton3 != null)
        {
            cheatButton3.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton4 != null)
        {
            cheatButton4.SetActive(BattleLoader.IsCheating);
        }
        if (cheatButton5 != null)
        {
            cheatButton5.SetActive(BattleLoader.IsCheating);
        }
    }


}


