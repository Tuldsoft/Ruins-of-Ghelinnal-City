using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to SpellText, a Text object in the MagicSubMenu.
/// Contains a BattleMode to determine which spell to cast. 
/// Also contains the On_Click() method.
/// </summary>

public class SpellClick : MonoBehaviour
{
    
    // The spell to be cast upon click. Set upon creation via method.
    BattleMode mode = BattleMode.Magic_Cure;
    public BattleMode Mode { get { return mode; } }
    
    // The script attached to the magicSubMenu
    MagicSubMenu magicSubMenu;

    // Used when created to define the spell, and the menu (for calling "Click)
    public void SetMode(BattleMode mode, MagicSubMenu menu)
    {
        this.mode = mode;
        magicSubMenu = menu;
        gameObject.GetComponentInChildren<MPCostComponent>().SetMode(mode);

    }

    // SpellText On_Click()
    public void Click_Spell()
    {
        magicSubMenu.Click_Spell(mode);
    }
}
