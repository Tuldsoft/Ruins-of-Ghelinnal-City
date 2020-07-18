using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of methods for calculating things in battle.
/// Most methods reply on the passing of one or more BattleStats.
/// BattleMath is creature-agnostic. It does not distinguish whether
/// the BattleStats come from a hero or enemy.
/// </summary>
public static class BattleMath
{
    #region Fields
    const int BaseMissChance = 10;
    const int BaseCritChance = 5;
    const float DamageVariance = 0.25f;
    const float ReduceVariance = 0.25f;
    #endregion

    #region Methods
    // Converts a BattleHero's partyID to battleID, and vice versa
    public static int ConvertHeroID(int heroNum)
    {
        return -heroNum - 1;
    }

    // Calculates the damage of a single attack or spell (including healing spells).
    static int CalcDamage(BattleStats attStats, BattleStats defStats = null, bool isPhysical = true) 
    {
        // See above constants for Variance
        
        // if physical, use str, otherwise, use magic
        int attAbilityScore = isPhysical ? attStats.Strength : attStats.Magic;
        
        // 1 +/- DamageVariance per Str or Mag
        int attackDamage = Mathf.CeilToInt(Random.Range( 1f - DamageVariance, 1f + DamageVariance) * attAbilityScore);


        // if defStats == null, there is 0 reduction
        int defDamage = 0;
        if (defStats != null)
        {
            // if physical, use def, otherwise, use resistance
            int defAbilityScore = isPhysical ? defStats.Defense : defStats.Resistance;

            // 0.5 +/- ReduceVariance damage reduction per def or res
            defDamage = Mathf.FloorToInt(Random.Range(1f - ReduceVariance, 1f + ReduceVariance) * defAbilityScore);
        }

        int output = Mathf.Max(attackDamage - defDamage, 1);

        return output;
    }

    // Calculates whether a single attack is a critical
    static bool CalcCrit (BattleStats attStats)
    {
        int chance = Random.Range(0, (100 + 2 * attStats.CritChance));
        bool output = chance >= (100 - BaseCritChance) ? true : false;
        
        return output;
    }

    /// <summary>
    /// Calculates how many hits occur, and whether they connect or miss.
    /// Returns 0 for a miss, or a positive int for one or multiple hits
    /// Based on a creature's BattleStats.HitPercent. 
    /// </summary>
    /// <param name="attStats">BattleStats of attacking creature</param>
    /// <param name="defStats">BattleStats of defending creature</param>
    /// <returns></returns>
    static int CalcHitsOrMiss(BattleStats attStats, BattleStats defStats = null)
    {
        int hitPercent = attStats.HitPercent;
        
        // If defender is null, evade percent of -BaseMissChance (10) guarantees that
        // hit chance is greater 
        int evadePercent = defStats == null ? -BaseMissChance : defStats.EvadePercent;

        int chance = Random.Range(0, (100 + 2 * hitPercent));

        // 10 - 100 is one hit, 100-200 is two hits, etc.
        // if under 10, it is a miss, so return 0
        int output = chance >= BaseMissChance + evadePercent ? (100 + chance) / 100 : 0;
        
        return output;
    }

    /// <summary>
    /// A simplified version of Damage(), used for hero or enemy Fight attacks.
    /// Returns a Damage object (not an int), which contains 1 or more hits/misses,
    /// and info on whether each hit was a crit.
    /// </summary>
    /// <param name="attStats">BattleStats of the attacker</param>
    /// <param name="defStats">BattleStats of the defender</param>
    /// <returns></returns>
    public static Damage FightDamage (BattleStats attStats, BattleStats defStats)
    {
        // Calculate the number of hits
        int hits = CalcHitsOrMiss(attStats, defStats);

        // Create an empty Damage object
        Damage damage = new Damage();

        // If no hits, return a Damage with a single value of 0 to signify a miss
        if (hits <= 0) { damage.Add(0); }
        else
        {
            // Calculate a crit for each hit
            for (int i = 0; i < hits; i++)
            {
                //if (i > 1) { Debug.Log("Hit No. " + (i + 1)); }

                // CalcCrit(). If crit, increase the multiplier to 2
                bool isCrit = false;
                int critMultiplier = CalcCrit(attStats) ? 2 : 1;
                if (critMultiplier == 2)
                {
                    isCrit = true;
                    //Debug.Log("Critical");
                }

                // Adds the value of a crit-modified hit to the list in Damage
                damage.Add(CalcDamage(attStats, defStats) * critMultiplier, isCrit);
            }
        }
        return damage;
    }

    /// <summary>
    /// Creates a generic, type- and creature-agnostic calculation of Damage. Used
    /// by every non-Fight option where calculations are required (including healing).
    /// Returns a Damage object (not an int), which contains 1 or more hits/misses,
    /// and info on whether each hit was a crit.
    /// If a BattleAbility is supplied, modifies the results based on the ability's properties.
    /// </summary>
    /// <param name="userStats">The attacker's BattleStats</param>
    /// <param name="targetStats">The defender's BattleStats</param>
    /// <param name="ability">The BattleAbility being used</param>
    /// <returns></returns>
    public static Damage Damage(BattleStats userStats, BattleStats targetStats = null, BattleAbility ability = null)
        // BattleAbility Properties and their defaults:
        //float modifier = 1f, bool noMiss = false, int? overrideNumHits = null, bool noCrit = false)
    {
        
        // Negate targetStats if NoReduction
        if (ability != null && ability.NoReduction)
        {
            targetStats = null;
        }

        // if there is a HitNumOverride, use it for hits.
        // if not, if it is NoMiss, hits = 1.
        // if no HitNumOverride and NoMiss is false, calculate the number of hits.
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
        // A Damage object contains all the occurences of damage towards a target from this action
        Damage damage = new Damage();

        // a value of 0 is a miss

        // if hits = 0, damage remains 0, otherwise calculate damage hit times
        if (hits <= 0) { damage.Add(0); }
        else
        {
            // calculate once per hit
            for (int i = 0; i < hits; i++)
            {
                //if (i > 1) { Debug.Log("Hit No. " + (i + 1)); }

                // Get a single, initial damage value
                // if resist-free, targetStats = null as above
                // CalcDamage will choose which ability scores based on isPhysical
                float tempDamage = (float)CalcDamage(userStats, targetStats, ability.IsPhysical);

                // Calculate critMultiplier, leave as 1 if ability prevents crit via NoCrit
                bool isCrit = false;
                int critMultiplier = 1;
                if (!ability.NoCrit)
                {
                    // Calculate if this hit is a crit. If crit, multiplier is 2, else 1
                    critMultiplier = CalcCrit(userStats) ? 2 : 1;
                    if (critMultiplier == 2)
                    {
                        isCrit = true;
                        //Debug.Log("Critical");
                    }
                }

                // factor in critModifier
                tempDamage *= (float)critMultiplier;

                // factor in ability modifier
                tempDamage *= ability.Modifier;
                
                // Convert to int, and add as an entry to the Damage object
                damage.Add(Mathf.CeilToInt(tempDamage), isCrit);
            }
        }

        return damage;
    }
    #endregion
}
