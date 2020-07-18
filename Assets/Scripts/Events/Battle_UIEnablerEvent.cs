using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Enables or disables UI elements, for example, during an enemy turn
public class Battle_UIEnablerEvent : UnityEvent<bool, HeroType>
{
}
