using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuManager
{
    static int dungeonLevel = 1;

    public static void GoToMenu(MenuName choice)
    {
        switch (choice)
        {
            case MenuName.MainToTown:
                
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    AudioManager.PlayMusic(AudioClipName.Music_Kids_Run_Through_The_City);
                    SceneManager.LoadScene("Town");
                }
                // else no change because already in town.
                
                break;

            case MenuName.Load:
                // Called from MainMenu, Town
                // instantiate the prefab Load menu
                // Object.Instantiate(Resources.Load("prefabLoadMenu"));

                SceneManager.LoadScene("Town"); // remove later
                break;

            case MenuName.MainOptions:
                // instantiate the prefab Options menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabOptionsMenu"));
                break;

            case MenuName.About:
                // instantiate the prefab About menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabAboutMenu"));
                break;

            case MenuName.Help:
                // Called from MainMenu, Pause

                // instantiate the prefab Help Menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabHelpMenu"));
                break;

            case MenuName.Quit:
                // Called from MainMenu, Town, Pause

                Application.Quit();
                Debug.Log("You have quit the game.");
                break;

            case MenuName.TownTalk:
                // instantiate the prefab Talk dialog box
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabTalkMenu"));
                break;

            case MenuName.TownShop:
                // instantiate the prefab Shop Menu
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabShopMenu"));
                break;

            case MenuName.TownSave:
                // instantiate the prefab Save Menu
                // Object.Instantiate(Resources.Load("prefabShopMenu"));
                break;

            case MenuName.TownReturnToMain:
                SceneManager.LoadScene("MainMenu");
                break;

            case MenuName.TownExploreRuins:
                SceneManager.LoadScene("Dungeon");
                break;

            case MenuName.DungeonLevel:
                SceneManager.LoadScene("DungeonLevel" + dungeonLevel);
                // battleloader load level (level)
                break;

            case MenuName.DungeonTown:
                // no change in audio if coming from dungeon selection
                
                

                SceneManager.LoadScene("Town");
                break;

            case MenuName.Battle:
                SceneManager.LoadScene("Battle");
                break;

            case MenuName.BattleToTown:
                SceneManager.LoadScene("Town");
                //SceneManager.UnloadSceneAsync("Battle");
                //SceneManager.UnloadSceneAsync("DungeonLevel" + dungeonLevel);
                AudioManager.PlayMusic(AudioClipName.Music_Kids_Run_Through_The_City);
                dungeonLevel = 1;
                break;

            case MenuName.Pause:
                if (BattleLoader.GameOver == null)
                {
                    Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabPauseMenu"));
                }
                break;

            case MenuName.GameOver:
                Object.Instantiate(Resources.Load(@"MenuPrefabs\prefabGameOverMenu"));
                break;

            case MenuName.ManageParty:
                GameObject manageMenu = GameObject.Instantiate(
                    Resources.Load<GameObject>(@"MenuPrefabs\prefabManagePartyMenu"));
                manageMenu.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
                break;

            default:
                Debug.Log("Unknown menu command.");
                break;  
        }
    }

    public static void EnterDungeonLevel (int level, bool newLevel) 
    {
        dungeonLevel = level;
        BattleLoader.NewDungeonLevel = newLevel;
        GoToMenu(MenuName.DungeonLevel);
    }

    public static void GoToFileMenu (bool asSave)
    {
        GameObject fileMenu = GameObject.Instantiate(
            Resources.Load<GameObject>(@"MenuPrefabs\prefabFileMenu"));

        fileMenu.GetComponent<FileMenuMonitor>().SetupFileMenu(asSave);
    }

    
}
