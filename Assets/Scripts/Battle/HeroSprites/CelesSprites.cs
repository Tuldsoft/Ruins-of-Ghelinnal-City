using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides sprite manipulation unique to Celes. 
/// </summary>
public class CelesSprites : HeroSprites
{
    // Add RunicBlade sprites
    public Sprite[] RunicBlade { get; set; } = new Sprite[7];

    // Add RunicBlade updates
    public override void UpdateSprite(Stance stance = Stance.Idle, float? duration = null, int index = 0)
    {
        switch (stance)
        {
            case Stance.RunicBlade:
                heroSprite = RunicBlade[index];
                break;
            
        }

        base.UpdateSprite(stance, duration, index);
    }
    
    // Add RunicBlade idling
    protected override void ReturnToIdleSprite()
    {
        if ( hero.BattleMode == BattleMode.Runic)
        {
            for (int i = 0; i < RunicBlade.Length - 1; i++)
            {
                if (heroSprite == RunicBlade[i])
                {
                    UpdateSprite(Stance.RunicBlade, 0.09f, i + 1);
                    return;
                }
            }
            
        }

        base.ReturnToIdleSprite();

    }
}
