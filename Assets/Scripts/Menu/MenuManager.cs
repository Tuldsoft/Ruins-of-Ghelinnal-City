using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A static class for managing the travel between menus, so that one menu scene or prefab does
/// does not need to store a reference to the next item.
/// REFACTOR: Create a "Menu Stack" to track which menus are coming from where. Provide a "Close Menu"
/// class that destroys the current menu prefab and enables the previous menu, so that classes don't 
/// need to store that info through "Set"methods. 
/// REFACTOR: Provide an alternate GoToMenu(MenuName choice, int? index) that provides a means to 
/// pass a single int, such as a hero ID or item, etc.
/// REFACTOR: Provide an alternate GoToMenu(MenuName choice, bool boolean) that provides a means to 
/// pass a single bool (ex FileMenu, ChangeMusic, etc.)
/// </summary>
public static class MenuManager
{
    // Used choosing which dungeonLevel to load or reload
    static int dungeonLevel = 1;

    // Switchboard for most menu transitions
    public static void GoToMenu(MenuName choice)
    {
        switch (choice)
        {
            case MenuName.MainToTown:
                // Load the Town menu (but only from the Main Menu)                
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    AudioManager.PlayMusic(AudioClipName.Music_Kids_Run_Through_The_City);
                    SceneManager.LoadScene("Town");
                }
                // else no change because already in town.
                
                break;

            case MenuName.Load:
                // Show the FileMenu as a Load menu, called from Main Menu and from Town
                SceneManager.LoadScene("Town");
                break;

            case MenuName.MainOptions:
                // Show the Options menu, called from Main Menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabOptionsMenu"));
                break;

            case MenuName.About:
                // Show the About menu, called from Main Menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabAboutMenu"));
                break;

            case MenuName.Help:
                // Show the Help menu, called from Main Menu and Help
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabHelpMenu"));
                break;

            case MenuName.Quit:
                // Closes the entire application, called from Main Menu or pause menu
                Application.Quit();
                Debug.Log("You have quit the game.");
                break;

            case MenuName.TownTalk:
                // Shows the Talk Menu, called from Town
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabTalkMenu"));
                break;

            case MenuName.TownShop:
                // Shows the Shop menu, called from Town
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabShopMenu"));
                break;

            case MenuName.TownSave:
                // not longer used. See GoToFileMenu()
                // Object.Instantiate(Resources.Load("prefabShopMenu"));
                break;

            case MenuName.TownReturnToMain:
                // Shows the Main Menu, from Town
                SceneManager.LoadScene("MainMenu");
                break;

            case MenuName.TownExploreRuins:
                // Show the Dungeon selection menu, from Town
                SceneManager.LoadScene("Dungeon");
                break;

            case MenuName.DungeonLevel:
                // Loads a DungeonLevel scene
                SceneManager.LoadScene("DungeonLevel" + dungeonLevel);
                break;

            case MenuName.DungeonTown:
                // Loads the Town scene from the Dungeon selection screen
                // No change in audio if coming from dungeon selection
                SceneManager.LoadScene("Town");
                break;

            case MenuName.Battle:
                // Loads a Battle scene from a DungeonLevel. BattleLoader handles the rest.
                SceneManager.LoadScene("Battle");
                break;

            case MenuName.BattleToTown:
                // Loads the Town menu from Battle or from Dungeon (through a GameOver)
                SceneManager.LoadScene("Town");
                AudioManager.PlayMusic(AudioClipName.Music_Kids_Run_Through_The_City);
                dungeonLevel = 1;
                break;

            case MenuName.Pause:
                // Show a Pause menu. Called from DungeonLevel by pressing Esc.
                // GameOver can be considered a "Pause Menu". If not null, then a GameOver is being displayed
                if (BattleLoader.GameOver == null)
                {
                    Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabPauseMenu"));
                }
                break;

            case MenuName.GameOver:
                // Show a GameOver menu, one of three varieties. Called at death or end of battle
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabGameOverMenu"));
                break;

            case MenuName.ManageParty:
                // Show a ManagePartyMenu. Called from Town or from the Pause menu
                GameObject manageMenu = GameObject.Instantiate(
                    Resources.Load<GameObject>(@"MenuPrefabs\prefabManagePartyMenu"));
                // Store camera for adjusting size of components
                manageMenu.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                break;

            default:
                Debug.Log("Unknown menu command.");
                break;  
        }
    }

    // Stores dungeonLevel here and newLevel in BattleLoader, before changing scene to DungeonLevel
    public static void EnterDungeonLevel (int level, bool newLevel) 
    {
        dungeonLevel = level;
        BattleLoader.NewDungeonLevel = newLevel;
        GoToMenu(MenuName.DungeonLevel);
    }

    // Shows a FileMenu, either as a Save or a Load menu.
    public static void GoToFileMenu (bool asSave)
    {
        GameObject fileMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabFileMenu"));

        fileMenu.GetComponent<FileMenuMonitor>().SetupFileMenu(asSave);
    }

    
}
