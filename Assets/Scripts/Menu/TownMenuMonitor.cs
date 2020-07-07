using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Provides methods for menu navigation
public class TownMenuMonitor : MonoBehaviour
{
    
    [SerializeField]
    GameObject healFlare = null;

    SpriteRenderer flareRenderer;
    
    bool needHealing = false;
    float flareGlowSpeed = 0.75f;
    int colorSignMultiplier = 1;

    private void Awake()
    {
        Initializer.Run();
    }

    private void Start()
    {
        flareRenderer = healFlare.gameObject.GetComponent<SpriteRenderer>();


        // Moved to MenuManager, so that the music doesn't reset from DungeonLevelMenu to Town
        //AudioManager.PlayMusic(AudioClipName.Music_Kids_Run_Through_The_City);
    }

    private void Update()
    {
        needHealing = false;
        foreach (BattleHero hero in BattleLoader.Party.Hero)
        {
            if (hero.HP < hero.HPMax || hero.MP < hero.MPMax)
            {
                needHealing = true;
            }
        }

        if (needHealing)
        {
            float elapsedTime = Time.deltaTime;
            Color healflareColor = flareRenderer.color;

            float flareness = Mathf.Clamp(healflareColor.a + (colorSignMultiplier * flareGlowSpeed * elapsedTime), 0, 0.5f);

            healflareColor.a = flareness;
            flareRenderer.color = healflareColor;
            
            if (flareness == 0 || flareness == 0.5f)
            {
                colorSignMultiplier *= -1;
            }
        }
        else
        {
            flareRenderer.color = new Color ( 1f, 1f, 1f, 0f );
        }

    }

    public void Click_ExploreRuins()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.TownExploreRuins);
    }
    public void Click_Talk()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.TownTalk);
    }
    public void Click_Shop()
    {
        AudioManager.Chirp();
        Shop.StockShop(BattleLoader.DungeonLevelAccess);
        MenuManager.GoToMenu(MenuName.TownShop);
    }
    public void Click_HealParty()
    {
        AudioManager.PlaySound(AudioClipName.Cure);
        for (int i = 0; i < BattleLoader.Party.Hero.Length; i++)
        {
            BattleLoader.Party.Hero[i].HealAll();
        }
    }

    public void Click_ManageParty()
    {
        AudioManager.Chirp();
        MenuManager.GoToMenu(MenuName.ManageParty);
    }
    public void Click_Save()
    {
        AudioManager.Chirp();
        MenuManager.GoToFileMenu(true);
    }
    public void Click_Load()
    {
        AudioManager.Chirp();
        MenuManager.GoToFileMenu(false);
    }
    public void Click_ReturnToMain()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.TownReturnToMain);
    }
}
