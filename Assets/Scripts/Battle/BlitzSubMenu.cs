using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzSubMenu : BattleSubMenu
{
    BattleMode mode = BattleMode.Blitz_Kick;

    public override BattleMode Mode { get { return mode; } }

    public void Click_Kick()
    {
        AudioManager.Chirp();
        mode = BattleMode.Blitz_Kick;
        base.CloseSubMenu();
    }

    public void Click_AuraBolt()
    {
        AudioManager.Chirp();
        mode = BattleMode.Blitz_AuraBolt;
        base.CloseSubMenu();
    }

    public void Click_FireDance()
    {
        AudioManager.Chirp();
        mode = BattleMode.Blitz_FireDance;
        base.CloseSubMenu();
    }
}
