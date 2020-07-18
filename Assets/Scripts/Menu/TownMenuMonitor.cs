using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the main camera in the Town scene. Contains methods for each button in the Town.
/// Has a feature where if the party is in need of healing, displays a pulsing flare over the
/// "Heal Party" button.
/// </summary>
public class TownMenuMonitor : MonoBehaviour
{
    #region Flare fields
    [SerializeField]
    GameObject healFlare = null;

    SpriteRenderer flareRenderer;
    
    bool needHealing = false;
    float flareGlowSpeed = 0.75f;
    int colorSignMultiplier = 1;
    #endregion

    #region Unity Methods
    // Awake() is called before Start()
    private void Awake()
    {
        Initializer.Run();
    }

    // Start() is called before the first frame update.
    private void Start()
    {
        // Store a reference to the flare's SpriteRenderer
        flareRenderer = healFlare.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update() is called once per frame
    private void Update()
    {
        // Determine if healing is needed (HP or MP is not at max)
        needHealing = false;
        foreach (BattleHero hero in BattleLoader.Party.Hero)
        {
            if (hero.HP < hero.HPMax || hero.MP < hero.MPMax)
            {
                needHealing = true;
            }
        }

        // Display and update the healing flare
        if (needHealing)
        {
            float elapsedTime = Time.deltaTime;
            Color healflareColor = flareRenderer.color;

            // Increase or decrease the transparency of the flare, from 0% to 50%
            float flareness = Mathf.Clamp(healflareColor.a + (colorSignMultiplier * flareGlowSpeed * elapsedTime), 0, 0.5f);

            // Set the flare transparency
            healflareColor.a = flareness;
            flareRenderer.color = healflareColor;
            
            // If flare was clamped, reverse transparency direction
            if (flareness == 0 || flareness == 0.5f)
            {
                colorSignMultiplier *= -1;
            }
        }
        else
        {
            // if HP and MP are 100%, set flare to fully transparent
            flareRenderer.color = new Color ( 1f, 1f, 1f, 0f );
        }
    }
    #endregion

    #region Click Methods
    // Each of these methods is called via a gameObject's On_Click()
    // Most open another menu, either via a prefab or by another scene.
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
        // Heals the entire party to max HP and max MP
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
        MenuManager.GoToFileMenu(true); // asSave = true
    }
    public void Click_Load()
    {
        AudioManager.Chirp();
        MenuManager.GoToFileMenu(false); // asSave = false
    }
    public void Click_ReturnToMain()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.TownReturnToMain);
    }
    #endregion
}
