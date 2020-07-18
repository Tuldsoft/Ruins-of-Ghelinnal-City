using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Battle: When an enemy has faded out completely from death, remove it from the scene
public class Battle_EnemyDeathEndEvent : UnityEvent<int>
{
}
