using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Provides methods for button clicks
public class MainMenuMonitor : MonoBehaviour
{
    private void Awake()
    {
        Initializer.Run();
    }

    private void Start()
    {
        AudioManager.Intro();
    }
    public void Click_NewGame()
    {
        Initializer.ResetData();
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.MainToTown);
    }
    public void Click_LoadGame()
    {
        AudioManager.Chirp();
        MenuManager.GoToFileMenu(false);
    }
    public void Click_Options()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.MainOptions);
    }
    public void Click_Help()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.Help);
    }
    public void Click_About()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.About);
    }
    public void Click_Quit()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.Quit);
    }
}
