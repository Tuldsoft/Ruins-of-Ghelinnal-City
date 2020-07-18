using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each ability used in Battle is defined as a BattleAbility. Its properties
/// determine the behaviors executed in battle. These are read-only, except through
/// the BattleAbility constructor. BattleAbilitys are created in BattleAbilityData.
/// </summary>
public class BattleAbility
{
    #region Properties
    public BattleMode Name { get; } // Name of the ability
    public bool IsPhysical { get; } // Physical (true) or Magical (false)
    public int? MP { get; } // MP required to use the ability (null if N/A)
    public bool NoReduction { get; } // whether the damage of the ability is reduced by def/res
    public float Modifier { get; } // damage multiplier (1 = 100% damage)
    public bool NoMiss { get; } // bypasses miss chance
    public int? HitNumOverride { get; } // if null, the number of hits are calculated normally, otherwise this specifies the number of hits.
    public bool NoCrit { get; } // bypasses crit chance
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor for BattleAbility
    /// </summary>
    /// <param name="name">Ability name as a BattleMode</param>
    /// <param name="isPhysical">Default true</param>
    /// <param name="mp">Default null</param>
    /// <param name="noReduction">Default false (ability can be reduced)</param>
    /// <param name="modifier">Damage multiplier (default 1.0)</param>
    /// <param name="noMiss">Default false (ability can be missed)</param>
    /// <param name="hitNumOverride">Default null (no num hits specified)</param>
    /// <param name="noCrit">Default false (ability can crit)</param>
    public BattleAbility( BattleMode name = BattleMode.none, bool isPhysical = true, int? mp = null,
        bool noReduction = false, float modifier = 1, 
        bool noMiss = false, int? hitNumOverride = null, bool noCrit = false)
    {
        Name = name;
        IsPhysical = isPhysical;
        MP = mp;
        NoReduction = noReduction;
        Modifier = modifier;
        NoMiss = noMiss;
        HitNumOverride = hitNumOverride;
        NoCrit = noCrit;

    }
    #endregion
}
