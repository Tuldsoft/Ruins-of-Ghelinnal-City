using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A spawner of coins. Added as a component to BattleMenuOptions by BattleManager to transder
/// funds from a defeated enemy to the bank in the center-top part of the battle scene.
/// When created, it is given a value from the defeated enemy. It then puts portions of that
/// value into coins, which are launched at intervals at the bank. After the final coin is 
/// created, it destroys the timer and self-destructs.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    #region Fields
    // Reward value remaining
    int valueRemaining = 10;
    
    // Origin of spawn and target
    Vector2 spawnPos;
    Vector2 bankPos;

    // Timer for generating spawn
    EventTimer spawnTimer;
    float spawnInterval = 0.14f; 

    // Speed of coins
    const float CoinSpeed = 15f;
    
    // Gold per coin
    int goldPerSpawn = 7;
    
    // Total duration of the spawning
    // (shorter than the fadeout to ensure all coins make it to the bank)
    const float SpawnDuration = 1.4f;
    const float SpawnDuration_Boss = 3.9f;

    // Coin gameObject template
    GameObject prefabCoinSpawn;
    #endregion

    #region Methods
    // Called by the BattleManager when an enemy reaches 0 hp.
    public void StartSpawn(Vector2 origPos, Vector2 targetPos, int reward, bool isBoss)
    {
        // Add timer to scene
        spawnTimer = gameObject.AddComponent<EventTimer>();
        
        // Timer triggers SpawnCoin method
        spawnTimer.AddListener_Finished(SpawnCoin);
        
        // Store prefab for instantiating
        prefabCoinSpawn = Resources.Load<GameObject>(@"BattlePrefabs\CoinSpawn"); 
        
        // Set positions
        spawnPos = origPos;
        bankPos = targetPos;
        
        valueRemaining = reward;
        
        // generate an interval between 0.14 and 0.01 based on the value
        // bigger cash rewards have shorter intervals, so that a flurry of coins are produced
        spawnInterval = Mathf.Clamp( 0.14f / (float)Math.Pow(2d, Math.Log10(valueRemaining) - 1d) , 0.01f, 0.14f);

        // Total spawn duration is longer for a boss, due to the longer death sequence
        float duration = isBoss ? SpawnDuration_Boss : SpawnDuration;
        
        // Calculate the value of each coin, given a total value, an interval, and a duration
        goldPerSpawn = Mathf.Max(
            Mathf.CeilToInt(valueRemaining / (duration / spawnInterval)), 7);
        
        // Set the interval on the timer
        spawnTimer.Duration = spawnInterval;

        // Spawn the first coin
        SpawnCoin();
    }

    // Spawns a coin and sends it towards the bank
    void SpawnCoin()
    {
        // set the value of the coin, deduct it from the valueRemaining to convert to coins
        int coinValue;
        if (valueRemaining - goldPerSpawn < 0)
        {
            coinValue = valueRemaining;
        }
        else
        {
            coinValue = goldPerSpawn;
        }
        valueRemaining -= coinValue;

        // Make a new coin
        GameObject newCoin = GameObject.Instantiate(prefabCoinSpawn);
        newCoin.GetComponent<CoinSpawn>().Value = coinValue;
        newCoin.transform.position = spawnPos;
        
        // Send the coin to the bank
        Rigidbody2D rb2d = newCoin.GetComponent<Rigidbody2D>();
        rb2d.velocity = (bankPos - spawnPos).normalized * CoinSpeed;
        
        // Set the timer for the next spawning, or destroy the timer and the spawner
        if (valueRemaining > 0)
        {
            spawnTimer.Run();
        }
        else
        {
            Destroy(spawnTimer);
            Destroy(this);
        }
    }
    #endregion
}
