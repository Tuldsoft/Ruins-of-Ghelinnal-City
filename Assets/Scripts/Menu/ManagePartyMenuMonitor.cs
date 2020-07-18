using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabManagePartyMenu. Called from the Town Menu and Pause menu. Displays
/// four panels for the (up to) four heros. Each panel has a PartyMenuPanel for the displaying
/// of a hero's name, HP and MP, and when clicked, generates a HeroStatsMenu.
/// </summary>

public class ManagePartyMenuMonitor : MonoBehaviour
{
    // References to the four hero panels, set in the inspector
    [SerializeField]
    Image heroPanel1 = null, heroPanel2 = null, heroPanel3 = null, heroPanel4 = null;

    // Reference to the hire panel, which may be covering one of the four panels
    [SerializeField]
    GameObject hirePanel = null;

    // A reference to the pauseMenu, in case the ManageParty menu is called from there.
    GameObject pauseMenu = null;

    // Start() is called before the first frame update
    public void Start()
    {
        // Enable panels for display according to partySize
        int partySize = BattleLoader.Party.Hero.Length;
        heroPanel1.GetComponent<PartyMenuPanel>().SetID(0, gameObject);
        if (partySize > 1)
        {
            heroPanel2.GetComponent<PartyMenuPanel>().SetID(1, gameObject);
            heroPanel2.gameObject.SetActive(true);
        }
        if (partySize > 2)
        {
            heroPanel3.GetComponent<PartyMenuPanel>().SetID(2, gameObject);
            heroPanel3.gameObject.SetActive(true);
        }
        if (partySize > 3)
        {
            heroPanel4.GetComponent<PartyMenuPanel>().SetID(3, gameObject);
            heroPanel4.gameObject.SetActive(true);
        }

        // Tell hirepanel about this (to avoid making another event)
        hirePanel.GetComponent<HirePanelMonitor>().SetPanel(this);
    }

    // Only called from the PauseMenu, otherwise pauseMenu is left null.
    public void SetPauseMenu(GameObject pause)
    {
        pauseMenu = pause;
    }
    
    // When the Close button On_Click()
    public void Click_Close()
    {
        AudioManager.Close();
        
        // if coming from the PauseMenu, it needs to be set to active again
        if (pauseMenu != null) { pauseMenu.SetActive(true); }
        // Destroy this menu
        Destroy(gameObject);
    }

}
