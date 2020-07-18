using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A static database for populating HeroSprite classes. Also used as a reference for copying sprites.
/// </summary>
public static class HeroSpriteData
{

    #region Fields and Properties
    static bool initialized = false;

    // Container for all sprite collections
    static Dictionary<HeroType, HeroSprites> data = new Dictionary<HeroType, HeroSprites>();

    public static Dictionary<HeroType, HeroSprites> Data { get { return data; } }
    #endregion

    #region Methods
    // Initialize the database upon loading
    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            BuildSpriteDatabase();        
        }
    }

    // Build a database for each hero, then add the database to the 'data' container
    static void BuildSpriteDatabase()
    {

        // sabin generics
        SabinSprites sabinSprites = new SabinSprites();
        BuildGeneric(HeroType.Sabin, sabinSprites);

        // sabin uniques
        sabinSprites.Kick = Resources.Load<Sprite>(@"Sprites\Hero\" + HeroType.Sabin.ToString() + Stance.Kick.ToString());
        sabinSprites.AuraBolt = sabinSprites.Cast[0];
        sabinSprites.FireDance = Resources.Load<Sprite>(@"Sprites\Hero\" + HeroType.Sabin.ToString() 
            + Stance.FireDance.ToString());

        // add sabin database
        data.Add(HeroType.Sabin, sabinSprites);


        // celes generics
        CelesSprites celesSprites = new CelesSprites();
        BuildGeneric(HeroType.Celes, celesSprites);

        // celes unique
        string prefix = @"Sprites\Hero\" + HeroType.Celes.ToString() + Stance.RunicBlade.ToString();
        for (int i = 0; i <= 6; i++)
        {
            celesSprites.RunicBlade[i] = Resources.Load<Sprite>(prefix + i);
        }

        // add celes database
        data.Add(HeroType.Celes, celesSprites);


        // edgar generics
        EdgarSprites edgarSprites = new EdgarSprites();
        BuildGeneric(HeroType.Edgar, edgarSprites);

        // add edgar database
        data.Add(HeroType.Edgar, edgarSprites);


        // locke generics
        LockeSprites lockeSprites = new LockeSprites();
        BuildGeneric(HeroType.Locke, lockeSprites);

        // add locke database
        data.Add(HeroType.Locke, lockeSprites);

    }

    // Load common sprites for any hero
    static void BuildGeneric (HeroType type, HeroSprites sprites)
    {
        // Idle, Walk, Jump, Kneel, Swing, Defend, Cast, Item, Struck, Dead, Victory,
        string prefix = @"Sprites\Hero\" + type.ToString();

        sprites.Portrait = Resources.Load<Sprite>(prefix + Stance.Portrait.ToString()); 
        sprites.Idle = Resources.Load<Sprite>(prefix + Stance.Idle.ToString());
        sprites.Walk[0] = sprites.Idle;
        sprites.Walk[1] = Resources.Load<Sprite>(prefix + Stance.Walk.ToString() + "0");
        sprites.Walk[2] = sprites.Idle;
        sprites.Walk[3] = Resources.Load<Sprite>(prefix + Stance.Walk.ToString() + "1");
        sprites.Jump[0] = Resources.Load<Sprite>(prefix + Stance.Jump.ToString() + "0");
        sprites.Jump[1] = Resources.Load<Sprite>(prefix + Stance.Jump.ToString() + "1");
        sprites.Kneel = Resources.Load<Sprite>(prefix + Stance.Kneel.ToString());
        sprites.Swing = Resources.Load<Sprite>(prefix + Stance.Swing.ToString());
        sprites.Defend = Resources.Load<Sprite>(prefix + Stance.Defend.ToString());
        sprites.Cast[0] = Resources.Load<Sprite>(prefix + Stance.Cast.ToString() + "0");
        sprites.Cast[1] = Resources.Load<Sprite>(prefix + Stance.Cast.ToString() + "1");
        sprites.Item = Resources.Load<Sprite>(prefix + Stance.Item.ToString());
        sprites.Struck = Resources.Load<Sprite>(prefix + Stance.Struck.ToString());
        sprites.Dead = Resources.Load<Sprite>(prefix + Stance.Dead.ToString());
        sprites.Victory = Resources.Load<Sprite>(prefix + Stance.Victory.ToString());
    }
    
    // Copy sprites from the database into a HeroSprite instance
    // Called during creation of a BattleHero
    public static HeroSprites MakeNewHeroSprites(HeroType type)
    {
        switch (type)
        {
            case HeroType.Sabin:
                SabinSprites sabinSprites = new SabinSprites();
                sabinSprites = (SabinSprites)Copy(HeroType.Sabin, sabinSprites);
                return sabinSprites;

            case HeroType.Locke:
                LockeSprites lockeSprites = new LockeSprites();
                lockeSprites = (LockeSprites)Copy(HeroType.Locke, lockeSprites);
                return lockeSprites;

            case HeroType.Edgar:
                EdgarSprites edgarSprites = new EdgarSprites();
                edgarSprites = (EdgarSprites)Copy(HeroType.Edgar, edgarSprites);
                return edgarSprites;

            case HeroType.Celes:
                CelesSprites celesSprites = new CelesSprites();
                celesSprites = (CelesSprites)Copy(HeroType.Celes, celesSprites);
                return celesSprites;

            default:
                sabinSprites = new SabinSprites();
                sabinSprites = (SabinSprites)Copy(HeroType.Sabin, sabinSprites);
                return sabinSprites;
        }

    }

    // used above in MakeNewHeroSprites during creation of a new BattleHero
    static HeroSprites Copy (HeroType type, HeroSprites sprites)
    {
        sprites.Portrait = data[type].Portrait;
        sprites.Idle = data[type].Idle;
        sprites.Walk[0] = data[type].Walk[0];
        sprites.Walk[1] = data[type].Walk[1];
        sprites.Walk[2] = data[type].Walk[2];
        sprites.Walk[3] = data[type].Walk[3];
        sprites.Jump[0] = data[type].Jump[0];
        sprites.Jump[1] = data[type].Jump[1];
        sprites.Kneel = data[type].Kneel;
        sprites.Swing = data[type].Swing;
        sprites.Defend = data[type].Defend;
        sprites.Cast[0] = data[type].Cast[0];
        sprites.Cast[1] = data[type].Cast[1];
        sprites.Item = data[type].Item;
        sprites.Struck = data[type].Struck;
        sprites.Dead = data[type].Dead;
        sprites.Victory = data[type].Victory;
        
        if (sprites is SabinSprites)
        {
            (sprites as SabinSprites).Kick = (data[type] as SabinSprites).Kick;
            (sprites as SabinSprites).AuraBolt = (data[type] as SabinSprites).AuraBolt;
            (sprites as SabinSprites).FireDance = (data[type] as SabinSprites).FireDance;
        }
        
        if (sprites is CelesSprites)
        {
            for (int i = 0; i <= 6; i++)
            {
                (sprites as CelesSprites).RunicBlade[i] = (data[type] as CelesSprites).RunicBlade[i];
            }
        }
        
        return sprites;
    }

    #endregion
}
