using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// attach as a component to anything that needs to invoke Battle_HeroTurnOverEvent
// Use initially to signal the end of the Hero turn, later, broaden to every hero and enemy
// Also expand to include either start or end turn as bool
// use different methods for different invocations?

 // passes id and damage done, if any (healing is negative damage)


//attach this component at run time like EventTimer


public class TurnInvoker : MonoBehaviour
{
    //  store and pass the ID of the creature whose turn is over
    //-1, -2, -3, or -4 for heroes, 0+ for enemies
    // int ID = -1;

    // bool isEndTurn;

    Battle_TurnOverEvent turnOver = new Battle_TurnOverEvent();

    private void Start()
    {
        EventManager.AddInvoker_Battle_TurnOver(this);
    }

    // sends out the ID of the turn that just ended, and any damage to display on that ID
    public void TurnOver(Damage damage, int battleID)
    {
        turnOver.Invoke(damage, battleID);
    }

    public void AddListener_Battle_TurnOver (UnityAction<Damage, int> listener)
    {
        turnOver.AddListener(listener);
    }

}
