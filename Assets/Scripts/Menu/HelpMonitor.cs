using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMonitor : MonoBehaviour
{
    [SerializeField]
    Text mainMenuText = null, inTownText = null, pickLevelText = null, 
        inDungeonText = null, inBattleText = null, specialAbilitiesText = null;
    
    enum Tabs { main, town, level, dungeon, battle, abilities }

    private void Start()
    {
        AudioManager.Chirp();
        Click_MainMenu();
    }

    void Toggle (Tabs tab)
    {
        mainMenuText.gameObject.SetActive(tab == Tabs.main ? true : false);
        inTownText.gameObject.SetActive(tab == Tabs.town ? true : false);
        pickLevelText.gameObject.SetActive(tab == Tabs.level ? true : false);
        inDungeonText.gameObject.SetActive(tab == Tabs.dungeon ? true : false);
        inBattleText.gameObject.SetActive(tab == Tabs.battle ? true : false);
        specialAbilitiesText.gameObject.SetActive(tab == Tabs.abilities ? true : false);
    }

    public void Click_MainMenu()
    {
        AudioManager.Chirp();
        Toggle(Tabs.main);
    }
    public void Click_InTown()
    {
        AudioManager.Chirp();
        Toggle(Tabs.town);
    }
    public void Click_PickLevel()
    {
        AudioManager.Chirp();
        Toggle(Tabs.level);
    }
    public void Click_InDungeon()
    {
        AudioManager.Chirp();
        Toggle(Tabs.dungeon);
    }
    public void Click_InBattle()
    {
        AudioManager.Chirp();
        Toggle(Tabs.battle);
    }
    public void Click_SpecialAbilities()
    {
        AudioManager.Chirp();
        Toggle(Tabs.abilities);
    }

    public void Click_CloseButton() 
    {
        AudioManager.Chirp();
        Destroy(gameObject);
    }
}
