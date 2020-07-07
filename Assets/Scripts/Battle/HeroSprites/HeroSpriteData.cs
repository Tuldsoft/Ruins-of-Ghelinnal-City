using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeroSpriteData
{
    static bool initialized = false;

    static Dictionary<HeroType, HeroSprites> data = new Dictionary<HeroType, HeroSprites>();

    public static Dictionary<HeroType, HeroSprites> Data { get { return data; } }


    public static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;

            BuildSpriteDatabase();        

        }

    }

    static void BuildSpriteDatabase()
    {

        // sabin
        SabinSprites sabinSprites = new SabinSprites();
        BuildGeneric(HeroType.Sabin, sabinSprites);

        sabinSprites.Kick = Resources.Load<Sprite>(@"Sprites\Hero\" + HeroType.Sabin.ToString() + Stance.Kick.ToString());
        sabinSprites.AuraBolt = sabinSprites.Cast[0];
        sabinSprites.FireDance = Resources.Load<Sprite>(@"Sprites\Hero\" + HeroType.Sabin.ToString() 
            + Stance.FireDance.ToString());

        data.Add(HeroType.Sabin, sabinSprites);


        // celes
        CelesSprites celesSprites = new CelesSprites();
        BuildGeneric(HeroType.Celes, celesSprites);

        string prefix = @"Sprites\Hero\" + HeroType.Celes.ToString() + Stance.RunicBlade.ToString();
        for (int i = 0; i <= 6; i++)
        {
            celesSprites.RunicBlade[i] = Resources.Load<Sprite>(prefix + i);
        }

        data.Add(HeroType.Celes, celesSprites);


        // edgar
        EdgarSprites edgarSprites = new EdgarSprites();
        BuildGeneric(HeroType.Edgar, edgarSprites);

        data.Add(HeroType.Edgar, edgarSprites);


        // locke
        LockeSprites lockeSprites = new LockeSprites();
        BuildGeneric(HeroType.Locke, lockeSprites);

        data.Add(HeroType.Locke, lockeSprites);

    }

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

}
