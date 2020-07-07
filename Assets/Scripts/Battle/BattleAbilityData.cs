using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleAbilityData 
{
    static ConfigData abilityData;

    public static Dictionary<BattleMode, BattleAbility> Data { get; }
        = new Dictionary<BattleMode, BattleAbility>();

    static bool initialized = false;

    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            abilityData = new ConfigData(ConfigDataType.AbilityData);

            foreach (KeyValuePair<BattleMode, BattleAbility> pair in abilityData.AbilityStatData)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }

    public static BattleAbility MakeNewBattleAbility(BattleMode name)
    {

        BattleAbility orig = Data[name];
        /*BattleEnemy newEnemy = new BattleEnemy(name, 
            orig.HPMax, orig.MPMax, orig.MinDamage, orig.MaxDamage, orig.MinReward, orig.MaxReward);*/
        //BattleEnemy newEnemy = new BattleEnemy(name, orig.Stats, orig.MinReward, orig.MaxReward);
        BattleAbility newAbility = new BattleAbility(name, orig.IsPhysical, orig.MP, orig.NoReduction, orig.Modifier, orig.NoMiss, orig.HitNumOverride, orig.NoCrit);

        return newAbility;
    }
}
