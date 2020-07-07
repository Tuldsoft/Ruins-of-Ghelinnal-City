using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabinSprites : HeroSprites
{
    public Sprite Kick { get; set; }
    public Sprite AuraBolt { get; set; }
    public Sprite FireDance { get; set; }



    public override void UpdateSprite(Stance stance = Stance.Idle, float? duration = null, int index = 0)
    {
        switch (stance)
        {
            case Stance.Kick:
                heroSprite = Kick;
                break;
            case Stance.Aurabolt:
                heroSprite = Kick;
                break;
            case Stance.FireDance:
                heroSprite = Kick;
                break;
        }
        base.UpdateSprite(stance, duration, index);
    }


}

