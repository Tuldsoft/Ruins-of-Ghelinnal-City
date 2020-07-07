using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStats 
{
    int baseHPMax = 50;
    public int BaseHPMax { get { return baseHPMax; } }
    int hpMaxBonus = 0;
    int hpMax = 50;
    public int HPMax { get { return hpMax; } }


    int baseMPMax = 50;
    public int BaseMPMax { get { return baseMPMax; } }
    int mpMaxBonus = 0;
    int mpMax = 50;
    public int MPMax { get { return mpMax; } }


    int strength = 5;
    public int Strength { get { return strength; } }

    int defense = 5;
    public int Defense { get { return defense; } }

    int magic = 5;
    public int Magic { get { return magic; } }

    int resistance = 5;
    public int Resistance { get { return resistance; } }


    int stamina = 5;
    public int Stamina { get { return stamina; } }
    int agility = 5;
    public int Agility { get { return agility; } }

    int baseHit = 0;
    public int BaseHit { get { return baseHit; } }
    int hitPercentBonus = 0;
    int hitPercent = 0;
    public int HitPercent { get { return hitPercent; } }

    int baseEvade = 0;
    public int BaseEvade { get { return baseEvade; } }
    int evadePercentBonus = 0;
    int evadePercent = 0;
    public int EvadePercent { get { return evadePercent; } }

    int critChance = 0;
    public int CritChance { get { return critChance; } }

    int minReward = 10;
    public int MinReward { get { return minReward; } }

    int maxReward = 25;
    public int MaxReward { get { return maxReward; } }

    

    public void UpdateMax(int hpIncrease = 0, int mpIncrease = 0)
    {
        hpMaxBonus += hpIncrease;
        mpMaxBonus += mpIncrease;
        hpMax = Mathf.CeilToInt(baseHPMax * (1f + Mathf.Pow((float)stamina, 1.25f) / 100f)) 
            + hpMaxBonus;
        mpMax = Mathf.CeilToInt(baseMPMax * (1f + Mathf.Pow((float)resistance, 0.5f) / 100f
            + Mathf.Pow((float)resistance, 0.5f) / 100f) ) + mpMaxBonus;

    }

    public void UpdateHitEvadeCrit(int hitIncrease = 0, int evadeIncrease = 0, int critIncrease = 0)
    {
        hitPercentBonus += hitIncrease;
        evadePercentBonus += evadeIncrease;
        critChance += critIncrease;

        hitPercent = Mathf.FloorToInt(baseHit * (1f + Mathf.Pow((float)agility, 1.1f) / 100))
            + hitPercentBonus;
        evadePercent = Mathf.FloorToInt(BaseEvade * (1f + Mathf.Pow((float)agility, 1.1f) / 100))
            + evadePercentBonus;
    }

    public void Equip (BattleStats addStats, BattleStats removeStats = null)
    {
        if (removeStats != null)
        {
            strength -= removeStats.Strength;
            defense -= removeStats.Defense;
            magic -= removeStats.Magic;
            resistance -= removeStats.Resistance;
            stamina -= removeStats.Stamina;
            agility -= removeStats.Agility;

            // BaseHPMax applies to hpbonus, etc. so that it is on top of the derived stat.

            UpdateMax(-removeStats.BaseHPMax, -removeStats.BaseMPMax);
            UpdateHitEvadeCrit(-removeStats.BaseHit, -removeStats.BaseEvade, -removeStats.CritChance);
        }

        strength += addStats.Strength;
        defense += addStats.Defense;
        magic += addStats.Magic;
        resistance += addStats.Resistance;
        stamina += addStats.Stamina;
        agility += addStats.Agility;
        
        // BaseHPMax applies to hpbonus, etc. so that it is on top of the derived stat.

        UpdateMax(addStats.BaseHPMax, addStats.BaseMPMax);
        UpdateHitEvadeCrit(addStats.BaseHit, addStats.BaseEvade, addStats.CritChance);

    }

    public void Import(int[] import)
    {
        if (import.Length != 13)
        {
            Debug.Log("error importing BattleStats, array wrong size.");
            return;
        }

        baseHPMax = import[0];
        baseMPMax = import[1];
        strength = import[2];
        defense = import[3];
        magic = import[4];
        resistance = import[5];
        stamina = import[6];
        agility = import[7];
        baseHit = import[8];
        baseEvade = import[9];
        critChance = import[10];
        minReward = import[11];
        maxReward = import[12];

        UpdateMax();
        UpdateHitEvadeCrit();

    }

    public bool ValidateOrder(int slot, BattleStatNames name)
    {
        switch (slot)
        {
            case 0:
                return name == BattleStatNames.BaseHPMax ? true : false;
            case 1:
                return name == BattleStatNames.BaseMPMax ? true : false;
            case 2:
                return name == BattleStatNames.Strength ? true : false;
            case 3:
                return name == BattleStatNames.Defense ? true : false;
            case 4:
                return name == BattleStatNames.Magic ? true : false;
            case 5:
                return name == BattleStatNames.Resistance ? true : false;
            case 6:
                return name == BattleStatNames.Stamina ? true : false;
            case 7:
                return name == BattleStatNames.Agility ? true : false;
            case 8:
                return name == BattleStatNames.BaseHit ? true : false;
            case 9:
                return name == BattleStatNames.BaseEvade ? true : false;
            case 10:
                return name == BattleStatNames.CritChance ? true : false;
            case 11:
                return name == BattleStatNames.MinReward ? true : false;
            case 12:
                return name == BattleStatNames.MaxReward ? true : false;
            default:
                return false;
        }

    }

    public int[] Export()
    {
        int[] export = new int[13];
        export[0] = baseHPMax;
        export[1] = baseMPMax;
        export[2] = strength;
        export[3] = defense;
        export[4] = magic;
        export[5] = resistance;
        export[6] = stamina;
        export[7] = agility;
        export[8] = baseHit;
        export[9] = baseEvade;
        export[10] = critChance;
        export[11] = minReward;
        export[12] = maxReward;
        
        return export;
    }

    
}
