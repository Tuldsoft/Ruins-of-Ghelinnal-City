using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attached as a component to any script that needs to invoke TurnOver events.
/// 
/// Used by BattleInventory to communicate to BattleManager - may be redudant now.
/// </summary>

public class TurnInvoker : MonoBehaviour
{

    Battle_TurnOverEvent turnOver = new Battle_TurnOverEvent();

    // Start is run before the first frame update
    private void Start()
    {
        EventManager.AddInvoker_Battle_TurnOver(this);
    }

    // Sends out the ID of the turn that just ended, and any damage to display on that ID
    public void TurnOver(Damage damage, int battleID)
    {
        turnOver.Invoke(damage, battleID);
    }

    // Used by others to recognize this as an invoker
    public void AddListener_Battle_TurnOver (UnityAction<Damage, int> listener)
    {
        turnOver.AddListener(listener);
    }

}
