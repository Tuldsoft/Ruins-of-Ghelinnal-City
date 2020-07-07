using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellClick : MonoBehaviour
{
    BattleMode mode = BattleMode.Magic_Cure;
    public BattleMode Mode { get { return mode; } }
    MagicSubMenu magicSubMenu;

    public void SetMode(BattleMode mode, MagicSubMenu menu)
    {
        this.mode = mode;
        magicSubMenu = menu;
        gameObject.GetComponentInChildren<MPCostComponent>().SetMode(mode);

    }

    public void Click_Spell()
    {
        magicSubMenu.Click_Spell(mode);
    }
}
