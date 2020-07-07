using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenuPanel : MonoBehaviour
{
    //[SerializeField]
    //Image heroImage = null;
    [SerializeField]
    Text heroNameText = null;
    [SerializeField]
    Slider hpSlider = null, mpSlider = null;
    [SerializeField]
    Text hpSliderText = null, mpSliderText = null;
    [SerializeField]
    Image heroImage = null;

    [SerializeField]
    protected int ID;
    protected GameObject parentMenuObject;
    GameObject prefabHeroStatsMenu;


    private void Start()
    {
        prefabHeroStatsMenu = Resources.Load<GameObject>(@"MenuPrefabs\prefabHeroStatsMenu");
    }

    public virtual void SetID(int id, GameObject parentGameObject)
    {
        ID = id;
        parentMenuObject = parentGameObject;

        RefreshPanel();
    }

    public void RefreshPanel()
    {
        BattleHero hero = BattleLoader.Party.Hero[ID];

        heroImage.sprite = hero.Sprites.Portrait;

        bool input = heroNameText.gameObject.TryGetComponent<InputField>(out InputField inputField);
        if (input) { inputField.text = hero.FullName; }
        else { heroNameText.text = hero.FullName; }
        hpSlider.maxValue = hero.HPMax;
        hpSlider.value = hero.HP;
        hpSliderText.text = hero.HP + " / " + hero.HPMax;
        mpSlider.maxValue = hero.MPMax;
        mpSlider.value = hero.MP;
        mpSliderText.text = hero.MP + " / " + hero.MPMax;
    }

    public void Click_Panel()
    {
        AudioManager.Chirp();
        GameObject obj = new GameObject();
        obj = GameObject.Instantiate(prefabHeroStatsMenu);
        obj.GetComponent<PartyMenuPanel>().SetID(ID, parentMenuObject);
        parentMenuObject.SetActive(false);
    }

    public void EnterName(string name)
    {
        BattleLoader.Party.Hero[ID].Rename(name);
    }


}
