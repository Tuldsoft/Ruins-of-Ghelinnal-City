using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BattleSubMenu : MonoBehaviour
{

    Battle_SubMenuSelectionEvent subMenuSelection = new Battle_SubMenuSelectionEvent();

    public abstract BattleMode Mode { get; }


    void Start()
    {
        EventManager.AddInvoker_Battle_SubMenuSelection(this);
    }

    protected void CloseSubMenu()
    {
        subMenuSelection.Invoke(Mode);
        gameObject.SetActive(false);
    }

    public void Click_Back()
    {
        AudioManager.Close();
        gameObject.SetActive(false);
    }

    public void AddListener_Battle_SubMenuSelection(UnityAction<BattleMode> listener)
    {
        subMenuSelection.AddListener(listener);
    }
}
