using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DamageDisplay : MonoBehaviour
{
    protected int objID = 0;
    protected bool isDisplaying = false;
    
    protected EventTimer travelTimer;
    protected const float travelTime = 1.5f;
    protected const float travelSpeed = 15f;
    protected const float maxTravelY = 6f;

    protected EventTimer multiHitTimer;
    protected const float multiHitTime = 0.4f;

    protected Damage secondaryDamage;
    protected int secondaryBattleID;

    protected Battle_ApplyDamageEvent applyDamage = new Battle_ApplyDamageEvent();
    //protected Battle_RefreshHPMPSlidersEvent refreshHPMPSliders = new Battle_RefreshHPMPSlidersEvent();
    //protected Battle_DisplayDamageEndEvent endDisplay = new Battle_DisplayDamageEndEvent();

    protected GameObject hpmpsliders;
    protected Slider hpSlider;
    // public Slider HPSlider { get { return hpSlider; } }

    protected Slider mpSlider;
    //public Slider MPSlider { get { return mpSlider; } }

    Text damageAmountText;
    public Text DamageAmountText { get { return damageAmountText; } }


    protected virtual void Start()
    {
        travelTimer = gameObject.AddComponent<EventTimer>();
        travelTimer.Duration = travelTime;
        travelTimer.AddListener_Finished(EndDisplayDamage);

        multiHitTimer = gameObject.AddComponent<EventTimer>();
        multiHitTimer.Duration = multiHitTime;
        multiHitTimer.AddListener_Finished(DisplaySecondaryDamage);
        
        EventManager.AddListener_Battle_DisplayDamage(DisplayDamage);
        EventManager.AddListener_Battle_RefreshHPMPSliders(RefreshHPMPSliders);
        EventManager.AddInvoker_Battle_ApplyDamage(this);
        //EventManager.AddInvoker_Battle_DisplayDamageEnd(this);

        ///hpmpsliders.SetActive(false);
        //damageAmountText.enabled = false;
    }
    protected void Update()
    {
        if (isDisplaying && damageAmountText.transform.localPosition.y < maxTravelY)
        {
            damageAmountText.transform.localPosition += Vector3.up * travelSpeed * Time.deltaTime;
            //transform.localPosition += Vector3.up * travelSpeed;
        }
    }

    public void SetDamageDisplay(int id, int hp, int hpMax, int mp, int mpMax)
    {
        objID = id;

        Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "DamageAmountText") { damageAmountText = texts[i]; }
        }

        SetSliders(hp, hpMax, mp, mpMax);
    }

    protected virtual void SetSliders(int hp, int hpMax, int mp, int mpMax)
    {
        Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].gameObject.name == "HPSlider") { hpSlider = sliders[i]; }
            if (sliders[i].gameObject.name == "MPSlider") { mpSlider = sliders[i]; }
        }

        hpmpsliders = hpSlider.transform.parent.gameObject;

        hpSlider.maxValue = hpMax;
        mpSlider.maxValue = mpMax;
        hpSlider.value = hp;
        mpSlider.value = mp;
        //hpmpsliders.SetActive(false);
        //damageAmountText.enabled = false;
    }


    public void SetID(int id)
    {
        objID = id;
    }

    // invoked at the end of hero turn, also when enemy does damage to hero
    // Positive is damage in red,
    // Negative is healing in green.
    // 0 is a Miss
    // null means the turn ended through some other effect
    protected virtual void DisplayDamage(int battleID, Damage damage)
    {
        if (damage == null) { Debug.Log("Error, damage is null."); return; }

        if (battleID == objID)
        {
            int count = damage.Count;
            int? dmg = damage.Amount;
            bool crit = damage.IsCrit;

            if (dmg != null)
            {
                if (dmg == 0)
                {
                    // miss
                    damageAmountText.text = "Miss";
                    damageAmountText.color = Color.white;
                }
                else if (dmg > 0)
                {
                    // damage

                    damageAmountText.text = (count > 1 ? "COMBO!\n" : "") 
                        + (crit ? "CRITICAL!\n" : "") + dmg.ToString();
                    damageAmountText.color = Color.red;
                }
                else // if (damage < 0)
                {
                    // healing (negative damage)
                    damageAmountText.text = (count > 1 ? "COMBO!\n" : "")
                        + (crit ? "CRITICAL!\n" : "") + Mathf.Abs((int)dmg).ToString();
                    damageAmountText.color = Color.green;
                }

                RefreshHPMPSliders(battleID, (int)dmg, 0);

                if (!damage.IsMP)
                {
                    applyDamage.Invoke(battleID, dmg, null);
                }
                else
                {
                    applyDamage.Invoke(battleID, null, dmg);
                }

                damageAmountText.enabled = true;
                damageAmountText.transform.localPosition = Vector2.zero;
                isDisplaying = true;

                damage.Remove();

                // if there is another hit remaining, store "damage" and prepare for a secondary hit
                if (damage.Count > 0)
                {
                    secondaryBattleID = battleID;
                    secondaryDamage = damage;
                    multiHitTimer.Run();
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

    protected virtual void RefreshHPMPSliders (int battleID, int hpReduction, int mpReduction)
    {
        if (battleID == objID)
        {
            hpSlider.value -= hpReduction;
            mpSlider.value -= mpReduction;

            // display enemy sliders when enemy is damaged
            if (hpSlider.value < hpSlider.maxValue && hpmpsliders.activeSelf == false)
            {
                hpmpsliders.SetActive(true);
            }

            // hide (only) enemy sliders if at max hp.
            if (hpSlider.value >= hpSlider.maxValue
                && hpmpsliders.activeSelf == true
                && battleID >= 0)
            {
                hpmpsliders.SetActive(false);
            }
        }
        
    }

    protected void DisplaySecondaryDamage()
    {
        DisplayDamage(secondaryBattleID, secondaryDamage);
    }

    protected void EndDisplayDamage()
    {
        damageAmountText.enabled = false;
        isDisplaying = false;
        //endDisplay.Invoke(objID);
    }

    public void AddListener_Battle_ApplyDamage(UnityAction<int, int?, int?> listener)
    {
        applyDamage.AddListener(listener);
    }

    /*public void AddListener_Battle_RefreshHPMPSliders(UnityAction<int, int, int> listener)
    {
        refreshHPMPSliders.AddListener(listener);
    }*/

    /*public void AddListener_Battle_DisplayDamageEnd(UnityAction<int> listener)
    {
        endDisplay.AddListener(listener);
    }*/
}
