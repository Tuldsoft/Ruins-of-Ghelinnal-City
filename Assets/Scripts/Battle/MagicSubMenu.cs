using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSubMenu : BattleSubMenu
{
    BattleMode mode = BattleMode.Magic_Cure;

    public override BattleMode Mode { get { return mode; } }
    HeroType type = HeroType.Sabin;
    bool initialized = false;

    [SerializeField]
    Text spellText = null;

    const float SecondColumnXOffset = 150f;

    //Vector2 firstPosition = new Vector2(-485, -135);
    //Vector2 backPosition = new Vector2(-335, -285);
    

    Vector2[] positions = new Vector2[6];
    List<Text> spellTexts = new List<Text>();

    private void OnEnable()
    {
        SetSpells(type);
    }

    public void Initialize(HeroType type)
    {
        this.type = type;

        if (!initialized) 
        {
            initialized = true;

            float height = spellText.rectTransform.rect.height * spellText.rectTransform.localScale.y;

            positions[0] = spellText.transform.localPosition;
            positions[1] = new Vector2(positions[0].x, positions[0].y - height);
            positions[2] = new Vector2(positions[0].x, positions[0].y - (2 * height));
            positions[3] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y);
            positions[4] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y - height);
            positions[5] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y - (2 * height));

            Transform parentTransform = spellText.transform.parent.transform;

            //List<string> spellNames = new List<string>();
            //List<Text> spellTexts = new List<Text>();

            foreach(BattleMode mode in BattleMode.GetValues(typeof(BattleMode)))
            {
                if (mode.ToString().StartsWith("Magic_"))
                {
                    string name = mode.ToString().Substring(6);
                    //spellNames.Add(name);
                    //spellModes.Add(mode);

                    GameObject newText = GameObject.Instantiate(spellText.gameObject);
                    newText.transform.SetParent(parentTransform);
                    newText.transform.localScale = spellText.transform.localScale;
                    newText.GetComponent<Text>().text = name;
                    
                    newText.GetComponent<SpellClick>().SetMode(mode, this);

                    spellTexts.Add(newText.GetComponent<Text>());

                }
            }

            Destroy(spellText.gameObject);

            for (int i = 0; i < spellTexts.Count; i++)
            {
                spellText.transform.localPosition = positions[i];
            }

/*
            initialized = true;
            List<Text> texts = new List<Text>();
            gameObject.GetComponentsInChildren<Text>(true, texts);
            foreach (Text text in texts)
            {
                if (text.gameObject.name == "CureText") { spellTexts = text; }
                if (text.gameObject.name == "FireballText") { fireballText = text; }
            }
            float height = spellTexts.rectTransform.rect.height * spellTexts.rectTransform.localScale.y;

            positions[0] = firstPosition;
            positions[1] = new Vector2(firstPosition.x, firstPosition.y - height);
            positions[2] = new Vector2(firstPosition.x, firstPosition.y - (2 * height));
            positions[3] = new Vector2(backPosition.x, firstPosition.y);
            positions[4] = new Vector2(backPosition.x, firstPosition.y - height);
            positions[5] = new Vector2(backPosition.x, firstPosition.y - (2 * height));*/
        }
    }



    public void SetSpells(HeroType type)
    {
        Initialize(type);
        
        List<BattleMode> enableModes = new List<BattleMode>();
        switch (type)
        {
            case HeroType.Celes:
                enableModes.Add(BattleMode.Magic_Cure);
                enableModes.Add(BattleMode.Magic_Heal);
                enableModes.Add(BattleMode.Magic_Fireball);
                enableModes.Add(BattleMode.Magic_Poison);
                break;
            case HeroType.Locke:
                enableModes.Add(BattleMode.Magic_Cure);
                enableModes.Add(BattleMode.Magic_Poison);
                break;
            default:
                enableModes.Add(BattleMode.Magic_Cure);
                break;
        }

        int i = 0;
        foreach (Text spellText in spellTexts)
        {
            BattleMode mode = spellText.gameObject.GetComponent<SpellClick>().Mode;
            
            if (enableModes.Contains(mode)) 
            {
                spellText.gameObject.SetActive(true);
                spellText.gameObject.transform.localPosition = positions[i];
                i++;
            }
            else
            {
                spellText.gameObject.SetActive(false);
            }
        }

        //int i = 0;
        //i += CheckEnable(enableModes, BattleMode.Magic_Cure, spellTexts.gameObject, i) ? 1 : 0;
        //i += CheckEnable(enableModes, BattleMode.Magic_Fireball, fireballText.gameObject, i) ? 1 : 0;
        
        /*if (enableMode.Contains(BattleMode.Magic_Cure))
        {
            cureText.transform.position = positions[i];
            cureText.gameObject.SetActive(true);
            i++;
        }
        else { cureText.gameObject.SetActive(false); }

        if (enableMode.Contains(BattleMode.Magic_Fireball))
        {
            fireballText.transform.position = positions[i];
            fireballText.enabled = true;
            //i++;
        }
        else { fireballText.enabled = false; }*/

    }

    bool CheckEnable (List<BattleMode> modes, BattleMode mode, GameObject gameObj, int positionNum)
    {
        if (modes.Contains(mode))
        {
            gameObj.transform.localPosition = positions[positionNum];
            gameObj.gameObject.SetActive(true);
            return true;
        }
        else 
        { 
            gameObj.SetActive(false);
            return false;
        }

    }

    public void Click_Spell(BattleMode mode)
    {
        this.mode = mode;
        base.CloseSubMenu();
    }

    public void Click_Cure()
    {
        mode = BattleMode.Magic_Cure;
        base.CloseSubMenu();
    }

    public void Click_Fireball()
    {
        mode = BattleMode.Magic_Fireball;
        base.CloseSubMenu();
    }

}
