using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage 
{
    // negative for healing, 0 for Miss, positive for damage
    // null means the turn ended from some other effect

    // remove entries from each list as damage is displayed and applied
    Stack<int?> amount = new Stack<int?>();
    public int? Amount { get { return Mathf.CeilToInt((float)amount.Peek() * modifier); } }

    Stack<bool> isCrit = new Stack<bool>();
    public bool IsCrit { get { return isCrit.Peek(); } }

    Stack<bool> isItem = new Stack<bool>();
    public bool IsItem { get { return isItem.Peek(); } }

    Stack<bool> isMP = new Stack<bool>();
    public bool IsMP { get { return isMP.Peek(); } }

    float modifier = 1f;
    public int Count { get { return amount.Count; } }

    public void Add(int? amount, bool isCrit = false, bool isItem = false, bool isMP = false)
    {
        this.amount.Push(amount);
        this.isCrit.Push(isCrit);
        this.isItem.Push(isItem);
        this.isMP.Push(isMP);
    }

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

    public void Clear()
    {
        amount.Clear();
        isCrit.Clear();
        isItem.Clear();
        isMP.Clear();
    }

    public void Modify(float cheatMultiplier)
    {
        this.modifier = cheatMultiplier;
    }
}
