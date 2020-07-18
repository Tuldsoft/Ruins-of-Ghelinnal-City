using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to each hero prefab.
/// A variant of DamageDisplay that replaces references to HP Sliders. For an enemy,
/// the HP Slider is attached to the enemy prefab. Here, the BattleManager tells 
/// HeroDamageDisplay what the container gameObject is, and includes a "HP/HPMAX" label.
/// REFACTOR: Eliminate some redundant code
/// </summary>

public class HeroDamageDisplay : DamageDisplay
{
    #region Fields and Properties

    // Label that displays "HP / HPMax" or "MP / MPMax"
    Text hpText;
    Text mpText;

    public Slider HPSlider { get { return hpSlider; } }
    public Slider MPSlider { get { return mpSlider; } }

    #endregion

    #region Methods
    // Stores a reference for the container gameObject of sliders, name, and labels
    public void SetHeroStatusBar(GameObject heroStatusBar)
    {
        hpmpsliders = heroStatusBar;
        hpmpsliders.SetActive(true);
    }

    // Finds and sets up HP and MP Sliders. Called by parent DamageDisplay
    protected override void SetSliders(int hp, int hpMax, int mp, int mpMax)
    {
        // Find HP and MPSlider
        Slider[] sliders = hpmpsliders.GetComponentsInChildren<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].gameObject.name == "HPSlider") { hpSlider = sliders[i]; }
            if (sliders[i].gameObject.name == "MPSlider") { mpSlider = sliders[i]; }
        }

        // Set values of sliders
        hpSlider.maxValue = hpMax;
        mpSlider.maxValue = mpMax;
        hpSlider.value = hp;
        hpSlider.value = mp;

        // REFACTOR: Move this to SetNameAndHPMP
        // Find hp/mp labels
        Text[] texts = hpmpsliders.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "HPText") { hpText = texts[i]; }
            if (texts[i].name == "MPText") { mpText = texts[i]; }
        }

        // Set text of labels
        hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
        mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();
    }

    // Set name and current slider parameters. Called once by BattleManager (and again when cheating)
    public void SetNameAndHPMP(string name, int hp, int mp)
    {
        Text[] nameText = hpmpsliders.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < nameText.Length; i++)
        {
            if (nameText[i].gameObject.name == "HeroNameText")
            {
                nameText[i].text = name;
            }
        }

        hpSlider.value = hp;
        mpSlider.value = mp;
        hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
        mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();
    }

    // Now unnecessary?
    protected override void DisplayDamage(int id, Damage damage)
    {
        base.DisplayDamage(id, damage);

        //if (id < 0 && damage != null)
        {
            //hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
            //mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();
        }
    }

    // Same as regular Refresh, but includes labels. Used by DamageDisplay and HeroDamageDisplay
    protected override void RefreshHPMPSliders(int battleID, int hpReduction, int mpReduction)
    {
        base.RefreshHPMPSliders(battleID,hpReduction,mpReduction);

        hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
        mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();

    }
    #endregion
}
