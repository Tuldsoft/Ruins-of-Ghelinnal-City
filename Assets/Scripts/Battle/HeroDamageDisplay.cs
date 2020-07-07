using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDamageDisplay : DamageDisplay
{

    [SerializeField]
    //GameObject heroStatusBar = null;

    Text hpText;
    Text mpText;

    public Slider HPSlider { get { return hpSlider; } }
    public Slider MPSlider { get { return mpSlider; } }

    public void SetHeroStatusBar(GameObject heroStatusBar)
    {
        hpmpsliders = heroStatusBar;
        hpmpsliders.SetActive(true);
    }

    protected override void SetSliders(int hp, int hpMax, int mp, int mpMax)
    {
        //hpmpsliders = heroStatusBar;

        Slider[] sliders = hpmpsliders.GetComponentsInChildren<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].gameObject.name == "HPSlider") { hpSlider = sliders[i]; }
            if (sliders[i].gameObject.name == "MPSlider") { mpSlider = sliders[i]; }
        }

        hpSlider.maxValue = hpMax;
        mpSlider.maxValue = mpMax;
        hpSlider.value = hp;
        hpSlider.value = mp;

        Text[] texts = hpmpsliders.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "HPText") { hpText = texts[i]; }
            if (texts[i].name == "MPText") { mpText = texts[i]; }
        }

        hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
        mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();


    }

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

    protected override void DisplayDamage(int id, Damage damage)
    {
        base.DisplayDamage(id, damage);

        //if (id < 0 && damage != null)
        {
            //hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
            //mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();
        }
    }
    protected override void RefreshHPMPSliders(int battleID, int hpReduction, int mpReduction)
    {
        base.RefreshHPMPSliders(battleID,hpReduction,mpReduction);

        hpText.text = hpSlider.value.ToString() + "/" + hpSlider.maxValue.ToString();
        mpText.text = mpSlider.value.ToString() + "/" + mpSlider.maxValue.ToString();

    }
}
