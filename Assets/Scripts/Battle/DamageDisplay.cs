using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Attached as a component to the MonsterCanvas of a prefabEnemy, or to a Hero prefab.
///  Displays the numerical value of damage, cycling through stacks in a Damage object.
///  Changes color based on the type of damage, and includes other messages.
///  Requires a Text gameObject as a children's component, called "DamageAmountText".
/// Also keeps HP and MP sliders up to date.
///  Has a HeroDamageDisplay variant that controls HP/MP Sliders.
/// REFACTOR: Pass Battler into SetSlider instead of independent values
/// REFACTOR: Make a HPMPSlider class to be used in town menus and battle alike.
/// REFACTOR: Incorporate travelTimer function into multiHitTimer
/// </summary>

public class DamageDisplay : MonoBehaviour
{
    #region Fields and Properties
    protected int objID = 0;                 // BattleID of associated BattleHero or BattleEnemy
    protected bool isDisplaying = false;     // Whether currently displaying texts
    
    // Timer for movement and display
    protected EventTimer travelTimer;
    protected const float travelTime = 1.5f; // Duration of display
    protected const float travelSpeed = 15f; // How fast it moves upward
    protected const float maxTravelY = 6f;   // How far up the display travels

    // Timer for displaying multiple hits
    protected EventTimer multiHitTimer;         
    protected const float multiHitTime = 0.4f; // Interval between messages. Overlaps with previous.

    protected Damage secondaryDamage; // Used with multiHitTimer to chain damages
    protected int secondaryBattleID;  // ID passed in timer event handling

    // Invokes ApplyDamage event to tell BattleManager to deduct displayed amount
    protected Battle_ApplyDamageEvent applyDamage = new Battle_ApplyDamageEvent();

    // HPMP Sliders. These are attached on an enemy, and require setting via method on a hero
    protected GameObject hpmpsliders; // Container object of both sliders
    protected Slider hpSlider;        // HPSlider child gameObject of hpmpsliders
    protected Slider mpSlider;        // MPSlider child gameObject of hpmpsliders

    
    Text damageAmountText;           // DamageAmountText gameObject child
    public Text DamageAmountText { get { return damageAmountText; } } // for debugging
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Set up travelTimer
        travelTimer = gameObject.AddComponent<EventTimer>();
        travelTimer.Duration = travelTime;
        travelTimer.AddListener_Finished(EndDisplayDamage);

        // Set up multiHitTimer
        multiHitTimer = gameObject.AddComponent<EventTimer>();
        multiHitTimer.Duration = multiHitTime;
        multiHitTimer.AddListener_Finished(DisplaySecondaryDamage);
        
        // Listener for DisplayDamage events and RefreshHPMPSliders
        EventManager.AddListener_Battle_DisplayDamage(DisplayDamage);
        EventManager.AddListener_Battle_RefreshHPMPSliders(RefreshHPMPSliders);
        // Invoker for ApplyDamage
        EventManager.AddInvoker_Battle_ApplyDamage(this);
    }

    // Update is called with each frame update
    protected void Update()
    {
        // Move the display upward if displaying
        if (isDisplaying && damageAmountText.transform.localPosition.y < maxTravelY)
        {
            damageAmountText.transform.localPosition += Vector3.up * travelSpeed * Time.deltaTime;
        }
    }

    // Stores a reference to the DamageAmountText. 
    // Called by BattleManager during setup, to remain agnostic to where the HP/MP values come from.
    public void SetDamageDisplay(int id, int hp, int hpMax, int mp, int mpMax)
    {
        objID = id;

        // Scan Text children for one named "DamageAmountText"
        Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "DamageAmountText") { damageAmountText = texts[i]; }
        }

        // Call virtual SetSliders()
        SetSliders(hp, hpMax, mp, mpMax);
    }

    // Define values needed by HP and MP Sliders (BattleEnemys). Overwritten in HeroDamageDisplay.
    protected virtual void SetSliders(int hp, int hpMax, int mp, int mpMax)
    {
        // Get both sliders from childrens' components
        Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].gameObject.name == "HPSlider") { hpSlider = sliders[i]; }
            if (sliders[i].gameObject.name == "MPSlider") { mpSlider = sliders[i]; }
        }

        // Store container gameObject of sliders
        hpmpsliders = hpSlider.transform.parent.gameObject;

        // Set values
        hpSlider.maxValue = hpMax;
        mpSlider.maxValue = mpMax;
        hpSlider.value = hp;
        mpSlider.value = mp;
    }

    // Sets ID of DamageDisplay. Called by BattleManager when reassigning BattleIDs.
    public void SetID(int id)
    {
        objID = id;
    }

    /// <summary>
    /// The main method for converting a Damage object into a display. 
    /// Invoked by the DisplayDamage event.
    /// HeroDamageDisplay adds a "HP / MP" display over the sliders.
    /// </summary>
    /// <param name="battleID">BattleID of attached battler</param>
    /// <param name="damage">Damage object, not values</param>
    protected virtual void DisplayDamage(int battleID, Damage damage)
    {
        // Ignore a null damage object 
        // (this should not happen, instead a Damage object containing null should be passed.)
        if (damage == null) 
        { 
            // Debug.Log("Error, damage is null.");
            return; 
        }

        // All DisplayDamage objects are listening, so only display if the ID matches.
        if (battleID == objID)
        {
            int count = damage.Count;
            int? dmg = damage.Amount;
            bool crit = damage.IsCrit;

            if (dmg != null) // null value is ignored
            {
                // Change color based on value
                if (dmg == 0) 
                {
                    // 0 is a miss
                    damageAmountText.text = "Miss";
                    damageAmountText.color = Color.white;
                }
                else if (dmg > 0)
                {
                    // damage if > 0

                    damageAmountText.text = (count > 1 ? "COMBO!\n" : "") 
                        + (crit ? "CRITICAL!\n" : "") + dmg.ToString();
                    damageAmountText.color = Color.red;
                }
                else 
                {
                    // healing (negative damage)
                    damageAmountText.text = (count > 1 ? "COMBO!\n" : "")
                        + (crit ? "CRITICAL!\n" : "") + Mathf.Abs((int)dmg).ToString();
                    damageAmountText.color = Color.green;
                }

                // Change value of Sliders by damage.Amount
                RefreshHPMPSliders(battleID, (int)dmg, 0);

                // Apply damage to relevant BattleHero or BattleEnemy (via BattleManager)
                if (!damage.IsMP)
                {
                    applyDamage.Invoke(battleID, dmg, null);
                }
                else
                {
                    applyDamage.Invoke(battleID, null, dmg);
                }

                // Start overhead display
                damageAmountText.enabled = true;
                damageAmountText.transform.localPosition = Vector2.zero;
                isDisplaying = true;

                // Remove from stack
                damage.Remove();

                // If there is another hit remaining, store "damage" and prepare for a secondary hit
                if (damage.Count > 0)
                {
                    secondaryBattleID = battleID;
                    secondaryDamage = damage;
                    multiHitTimer.Run(); // multiHitTimer calls the next DamageDisplay
                }
                // else, just run a normal traveltimer and end isDisplaying when finished
                else
                {
                    travelTimer.Run();
                }

            }
            else
            // null is passed during a turnover event
            {
                damage.Clear();
            }
        }
    }

    // Refreshes HPMPSliders by deducting from their values. Has a customized Hero version.
    protected virtual void RefreshHPMPSliders (int battleID, int hpReduction, int mpReduction)
    {
        if (battleID == objID)
        {
            hpSlider.value -= hpReduction; 
            mpSlider.value -= mpReduction;

            // Display enemy sliders only if the enemy is damage. Remains hidden if at max.
            if (hpSlider.value < hpSlider.maxValue && hpmpsliders.activeSelf == false)
            {
                hpmpsliders.SetActive(true);
            }

            // Hide (only) enemy sliders if at max hp.
            if (hpSlider.value >= hpSlider.maxValue
                && hpmpsliders.activeSelf == true
                && battleID >= 0)
            {
                hpmpsliders.SetActive(false);
            }
        }
    }

    // Invoked by the multiHitTimer, triggering the next DisplayDamage
    protected void DisplaySecondaryDamage()
    {
        DisplayDamage(secondaryBattleID, secondaryDamage);
    }

    // Invoked by travelTimer, re-hides the display
    protected void EndDisplayDamage()
    {
        damageAmountText.enabled = false;
        isDisplaying = false;
    }

    // Register as an invoker of the ApplyDamage event
    public void AddListener_Battle_ApplyDamage(UnityAction<int, int?, int?> listener)
    {
        applyDamage.AddListener(listener);
    }

}
