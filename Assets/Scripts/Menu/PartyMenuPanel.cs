using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the four panels in prefabManagePartyMenu. Contains references to the
/// panel's Name, HP, MP, and portrait, which are updated according to the panel's hero.
/// HeroStatsPanel inherits this class because it displays all the same info and more.
/// </summary>
public class PartyMenuPanel : MonoBehaviour
{
    // Objects for updating through code later
    [SerializeField]
    Text heroNameText = null;
    [SerializeField]
    Slider hpSlider = null, mpSlider = null;
    [SerializeField]
    Text hpSliderText = null, mpSliderText = null;
    [SerializeField]
    Image heroImage = null;

    // ID can be set through a SetID() method if the panel is created while running,
    // or in the inspector if created in the editor.
    [SerializeField]
    protected int ID;

    // A reference to the parent of the panel
    protected GameObject parentMenuObject;

    // The menu object that this panel creates when clicked
    GameObject prefabHeroStatsMenu; // this can safely be moved to the Click_ method, doing away with Start()

    // Start is called before the first frame update
    private void Start()
    {
        prefabHeroStatsMenu = Resources.Load<GameObject>(@"MenuPrefabs\prefabHeroStatsMenu");
    }

    // If the panel created at runtime, set the ID and parent object with this method
    public virtual void SetID(int id, GameObject parentGameObject)
    {
        ID = id;
        parentMenuObject = parentGameObject;

        RefreshPanel();
    }

    // Show the current state of the referenced hero with sliders and labels
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

    // Called by the panel's invisible button, On_Click()
    public void Click_Panel()
    {
        AudioManager.Chirp();
        GameObject obj = new GameObject();
        obj = GameObject.Instantiate(prefabHeroStatsMenu);
        obj.GetComponent<PartyMenuPanel>().SetID(ID, parentMenuObject);
        parentMenuObject.SetActive(false);
    }

    // The HeroNameText gameObject can be altered. When altered, the new name is stored.
    public void EnterName(string name)
    {
        BattleLoader.Party.Hero[ID].Rename(name);
    }


}
