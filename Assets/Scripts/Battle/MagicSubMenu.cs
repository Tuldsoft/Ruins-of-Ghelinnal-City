using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A SubMenu for Magic options. Fills the box with options based on the active hero.
/// Uses a SpellClick class to discern which spell is clicked.
/// </summary>

public class MagicSubMenu : BattleSubMenu
{
    #region Fields and Properties
    // Mode will determine which spell is cast upon click
    BattleMode mode = BattleMode.Magic_Cure;
    public override BattleMode Mode { get { return mode; } }
    
    // MagicSubMenu reconfigures itself base on who is casting magic
    HeroType type = HeroType.Sabin;
    bool initialized = false;

    // Text GameObject, to be used as a template. Positioned in the first position.
    [SerializeField]
    Text spellText = null;

    // Two columns of spells, second column is to the right by this amount
    const float SecondColumnXOffset = 150f;

    // Up to 6 positions (more can be added later through use of a More and Back button).
    Vector2[] positions = new Vector2[6];
    // Expandable list of objects
    List<Text> spellTexts = new List<Text>();
    #endregion

    // Called when using .SetActive(true). Called by BattleManager.Click_Magic()
    protected override void OnEnable()
    {
        base.OnEnable();
        SetSpells(type);
    }

    // Called just before being set active. Called by BattleManager.Click_Magic()
    // Used once in whole, but subsequently only sets HeroType.
    // Requires a HeroType to know what spells to set.
    public void Initialize(HeroType type)
    {
        // Set type before setting active.
        this.type = type;

        // The rest is only performed once.
        if (!initialized) 
        {
            initialized = true;

            // Collect height from template
            float height = spellText.rectTransform.rect.height * spellText.rectTransform.localScale.y;

            // define 6 positions based on height and onset, for later use
            positions[0] = spellText.transform.localPosition;
            positions[1] = new Vector2(positions[0].x, positions[0].y - height);
            positions[2] = new Vector2(positions[0].x, positions[0].y - (2 * height));
            positions[3] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y);
            positions[4] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y - height);
            positions[5] = new Vector2(positions[0].x + SecondColumnXOffset, positions[0].y - (2 * height));

            // Store reference to parentTransform
            Transform parentTransform = spellText.transform.parent.transform;

            // Create a text object for each spell in the BattleMode enum, using spellText as template
            foreach(BattleMode mode in BattleMode.GetValues(typeof(BattleMode)))
            {
                if (mode.ToString().StartsWith("Magic_"))
                {
                    string name = mode.ToString().Substring(6);

                    GameObject newText = GameObject.Instantiate(spellText.gameObject);
                    newText.transform.SetParent(parentTransform);
                    newText.transform.localScale = spellText.transform.localScale;
                    newText.GetComponent<Text>().text = name;
                    
                    newText.GetComponent<SpellClick>().SetMode(mode, this);

                    spellTexts.Add(newText.GetComponent<Text>());

                }
            }

            // Destroy template
            Destroy(spellText.gameObject);

            // Place ALL spellTexts using 6 positions
            for (int i = 0; i < spellTexts.Count; i++)
            {
                spellText.transform.localPosition = positions[i];
            }
        }
    }

    // Called whenever the subMenu becomes active. Enables and sorts spells for display
    public void SetSpells(HeroType type)
    {
        // Set the HeroType
        Initialize(type); 
        
        // Form a list of available spells based on HeroType
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
                // The Magic subMenu is disabled except for the above, so there
                // should never be a default case.
                enableModes.Add(BattleMode.Magic_Cure);
                break;
        }

        // Organize and enable spellTexts into position slots
        int i = 0;
        foreach (Text spellText in spellTexts)
        {
            // collect mode of this spellText
            BattleMode mode = spellText.gameObject.GetComponent<SpellClick>().Mode;
            
            // Compare to list of enabled spells. 
            // If there's a match, enable and position, otherwise disable.
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
    }

    // Used by the SpellClick on a spellText, rather than the inspector
    public void Click_Spell(BattleMode mode)
    {
        this.mode = mode;
        base.CloseSubMenu(); // BattleSubMenu.CloseSubMenu() does the invocation, based on this.mode
    }

}
