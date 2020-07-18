using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to a HirePanel in the ManagePartyMenu. This HirePanel is displayed instead of a
/// PartyMenuPanel for any hero that has not been added yet. There is only ever one HirePanel,
/// that moves as heroes are hired. It supplies a choice of up to three different heroes for
/// hire. THe price to hire a hero increases with each hire.
/// </summary>
public class HirePanelMonitor : MonoBehaviour
{
    #region Fields
    // Cost to hire a new hero
    [SerializeField]
    Text goldText = null;

    // Three images of new recruits, and the buttons used to hire them
    [SerializeField]
    GameObject hireEdgar = null, hireCeles = null, hireLocke = null;
    [SerializeField]
    Button hireEdgarButton = null, hireCelesButton = null, hireLockeButton = null;

    // Cost to purchase new heroes (Hero[0] costs 0, because that's Sabin)
    int[] purchaseAmounts = new int[] { 0, 500, 2500, 12500 };

    // Offsets for moving the hirePanel
    Vector2[] panelLocations = new Vector2[] {Vector2.zero, new Vector2(277f,169f),
        new Vector2(-277f,-61f), new Vector2(277f,-61f) };

    // Color of gold text
    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    // A reference to the ManagePartyMenuMonitor, which controls this and the the hero panels
    ManagePartyMenuMonitor menuMonitor;
    #endregion

    #region Methods
    // Sets up the HirePanel, called by the ManagePartyMenuMonitor
    public void SetPanel(ManagePartyMenuMonitor menuMonitor)
    {
        this.menuMonitor = menuMonitor;
        
        // If party size < 4, move the panel to the correct place and set its display
        int partySize = BattleLoader.Party.Hero.Length;
        if (partySize < 4)
        {
            // Update cost
            int amount = purchaseAmounts[partySize];
            goldText.text = amount.ToString();
            
            // If cost > what the party has, no hire buttons can be clicked
            if (amount > BattleLoader.Party.Gold)
            {
                goldText.color = Color.red;
                hireEdgarButton.interactable = false;
                hireLockeButton.interactable = false;
                hireCelesButton.interactable = false;
            }
            else
            {
                goldText.color = goldColor;
                hireEdgarButton.interactable = true;
                hireLockeButton.interactable = true;
                hireCelesButton.interactable = true;
            }

            // Reposition HirePanel
            gameObject.transform.localPosition = panelLocations[partySize];

            // Remove any hirable heroes already in the party
            foreach (BattleHero hero in BattleLoader.Party.Hero)
            {
                if (hero.HeroType == HeroType.Edgar)
                {
                    hireEdgar.SetActive(false);
                }
                if (hero.HeroType == HeroType.Locke)
                {
                    hireLocke.SetActive(false);
                }
                if (hero.HeroType == HeroType.Celes)
                {
                    hireCeles.SetActive(false);
                }
            }
        }
        else
        // If party size is already 4, then disable the HirePanel entirely
        {
            gameObject.SetActive(false);
        }

    }

    // Hires Edgar when the Edgar button's On_Click()
    public void Click_HireEdgar()
    {
        AudioManager.Chirp();
        Hire(HeroType.Edgar);
    }

    // Hires Celes when the Celes button's On_Click()
    public void Click_HireCeles()
    {
        AudioManager.Chirp();
        Hire(HeroType.Celes);
    }

    // Hires Locke when the Locke button's On_Click()
    public void Click_HireLocke()
    {
        AudioManager.Chirp();
        Hire(HeroType.Locke);
    }

    // Called by one of the three hire buttons (portraits of new heroes).
    // Hires that hero by deducting gold fromt he party, then creating the new hero.
    void Hire(HeroType type)
    {
        // Retrieve size and amount
        int partySize = BattleLoader.Party.Hero.Length; 
        int amount = purchaseAmounts[partySize];

        // Deduct cost
        BattleLoader.Party.Gold -= amount;

        // Create and add a new hero to the party
        BattleLoader.Party.AddHero(new BattleHero(type, new BattleStats(), null,
            "Hero" + (BattleLoader.Party.Hero.Length + 1)));

        // Restart (refresh) the entire ManagePartyMenu
        menuMonitor.Start();
    }
    #endregion
}
