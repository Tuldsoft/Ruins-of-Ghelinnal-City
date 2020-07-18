using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Battle: A propelled object has hit its intended target
public class Battle_PropelHitTargetEvent : UnityEvent<int, int, BattleMode>
{
}
