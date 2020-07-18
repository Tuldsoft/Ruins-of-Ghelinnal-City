using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a List (more like a Queue) of turns. Its most frequent use is to tell BattleManager
/// and other classes whose turn it is, using battleID. It also generates the turns themselves,
/// and adds more as necessary. When an enemy is destroyed, TurnCounter will remove that enemy's
/// turns from the queue, and recalculate IDs.
/// The current turn is always turn[0].
/// REFACTOR: Use a struct or in-line class definition instead of parallel lists.
/// REFACTOR: Try creating as a LinkedList.
/// REFACTOR: Convert generic objs objects to Battler objects.
/// </summary>

public static class TurnCounter 
{
    #region Fields and Properties
    // Total number of turns that have occured in this battle (unused)
    static int totalTurns = 0;
    public static int TotalTurns { get { return totalTurns; } }

    // returns BattleID of ther current turn
    public static int CurrentID { get { return turns[0]; } }

    // returns a battleID of the current turn as a BattleHero partyID
    public static int CurrentHeroID { get { return -turns[0] - 1; } }

    // returns the battleID of the next turn
    public static int NextID { get { return turns[1]; } }

    
    // Stores BattleHero and BattleEnemy objs, parallel to turns
    static List<object> objTurns = new List<object>();    // ex: {Edgar, Sabin, Wererat, Edgar, Sabin}

    // Stores a single iteration of all objs to be copied into objTurns. Each combatant appears once.
    static List<object> objSequence = new List<object>(); // ex: {Edgar, Sabin, Wererat}

    // Stores turns as a list of battleIDs
    static List<int> turns = new List<int>();             // ex: { -2, -1, 0, -2, -1}

    // References to BattleLoader's enemy and hero partys
    static EnemyParty enemyParty;
    static HeroParty heroParty;
    #endregion

    #region Static Methods
    /// <summary>
    /// Called only once at the start of battle, this will generate the objSequence at 
    /// random, where each  combatant occurs exactly once. It does not generate the turns 
    /// themselves, except through a call to GenerateMoreTurns().
    /// </summary>
    /// <param name="heroes">the HeroParty in BattleLoader</param>
    /// <param name="enemies">the EnemyParty in BattleLoader</param>
    public static void GenerateFirstTurns(HeroParty heroes, EnemyParty enemies)
    {
        // Clear from previous battles
        objSequence.Clear();
        objTurns.Clear();
        turns.Clear();

        // Store provided references
        heroParty = heroes;
        enemyParty = enemies;
        
        // Generate a sequence in order
        foreach (BattleHero hero in heroParty.Hero)
        {
            objSequence.Add(hero);
        }
        foreach (BattleEnemy enemy in enemyParty.Enemies)
        {
            objSequence.Add(enemy);
        }

        // Use custom Randomize class to randomize the sequence of objects
        objSequence = Randomize<object>.List(objSequence);

        // Generate turns using objSequence as a key
        GenerateMoreTurns();
    }

    /// <summary>
    /// Called at the end of GenerateFirstTurns() and any time turns are running low,
    /// this adds a single iteration of objSequence to the back of objTurns. It keeps turns
    /// and objTurns in parallel.
    /// </summary>
    static void GenerateMoreTurns()
    {
        // Adds to the turn sequence in both objTurns and turns, based on randomized objSequence
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

    // If a hero or enemy dies, battleIDs need to be refreshed in TurnCounter
    // so that the destroyed object can no longer be referenced
    public static void RefreshIDs()
    {
        // Clear ints in turns, because battleIDs have changed
        turns.Clear();

        // Remove any dead objs from objTurns, 
        // working BACKWARDS so that nothing is skipped over
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

        // Remove dead obj from objSequence (there is only one)
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

        // Repopulate the turns list of BattleIDs, using the updated objTurns
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

        // Generate additional turns from the sequence if there's not a current and next.
        if (objTurns.Count < 2) { GenerateMoreTurns(); }
    }

    // Advances the TurnCounter to the next turn. Used only in BattleManager.TurnOver()
    public static void AdvanceToNextTurn()
    {
        // Remove current turn
        objTurns.RemoveAt(0);
        turns.RemoveAt(0);

        // Now currentTurn = the new turn[0] and nextTurn = the new turn[1]
        
        // If there's fewer than 2 turns, add more turns
        if (objTurns.Count < 2)
        {
            GenerateMoreTurns();
        }
        
        // Increase the counter of total turns during this battle
        totalTurns++;
    }
    #endregion
}
