using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes most static methods that store persistent data. Each Initialize() method
/// contains a boolean check for initialized, so that it can only be run once. Initializer is
/// run by the main controller script in each scene, often the one connected to the Main Camera.
/// This allows any scene to be loaded in the editor for testing.
/// ResetData() is used only when creating a new game. Only BattleLoader and Shop need reseting.
/// </summary>
public static class Initializer 
{
    // Runs the Initializer() method in each static script. The call is ignored if
    // a class has already been initialized.
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

    // Empties and re-initializes BattleLoader and Shop for a New Game.
    public static void ResetData()
    {
        // reset BattleLoader
        BattleLoader.ResetData();

        // reset Shop
        Shop.ResetData();
    }

}
