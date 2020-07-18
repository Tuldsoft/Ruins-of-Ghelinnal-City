using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Battle: A call to update a display of HP and MP Sliders, either enemy or hero
// battleid, hpslider reduction, mpslider reduction
public class Battle_RefreshHPMPSlidersEvent : UnityEvent<int, int, int>
{
}
