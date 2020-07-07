using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HirePanelMonitor : MonoBehaviour
{
    [SerializeField]
    Text goldText = null;

    [SerializeField]
    GameObject hireEdgar = null, hireCeles = null, hireLocke = null;
    [SerializeField]
    Button hireEdgarButton = null, hireCelesButton = null, hireLockeButton = null;

    int[] purchaseAmounts = new int[] { 0, 500, 2500, 12500 };

    Vector2[] panelLocations = new Vector2[] {Vector2.zero, new Vector2(277f,169f),
        new Vector2(-277f,-61f), new Vector2(277f,-61f) };

    Color goldColor = new Color(1f, 0.9495088f, 0.6235294f);

    ManagePartyMenuMonitor menuMonitor;

    public void SetPanel(ManagePartyMenuMonitor menuMonitor)
    {
        this.menuMonitor = menuMonitor;
        
        int partySize = BattleLoader.Party.Hero.Length;
        if (partySize < 4)
        {
            int amount = purchaseAmounts[partySize];
            goldText.text = amount.ToString();
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

            gameObject.transform.localPosition = panelLocations[partySize];

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
        {
            gameObject.SetActive(false);
        }

    }

    public void Click_HireEdgar()
    {
        AudioManager.Chirp();
        Hire(HeroType.Edgar);
    }

    public void Click_HireCeles()
    {
        AudioManager.Chirp();
        Hire(HeroType.Celes);
    }

    public void Click_HireLocke()
    {
        AudioManager.Chirp();
        Hire(HeroType.Locke);
    }

    void Hire(HeroType type)
    {
        int partySize = BattleLoader.Party.Hero.Length; 
        int amount = purchaseAmounts[partySize];

        BattleLoader.Party.Gold -= amount;

        BattleLoader.Party.AddHero(new BattleHero(type, new BattleStats(), null,
            "Hero" + (BattleLoader.Party.Hero.Length + 1)));

        menuMonitor.Start();
    }
}
