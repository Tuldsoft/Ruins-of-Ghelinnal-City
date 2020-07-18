using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Battle: Tells the BattleManager that the end of a turn has arrived.
public class Battle_TurnOverEvent : UnityEvent<Damage, int>
{
}
