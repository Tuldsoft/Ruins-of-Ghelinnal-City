using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Initializes and stores data for each kind of enemy
/// </summary>
public static class BattleEnemyData 
{
    #region Fields and Properties
    // enemyData is populated when constructed
    static ConfigData enemyData;
    
    // stores the data constructed in enemyData
    public static Dictionary<EnemyName, BattleEnemy> Data { get; } 
        = new Dictionary<EnemyName, BattleEnemy>();

    static bool initialized = false;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes BattleEnemyData. Called by the Initializer.
    /// </summary>
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            // Construct enemyData by loading it from the csv
            enemyData = new ConfigData(ConfigDataType.EnemyData);

            // Store data in Data
            foreach (KeyValuePair<EnemyName,BattleStats> pair in enemyData.EnemyStatData)
            {
                Data.Add(pair.Key, new BattleEnemy(pair.Key, pair.Value));
            }

            #region OldData
            /*
            // information here moved to ConfigData
            
            EnemyName name;
            BattleStats stats = new BattleStats();

            // hp, mp, minDam, maxDam, minReward, maxReward

            *//*baseHPMax, baseMPMax, 
             strength, defense, magic, resistance,
             stamina, agility, baseHit, baseEvade, critChance*//*


            name = EnemyName.none;
            stats.Import(new int[11] 
            {   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1 });
            Data.Add(name, new BattleEnemy(name, stats,   1,   1));
            
            name = EnemyName.Wererat;
            *//*Data.Add(name, new BattleEnemy(name,
                7, 7, 1, 4, 11, 21));*//*
            stats = new BattleStats(); 
            stats.Import(new int[11]
            {   7,   7,   2,   1,   1,   1,   1,   5,  10,   5,   5 });
            Data.Add(name, new BattleEnemy(name, stats, 11, 21));

            name = EnemyName.Ultros;
            *//*Data.Add(name, new BattleEnemy(name, 
                20, 20, 3, 7, 61, 89));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  20,  20,   7,   5,   5,   5,  15,  10,  25,   0,   0 });
            Data.Add(name, new BattleEnemy(name, stats, 61, 89));

            name = EnemyName.Guard;
            *//*Data.Add(name, new BattleEnemy(name,
                12, 20, 3, 9, 22, 44));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  12,  20,   6,   8,   3,   8,  10,  10,   5,   5,   0 });
            Data.Add(name, new BattleEnemy(name, stats, 22, 44));

            name = EnemyName.Wolf;
            *//*Data.Add(name, new BattleEnemy(name,
                20, 10, 6, 9, 20, 40));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  20,  10,   8,   4,   3,   4,  20,  20,  10,  10,   5 });
            Data.Add(name, new BattleEnemy(name, stats, 20, 40));

            name = EnemyName.Whelk;
            *//*Data.Add(name, new BattleEnemy(name,
                45, 30, 5, 17, 70, 102));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  45,  30,  20,  20,  12,  20,  30,  20,   5,   0,   0 });
            Data.Add(name, new BattleEnemy(name, stats, 70,150));

            name = EnemyName.Mechanic;
            *//*Data.Add(name, new BattleEnemy(name,
                16, 15, 4,  7, 24, 50));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  16,  15,   7,   3,   4,   5,  15,  15,  10,  10,   5 });
            Data.Add(name, new BattleEnemy(name, stats, 24, 50));

            name = EnemyName.Grease_Monkey;
            *//*Data.Add(name, new BattleEnemy(name,
                22, 30, 5,  9, 27, 55));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  22,  30,   8,   4,   8,   6,  15,  20,  10,  10,   5 });
            Data.Add(name, new BattleEnemy(name, stats, 27, 55));

            name = EnemyName.Roper;
            *//*Data.Add(name, new BattleEnemy(name,
                20, 40, 6,  9, 21, 30));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  20,  40,   8,  10,   6,   9,  30,  10,  10,   0,   0 });
            Data.Add(name, new BattleEnemy(name, stats, 21, 30));

            name = EnemyName.Magitek_Armor;
            *//*Data.Add(name, new BattleEnemy(name,
                38, 50,  9, 20,  85, 112));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  38,  50,  22,  22,  22,  22,  40,  20,   5,   0,  10 });
            Data.Add(name, new BattleEnemy(name, stats, 85,150));

            name = EnemyName.Tunnel_Armor;
            *//*Data.Add(name, new BattleEnemy(name,
                65, 60, 12, 35, 150, 275));*//*
            stats = new BattleStats();
            stats.Import(new int[11]
            {  65,  60,  35,  35,  25,  25,  50,  50,   5,  10,  10 });
            Data.Add(name, new BattleEnemy(name, stats,175,300));*/
            #endregion OldData
        }
    }

    /// <summary>
    /// Creates a new BattleEnemy and gives it its data
    /// </summary>
    /// <param name="name">Name as EnemyName</param>
    /// <returns></returns>
    public static BattleEnemy MakeNewBattleEnemy(EnemyName name)
    {
        // Create data
        BattleEnemy orig = Data[name];
        // Create enemy using data
        BattleEnemy newEnemy = new BattleEnemy(name, orig.Stats);
        newEnemy.HealAll();
        return newEnemy;
    }
    #endregion
}
