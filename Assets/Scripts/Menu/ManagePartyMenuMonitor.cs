using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagePartyMenuMonitor : MonoBehaviour
{
    [SerializeField]
    Image heroPanel1 = null, heroPanel2 = null, heroPanel3 = null, heroPanel4 = null;

    [SerializeField]
    GameObject hirePanel = null;

    GameObject pauseMenu = null;

    public void Start()
    {
        
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

        hirePanel.GetComponent<HirePanelMonitor>().SetPanel(this);

    }

    public void SetPauseMenu(GameObject pause)
    {
        pauseMenu = pause;
    }
    
           
    public void Click_Close()
    {
        AudioManager.Close();
        if (pauseMenu != null) { pauseMenu.SetActive(true); }
        Destroy(gameObject);
    }

}
