using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    int valueRemaining = 10;
    Vector2 spawnPos;
    Vector2 bankPos;
    EventTimer spawnTimer;
    float spawnInterval = 0.14f; 
    const float CoinSpeed = 15f;
    int goldPerSpawn = 7;
    const float SpawnDuration = 1.4f;
    const float SpawnDuration_Boss = 3.9f;

    GameObject prefabCoinSpawn;

    public void StartSpawn(Vector2 origPos, Vector2 targetPos, int reward, bool isBoss)
    {
        spawnTimer = gameObject.AddComponent<EventTimer>();
        
        spawnTimer.AddListener_Finished(SpawnCoin);
        prefabCoinSpawn = Resources.Load<GameObject>(@"BattlePrefabs\CoinSpawn"); 
        
        spawnPos = origPos;
        bankPos = targetPos;
        valueRemaining = reward;
        // generate an interval between 0.14 and 0.01 based on the value
        spawnInterval = Mathf.Clamp( 0.14f / (float)Math.Pow(2d, Math.Log10(valueRemaining) - 1d) , 0.01f, 0.14f);

        float duration = isBoss ? SpawnDuration_Boss : SpawnDuration;
        goldPerSpawn = Mathf.Max(
            Mathf.CeilToInt(valueRemaining / (duration / spawnInterval)), 7);
        //spawnInterval = SpawnDuration / (reward / goldPerSpawn);
        spawnTimer.Duration = spawnInterval;

        SpawnCoin();
    }

    void SpawnCoin()
    {
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
        GameObject newCoin = GameObject.Instantiate(prefabCoinSpawn);
        newCoin.GetComponent<CoinSpawn>().Value = coinValue;
        newCoin.transform.position = spawnPos;
        Rigidbody2D rb2d = newCoin.GetComponent<Rigidbody2D>();
        rb2d.velocity = (bankPos - spawnPos).normalized * CoinSpeed;
        if (valueRemaining > 0)
        {
            spawnTimer.Run();
        }
    }

}
