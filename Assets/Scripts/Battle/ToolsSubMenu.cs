using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached as a component to ToolsSubMenu. Contains On_Click() methods, as well 
/// as general SubMenu fields and behaviors.
/// </summary>
public class ToolsSubMenu : BattleSubMenu
{
    // Mode is set when a Tool is selected.
    BattleMode mode = BattleMode.Tools_AutoCrossbow;

    public override BattleMode Mode { get { return mode; } }


    // AutoCrossbowText On_Click()
    public void Click_AutoCrossbow()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_AutoCrossbow;
        base.CloseSubMenu();
    }

    // AutoCrossbowText On_Click()
    public void Click_Drill()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_Drill;
        base.CloseSubMenu();
    }

    // AutoCrossbowText On_Click() (inactive)
    public void Click_Chainsaw()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_Chainsaw;
        base.CloseSubMenu();
    }

    // Click_Close and CloseSubMenu() included with SubMenu
}
