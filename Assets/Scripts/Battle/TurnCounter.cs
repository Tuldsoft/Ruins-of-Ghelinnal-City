using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurnCounter 
{
    
    static int totalTurns = 0;
    public static int TotalTurns { get { return totalTurns; } }

    //static int currentID = -1;
    public static int CurrentID { get { return turns[0]; } }

    public static int CurrentHeroID { get { return -turns[0] - 1; } }

    // returns the battleID
    //static int nextID = 0;
    public static int NextID { get { return turns[1]; } }

    // stores BattleHero and BattleEnemy objs
    static List<object> objTurns = new List<object>();

    // stores a single iteration of all objs to be copied into objTurns
    static List<object> objSequence = new List<object>();

    static List<int> turns = new List<int>();
    //public static List<int> Turns { get { return turns; } }

    // key is origID, value is battleID
    static Dictionary<int, int> origIDToBattleID = new Dictionary<int, int>();

    static EnemyParty enemyParty;
    static HeroParty heroParty;

    public static void GenerateFirstTurns(HeroParty heroes, EnemyParty enemies)
    {
        objSequence.Clear();
        objTurns.Clear();
        turns.Clear();
        //origIDToBattleID.Clear();

        heroParty = heroes;
        enemyParty = enemies;
        
        // generate a sequence
        foreach (BattleHero hero in heroParty.Hero)
        {
            objSequence.Add(hero);
            //objTurns.Add(hero);
            //turns.Add(hero.BattleID);
        }
        foreach (BattleEnemy enemy in enemyParty.Enemies)
        {
            objSequence.Add(enemy);
            //objTurns.Add(enemy);
            //turns.Add(enemy.BattleID);
        }

        objSequence = Randomize<object>.List(objSequence);

        GenerateMoreTurns();
        /*currentID = Turns[0];
        nextID = Turns[1];*/
    }

    static void GenerateMoreTurns()
    {
        /*foreach (BattleHero hero in heroParty.Hero)
        {
            objTurns.Add(hero);
            turns.Add(hero.BattleID);
        }
        foreach (BattleEnemy enemy in enemyParty.Enemies)
        {
            objTurns.Add(enemy);
            turns.Add(enemy.BattleID);
        }*/

        // adds to the turn sequence in both objTurns and turns, based on randomized objSequence
        foreach (object obj in objSequence)
        {
            objTurns.Add(obj);
            
            if (obj is BattleHero)
            {
                turns.Add((obj as BattleHero).BattleID);
            }
            else if (obj is BattleEnemy)
            {
                turns.Add((obj as BattleEnemy).BattleID);
            }
        }

    }

    public static void RefreshIDs()
    {
        // clear the list of ints, because battleIDs have changed
        turns.Clear();

        // remove any dead objs from objTurns, 
        // working backwards so that nothing is skipped over
        for (int i = objTurns.Count-1; i >= 0; i--)
        { 
            if (objTurns[i] is BattleHero)
            {
                if ( (objTurns[i] as BattleHero).IsDead)
                {
                    objTurns.RemoveAt(i);
                }
            }
            else if (objTurns[i] is BattleEnemy)
            {
                if ( (objTurns[i] as BattleEnemy).IsDead)
                {
                    objTurns.RemoveAt(i);
                }
            }
        }

        // remove dead obj from objSequence (there should only be one)
        /*for (int i = objSequence.Count - 1; i >= 0; i--)
        {
            if (objSequence[i] is BattleHero)
            {
                if ((objSequence[i] as BattleHero).IsDead)
                {
                    objSequence.RemoveAt(i);
                    break;
                }
            }
            else if (objSequence[i] is BattleEnemy)
            {
                if ((objSequence[i] as BattleEnemy).IsDead)
                {
                    objSequence.RemoveAt(i);
                    break;
                }
            }
        }*/

        // remove dead obj from objSequence (there should only be one)
        foreach (object obj in objSequence)
        {
            if (obj is BattleHero)
            {
                if ((obj as BattleHero).IsDead)
                {
                    objSequence.Remove(obj);
                    break;
                }
            }
            else if (obj is BattleEnemy)
            {
                if ((obj as BattleEnemy).IsDead)
                {
                    objSequence.Remove(obj);
                    break;
                }
            }
        }

        // repopulate the turns list, using the updated objTurns
        foreach (object obj in objTurns)
        {
            if (obj is BattleHero)
            {
                turns.Add((obj as BattleHero).BattleID);
            }
            else if (obj is BattleEnemy)
            {
                turns.Add((obj as BattleEnemy).BattleID);
            }
        }

        // copy additional turns from the sequence if there's not a current and next.
        if (objTurns.Count < 2) { GenerateMoreTurns(); }

    }

    public static void AdvanceToNextTurn()
    {
        objTurns.RemoveAt(0);
        turns.RemoveAt(0);
        if (objTurns.Count < 2)
        {
            GenerateMoreTurns();
        }
        /*else
        {
            currentID = origIDToBattleID[Turns[0]];
            nextID = origIDToBattleID[Turns[1]];
        }*/
        totalTurns++;
    }

}
