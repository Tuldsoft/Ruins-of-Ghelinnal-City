using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbility
{
    public BattleMode Name { get; }
    public bool IsPhysical { get; }
    public int? MP { get; }
    public bool NoReduction { get; }
    public float Modifier { get; }
    public bool NoMiss { get; }
    public int? HitNumOverride { get; }
    public bool NoCrit { get; }

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


}
