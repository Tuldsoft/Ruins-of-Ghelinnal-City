using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Battle: When an enemy reach 0 HP, signals the start of an enemy's death fadeout
public class Battle_EnemyDeathBeginEvent : UnityEvent<int, bool>
{
}
