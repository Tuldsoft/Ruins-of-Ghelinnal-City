using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Damage class is an object that contains parallel Stacks of all instances of damage from one 
/// creature to another. It is created via BattleMath in most cases, and is passed to a DamageDisplay.
/// DamageDisplay works through the values in Damage, displaying them above the targeted hero 
/// or enemy using a timer.
/// Damage also contains information about the damage amount, such as whether it is a Crit or MP,
/// so that DamageDisplay knows what color to use and what messages to display with the numerical value.
/// Damage amounts are negative in the case of healing, positive in the case of damage, 0 in the
/// case of a miss, and null when it needs to be ignored (ex: during a TurnOver).
/// </summary>

public class Damage 
{
    // negative for healing, 0 for Miss, positive for damage
    // null means the turn ended from some other effect

    // DamageDisplay removes entries from each stack as damage is displayed and applied
    
    #region Fields and Properties
    // A stack of numerical values of damage
    Stack<int?> amount = new Stack<int?>();
    public int? Amount { get { return Mathf.CeilToInt((float)amount.Peek() * modifier); } }

    // Whether the amount is considered a Crit
    Stack<bool> isCrit = new Stack<bool>();
    public bool IsCrit { get { return isCrit.Peek(); } }

    // Whether the "damage" amount is from an item
    Stack<bool> isItem = new Stack<bool>();
    public bool IsItem { get { return isItem.Peek(); } }

    // Whether the "damage" applies to MP instead of HP (ex: mana potions)
    Stack<bool> isMP = new Stack<bool>();
    public bool IsMP { get { return isMP.Peek(); } }

    // Used by BattleManager for cheating
    float modifier = 1f;

    // Number of damage values in the stack
    public int Count { get { return amount.Count; } }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds a damage instance to Damage's stacks. Used by BattleMath for calculated amounts,
    /// and BattleInvMonitor and BattleManager for fixed amounts.
    /// </summary>
    /// <param name="amount">Numerical value of this instance of damage. (neg for heal, pos for dam, 0 to miss, null to skip display.</param>
    /// <param name="isCrit">Whether or not the damage has been doubled by BattleMath</param>
    /// <param name="isItem">Whether or not the damage is a result of item use</param>
    /// <param name="isMP">Whether or not the damage is to be applied to MP instead of MP</param>
    public void Add(int? amount, bool isCrit = false, bool isItem = false, bool isMP = false)
    {
        this.amount.Push(amount);
        this.isCrit.Push(isCrit);
        this.isItem.Push(isItem);
        this.isMP.Push(isMP);
    }

    // Removes a damage instance from Damage's stacks (ex: after being applied and displayed)
    public void Remove()
    {
        if (Count > 0)
        {
            amount.Pop();
            isCrit.Pop();
            isItem.Pop();
            isMP.Pop();
        }
    }

    // Removes all instances from Damage's stacks
    public void Clear()
    {
        amount.Clear();
        isCrit.Clear();
        isItem.Clear();
        isMP.Clear();
    }

    // Sets the cheatMultiplier
    public void Modify(float cheatMultiplier)
    {
        this.modifier = cheatMultiplier;
    }
    #endregion
}
