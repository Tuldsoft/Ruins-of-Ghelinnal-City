using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A collection of methods for calculating things in battle
public static class BattleMath
{
    const int BaseMissChance = 10;
    const int BaseCritChance = 5;
    const float DamageVariance = 0.25f;
    const float ReduceVariance = 0.25f;
    
    // converts hero party ID to battle ID, and vice versa
    public static int ConvertHeroID(int heroNum)
    {
        return -heroNum - 1;
    }

    static int CalcDamage(BattleStats attStats, BattleStats defStats = null, bool isPhysical = true) 
    {
        // if physical, use str, otherwise, use magic
        int attAbilityScore = isPhysical ? attStats.Strength : attStats.Magic;
        
        // 1 +/- DamageVariance per Str
        int attackDamage = Mathf.CeilToInt(Random.Range( 1f - DamageVariance, 1f + DamageVariance) * attAbilityScore);


        // if defStats == null, there is 0 reduction
        int defDamage = 0;
        if (defStats != null)
        {
            // if physical, use def, otherwise, use resistance
            int defAbilityScore = isPhysical ? defStats.Defense : defStats.Resistance;

            // 0.5 +/- ReduceVariance damage reduction per def
            defDamage = Mathf.FloorToInt(Random.Range(1f - ReduceVariance, 1f + ReduceVariance) * defAbilityScore);
        }


        int output = Mathf.Max(attackDamage - defDamage, 1);

        return output;
    }


    static bool CalcCrit (BattleStats attStats)
    {
        int chance = Random.Range(0, (100 + 2 * attStats.CritChance));
        bool output = chance >= (100 - BaseCritChance) ? true : false;
        
        return output;
    }

    // returns 0 for a miss, or a positive int for one or multiple hits
    static int CalcHitsOrMiss(BattleStats attStats, BattleStats defStats = null)
    {
        int hitPercent = attStats.HitPercent;
        
        // if defender is null, evade percent of -BaseMissChance (10) guarantees that
        // hit chance is greater 
        int evadePercent = defStats == null ? -BaseMissChance : defStats.EvadePercent;

        int chance = Random.Range(0, (100 + 2 * hitPercent));


        // 10 - 100 is one hit, 100-200 is two hits, etc.
        // if under 10, it is a miss, so return 0
        int output = chance >= BaseMissChance + evadePercent ? (100 + chance) / 100 : 0;
        
        return output;
    }

    public static Damage FightDamage (BattleStats attStats, BattleStats defStats)
    {
        // calculate the num hits
        int hits = CalcHitsOrMiss(attStats, defStats);

        Damage damage = new Damage();

        // damage 0 is a miss
        //int damage = 0;

        // if hits = 0, damage remains 0, otherwise calculate damage hit times
        if (hits <= 0) { damage.Add(0); }
        else
        {
            for (int i = 0; i < hits; i++)
            {
                if (i > 1) { Debug.Log("Hit No. " + (i + 1)); }

                // if crit, multiply by 2
                bool isCrit = false;
                int critMultiplier = CalcCrit(attStats) ? 2 : 1;
                if (critMultiplier == 2)
                {
                    isCrit = true;
                    Debug.Log("Critical");
                }

                // add crit modified damage to the total
                damage.Add(CalcDamage(attStats, defStats) * critMultiplier, isCrit);
            }
        }
        return damage;
    }

    public static Damage Damage(BattleStats userStats, BattleStats targetStats = null, BattleAbility ability = null)
        // BattleAbility:
        //float modifier = 1f, bool noMiss = false, int? overrideNumHits = null, bool noCrit = false)
    {
        if (ability != null && ability.NoReduction)
        {
            targetStats = null;
        }

        // if there is no override and noMiss, hits stays at 1
        int hits = 1;
        if (ability.HitNumOverride == null)
        {
            if (!ability.NoMiss)
            {
                // calculate the num hits, 0 is a miss
                hits = CalcHitsOrMiss(userStats, targetStats);
            }
            //else hits remains at 1
        }
        else
        {
            hits = (int)ability.HitNumOverride;
        }

        // Damage { list of damage amounts and whether each hit is a crit }
        // Damage contains all the occurences of damage towards a target from this action
        Damage damage = new Damage();

        // damage 0 is a miss

        // if hits = 0, damage remains 0, otherwise calculate damage hit times
        if (hits <= 0) { damage.Add(0); }
        else
        {
            // calculate once per hit
            for (int i = 0; i < hits; i++)
            {
                if (i > 1) { Debug.Log("Hit No. " + (i + 1)); }

                // get a single base damage
                // if resist-free, targetStats = null above
                // CalcDamage will choose the abil scores based on isPhysical
                float tempDamage = (float)CalcDamage(userStats, targetStats, ability.IsPhysical);

                // calculate critMultiplier, leave as 1 if noCrit
                bool isCrit = false;
                int critMultiplier = 1;
                if (!ability.NoCrit)
                {
                    // if crit, multiply by 2
                    critMultiplier = CalcCrit(userStats) ? 2 : 1;
                    if (critMultiplier == 2)
                    {
                        isCrit = true;
                        Debug.Log("Critical");
                    }
                }

                // factor in critModifier
                tempDamage *= (float)critMultiplier;

                // factor in modifier
                tempDamage *= ability.Modifier;
                
                damage.Add(Mathf.CeilToInt(tempDamage), isCrit);
            }
        }

        return damage;
    }

}
