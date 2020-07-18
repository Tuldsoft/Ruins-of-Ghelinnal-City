using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class to store a hero's sprites
/// </summary>
public class HeroSprites
{
    #region Fields and Properties

    protected BattleHero hero = null;
    protected GameObject heroObject = null;
    protected EventTimer spriteTimer; // timer for swapping sprites, triggers UpdateSprite
    protected Sprite heroSprite = null;
    protected SpriteRenderer heroSpriteRenderer = null;

    public Sprite Portrait { get; set; }
    public Sprite Idle { get; set; }
    public Sprite[] Walk { get; set; } = new Sprite[4]; // each hero has 4 walking sprites
    public Sprite[] Jump { get; set; } = new Sprite[2]; // each hero has 2 jumping sprites
    public Sprite Kneel { get; set; }
    public Sprite Swing { get; set; }
    public Sprite Defend { get; set; }
    public Sprite[] Cast { get; set; } = new Sprite[2]; // each hero has 2 casting sprites
    public Sprite Item { get; set; }
    public Sprite Struck { get; set; }
    public Sprite Dead { get; set; }
    public Sprite Victory { get; set; }

    // additional sprites defined by child classes of HeroSprites

    #endregion

    #region Methods

    // Initializes this instance of HeroSprites
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

    // Chooses the correct idling sprite
    // Used at the end of the sprite timer
    // Altered by children of HeroSprites
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
            UpdateSprite(); // no argument returns to Idle
        }
        
    }

    /// <summary>
    /// Change the displayed sprite and set the duration of the displayed sprite before
    /// going to the next sprite in the sequence, or returning to idle. UpdateSprite relies on
    /// an EventTimer called spriteTimer. Almost all animations are created in this manner.
    /// </summary>
    /// <param name="stance">The activity the hero is engaged in</param>
    /// <param name="duration">Duration to display the sprite in seconds</param>
    /// <param name="index">Index of the sprite to use, when the sprite is an array</param>
    // pass null duration if there is no time limit, otherwise float seconds
    public virtual void UpdateSprite(Stance stance = Stance.Idle, float? duration = null, int index = 0)
    {
        // interrupt any timer currently running
        if (spriteTimer.Running) { spriteTimer.Stop(); }

        // choose the next sprite according to Stance
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
        
        // set the sprite
        heroSpriteRenderer.sprite = heroSprite;

        // start timer
        if (duration != null)
        {
            spriteTimer.Duration = (float)duration;
            spriteTimer.Run();
        }
    }
    #endregion
}
