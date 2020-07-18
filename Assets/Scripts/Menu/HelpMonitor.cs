using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabHelpMenu. The Help Menu has tabs that show and hide blocks of text,
/// explaining how the game works. The text is pre-written into each text gameObject, using the editor.
/// </summary>
public class HelpMonitor : MonoBehaviour
{
    // Text objects for each tab
    [SerializeField]
    Text mainMenuText = null, inTownText = null, pickLevelText = null, 
        inDungeonText = null, inBattleText = null, specialAbilitiesText = null;
    
    // Names of the tabs
    enum Tabs { main, town, level, dungeon, battle, abilities }

    // Start() is called before the first frame update
    private void Start()
    {
        AudioManager.Chirp();
        Click_MainMenu(); // Simulate clicking the first tab
    }

    // Shows or displays a block of text. Called with a parameter by each Click_ method
    void Toggle (Tabs tab)
    {
        mainMenuText.gameObject.SetActive(tab == Tabs.main ? true : false);
        inTownText.gameObject.SetActive(tab == Tabs.town ? true : false);
        pickLevelText.gameObject.SetActive(tab == Tabs.level ? true : false);
        inDungeonText.gameObject.SetActive(tab == Tabs.dungeon ? true : false);
        inBattleText.gameObject.SetActive(tab == Tabs.battle ? true : false);
        specialAbilitiesText.gameObject.SetActive(tab == Tabs.abilities ? true : false);
    }

    // Displays the initial help screen
    public void Click_MainMenu()
    {
        AudioManager.Chirp();
        Toggle(Tabs.main);
    }

    // Displays help for in town activites
    public void Click_InTown()
    {
        AudioManager.Chirp();
        Toggle(Tabs.town);
    }

    // Displays help for choosing the next level of dungeon, and how to advance
    public void Click_PickLevel()
    {
        AudioManager.Chirp();
        Toggle(Tabs.level);
    }

    // Displays help for navigating dungeon levels
    public void Click_InDungeon()
    {
        AudioManager.Chirp();
        Toggle(Tabs.dungeon);
    }

    // Displays help for doing battle
    public void Click_InBattle()
    {
        AudioManager.Chirp();
        Toggle(Tabs.battle);
    }

    // Explains some abilities
    public void Click_SpecialAbilities()
    {
        AudioManager.Chirp();
        Toggle(Tabs.abilities);
    }

    // Closes the help menu
    public void Click_CloseButton() 
    {
        AudioManager.Chirp();
        Destroy(gameObject);
    }
}
