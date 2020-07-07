using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Initializer 
{
    
    public static void Run()
    {
        //AudioManager initialized by GameAudioSource object
        InvData.Initialize();
        BattleEnemyData.Initialize();
        HeroSpriteData.Initialize();
        BattleAbilityData.Initialize();
        BattleLoader.Initialize();
        Shop.Initialize();
        TalkData.Initialize();
    }

    public static void ResetData()
    {
        // reset BattleLoader
        BattleLoader.ResetData();

        // reset Shop
        Shop.ResetData();
    }

}
