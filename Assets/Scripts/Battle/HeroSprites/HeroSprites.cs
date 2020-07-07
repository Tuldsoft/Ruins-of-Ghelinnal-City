using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSprites
{
    protected BattleHero hero = null;
    protected GameObject heroObject = null;
    protected EventTimer spriteTimer;
    protected Sprite heroSprite = null;
    protected SpriteRenderer heroSpriteRenderer = null;

    public Sprite Portrait { get; set; }
    public Sprite Idle { get; set; }
    public Sprite[] Walk { get; set; } = new Sprite[4];
    public Sprite[] Jump { get; set; } = new Sprite[2];
    public Sprite Kneel { get; set; }
    public Sprite Swing { get; set; }
    public Sprite Defend { get; set; }
    public Sprite[] Cast { get; set; } = new Sprite[2];
    public Sprite Item { get; set; }
    public Sprite Struck { get; set; }
    public Sprite Dead { get; set; }
    public Sprite Victory { get; set; }

    // used by BattleHero.LoadObj in BattleManager
    // also used by HeroMovement
    public void SetObj(GameObject heroObj, BattleHero hero = null)
    {
        this.hero = hero;
        heroObject = heroObj;
        heroSprite = heroObj.GetComponent<Sprite>();
        heroSpriteRenderer = heroObj.GetComponent<SpriteRenderer>();

        spriteTimer = heroObj.AddComponent<EventTimer>();
        spriteTimer.AddListener_Finished(ReturnToIdleSprite);
    }

    // used at the end of the sprite timer
    protected virtual void ReturnToIdleSprite()
    {
        if (hero.IsDead) // at the end of Struck
        {
            UpdateSprite(Stance.Dead);
        }
        else if (heroSprite == Cast[0])
        {
            UpdateSprite(Stance.Cast, 0.2f, 1);
        }
        else if (heroSprite == Cast[1])
        {
            UpdateSprite(Stance.Item, 0.1f);
        }
        else if (hero.BattleMode == BattleMode.Defend) // at the end of Struck
        {
            UpdateSprite(Stance.Defend);
        }
        else if (hero.HP < hero.HPMax / 4)
        {
            UpdateSprite(Stance.Kneel);
        }
        else
        {
            UpdateSprite();
        }
        
    }

    // pass null duration if there is no time limit, otherwise float seconds
    public virtual void UpdateSprite(Stance stance = Stance.Idle, float? duration = null, int index = 0)
    {
        // interrupt any timer
        if (spriteTimer.Running) { spriteTimer.Stop(); }

        switch (stance)
        {
            case Stance.Portrait:
                heroSprite = Portrait;
                break;
            case Stance.Idle:
                if (hero.HP < hero.HPMax / 4)
                { heroSprite = Kneel; }
                else { heroSprite = Idle; }
                break;
            case Stance.Walk:
                heroSprite = Walk[index];
                break;
            case Stance.Jump:
                heroSprite = Jump[index];
                break;
            case Stance.Kneel:
                heroSprite = Kneel;
                break;
            case Stance.Swing:
                heroSprite = Swing;
                break;
            case Stance.Defend:
                heroSprite = Defend;
                break;
            case Stance.Cast:
                heroSprite = Cast[index];
                break;
            case Stance.Item:
                heroSprite = Item;
                break;
            case Stance.Struck:
                heroSprite = Struck;
                break;
            case Stance.Dead:
                heroSprite = Dead;
                break;
            case Stance.Victory:
                heroSprite = Victory;
                break;
        }
        heroSpriteRenderer.sprite = heroSprite;

        if (duration != null)
        {
            spriteTimer.Duration = (float)duration;
            spriteTimer.Run();
        }
    }
}
