using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsSubMenu : BattleSubMenu
{
    BattleMode mode = BattleMode.Tools_AutoCrossbow;

    public override BattleMode Mode { get { return mode; } }

    public void Click_AutoCrossbow()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_AutoCrossbow;
        base.CloseSubMenu();
    }

    public void Click_Drill()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_Drill;
        base.CloseSubMenu();
    }

    public void Click_Chainsaw()
    {
        AudioManager.Chirp();
        mode = BattleMode.Tools_Chainsaw;
        base.CloseSubMenu();
    }
}
