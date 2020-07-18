using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a reference source for all battle abilities.
/// </summary>
public static class BattleAbilityData 
{
    #region Fields and Properties
    // create a ConfigData for use in this class. 
    static ConfigData abilityData;

    // Main collection of BattleAbilitys
    public static Dictionary<BattleMode, BattleAbility> Data { get; }
        = new Dictionary<BattleMode, BattleAbility>();

    static bool initialized = false;
    #endregion

    #region Methods
    // called by the Initializer
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            // Construct abilityData. This loads abilities from the csv.
            abilityData = new ConfigData(ConfigDataType.AbilityData);

            // move from the ConfigData into the data in this class
            foreach (KeyValuePair<BattleMode, BattleAbility> pair in abilityData.AbilityStatData)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }

    // unused
    /*public static BattleAbility MakeNewBattleAbility(BattleMode name)
    {

        BattleAbility orig = Data[name];
        *//*BattleEnemy newEnemy = new BattleEnemy(name, 
            orig.HPMax, orig.MPMax, orig.MinDamage, orig.MaxDamage, orig.MinReward, orig.MaxReward);*//*
        //BattleEnemy newEnemy = new BattleEnemy(name, orig.Stats, orig.MinReward, orig.MaxReward);
        BattleAbility newAbility = new BattleAbility(name, orig.IsPhysical, orig.MP, orig.NoReduction, orig.Modifier, orig.NoMiss, orig.HitNumOverride, orig.NoCrit);

        return newAbility;
    }*/
    #endregion
}
