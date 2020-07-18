using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enumeration of the different hero actions. Used extensively in BattleManager.
/// </summary>
public enum BattleMode 
{
    none,
    Fight, // 1x physical damage, single target
    Defend, // reduces incoming damge by two (round down) until next turn
    Magic_Cure, // 0.75x magic healing, single target, ignores resistance?, 6mp
    Magic_Heal, // 0.75 magic healing to all
    Magic_Fireball, // 1x magic damage, all targets, 9mp
    Magic_Poison, // causes damage at the start of turn, 4x
    Blitz_Kick, // 0.75x physical damage, single target, plus 0.5 physical damage, random other target
    Blitz_AuraBolt, // 2.0x magic damage, single target
    Blitz_FireDance, // 0.5x magic damage, all targets, ignores resistance? (unused)
    Tools_AutoCrossbow,
    Tools_Drill,
    Tools_Chainsaw, // (unused)
    Runic,
    Steal,
    Item
}
