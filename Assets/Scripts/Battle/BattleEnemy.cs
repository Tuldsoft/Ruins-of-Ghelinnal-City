using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEnemy
{

    EnemyName enemyName;
    public EnemyName Name { get { return enemyName; } }

    Vector2 position;
    public Vector2 Position { get { return position; } }
    
    GameObject gameObject;
    public GameObject GameObject { get { return gameObject; } }

    int battleID = 0;
    public int BattleID { get { return battleID; } }

    int NameNumber;

    string fullName;
    public string FullName { get{ return fullName; }}
    
    int hp = 10;
    public int HP { get { return hp; } }

    //int hpMax = 10;
    public int HPMax { get { return Stats.HPMax; } }
    //Slider hpSlider;
    //public Slider HPSlider { get { return hpSlider; } }

    int mp = 10;
    public int MP { get { return mp; } }

    //int mpMax = 10;
    public int MPMax { get { return Stats.MPMax; } }
    //Slider mpSlider;
    //public Slider MPSlider { get { return mpSlider; } }

    public BattleStats Stats = new BattleStats();


    /*int minDamage = 1;
    public int MinDamage { get { return minDamage; } }

    int maxDamage = 4;
    public int MaxDamage { get { return maxDamage; } }*/

    //int minReward = 7;
    public int MinReward { get { return Stats.MinReward; } }

    //int maxReward = 27;
    public int MaxReward { get { return Stats.MaxReward; } }
    public int Reward { get { return Random.Range(MinReward, MaxReward + 1); } }


    bool isDead = false;
    public bool IsDead { get { return isDead; } }

    public int PoisonCounter { get; set; } = 0;
    public BattleStats PoisonerBStats { get; set; } = null;

    public int StealsRemaining { get; set; } = 3;
    
    bool isBoss = false;
    public bool IsBoss { get { return isBoss; } }


    //public BattleEnemy(EnemyName name, int hpMax, int mpMax, int minDam, int maxDam, int minRew, int maxRew)
    public BattleEnemy(EnemyName name, BattleStats stats)
    {
        enemyName = name;
        //gameObject = obj;
        //position = pos;
        //NameNumber = nameNum;
        //isBoss = boss;
        //string idAddendum = "";
        //if (NameNumber > 0) { idAddendum = " " + NameNumber; }
        //fullName = enemyName.ToString() + idAddendum;

        //this.hpMax = hpMax;
        //this.mpMax = mpMax;
        //minDamage = minDam;
        //maxDamage = maxDam;

        this.Stats = stats; 

        hp = stats.HPMax;
        mp = stats.MPMax;
        //minReward = minRew;
        //maxReward = maxRew;

        //obj.transform.position = position;

        
    }

    public void LoadObjs(int battleID, int nameSuffix, GameObject obj, Vector2 pos, bool boss)
    {
        this.battleID = battleID;        
        NameNumber = nameSuffix;
        string idAddendum = "";
        if (NameNumber > 0) { idAddendum = " " + NameNumber; }
        fullName = (enemyName.ToString() + idAddendum).Replace("_", " ");

        gameObject = obj;

        position = pos;
        obj.transform.position = position;

        isBoss = boss; // set during placement
    }

    public void ResetID(int battleID)
    {
        this.battleID = battleID;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        { hp = 0; isDead = true; }
        else { isDead = false; }
        if (hp > HPMax) { hp = HPMax; }

    }

    public void TakeMPDamage(int damage)
    {
        mp -= damage;
        if (mp <= 0) { mp = 0; }
        if (mp > MPMax) { mp = MPMax; }
    }

    public void HealAll()
    {
        hp = HPMax;
        mp = MPMax;
    }

}
