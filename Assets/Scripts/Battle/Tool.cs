using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component attached to Edgar's Tool Sprite, on his EdgarHero prefab.
/// Changes its spride based on the action being performed.
/// Also handles the firing of the AutoCrossbow.
/// Does not handle what happens when a bolt or drill makes contact with its target,
/// that is handled by BattleManager.PropelHitTarget().
/// </summary>

public class Tool : MonoBehaviour
{
    #region Fields and Properties
    // The SpriteRenderer of the tool
    SpriteRenderer spriteRenderer;
    
    // GameObject references
    GameObject prefabBolt;
    Sprite[] crossbowSprite = new Sprite[2];
    Sprite drillSprite;

    // Timer for firing the AutoCrossbow
    EventTimer crossbowFireTimer;
    const float CrossbowFireTimerDuration = 0.12f;
    
    // Toggles on and off to swap out AutoCrossbow sprites
    bool loaded = true;
    
    // Determines how many more timems to fire
    int boltsRemaining = 0;
    public int BoltsRemaining { get { return boltsRemaining; } }

    // battleID of the hero, set in Aim()
    int heroBattleID = -1;
    
    // Stack of AutoCrossbow victims
    Stack<GameObject> targetObjs;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Load resources
        prefabBolt = Resources.Load<GameObject>(@"BattlePrefabs\EdgarBolt");
        crossbowSprite[0] = Resources.Load<Sprite>(@"Sprites\Hero\EdgarAutoCrossbow0");
        crossbowSprite[1] = Resources.Load<Sprite>(@"Sprites\Hero\EdgarAutoCrossbow1");
        drillSprite = Resources.Load<Sprite>(@"Sprites\Hero\EdgarDrill");

        // Set up firing timer
        crossbowFireTimer = gameObject.AddComponent<EventTimer>();
        crossbowFireTimer.Duration = CrossbowFireTimerDuration;
        crossbowFireTimer.AddListener_Finished(Load_Fire);

        // Store reference to the SpriteRenderer, and hide the Tool GameObject
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by BattleManager. Loads the AutoCrossbow with bolts and sets the targets
    /// for use with PropelEdgarBolt. Tool has no way to know what targets are possible,
    /// so its targets must be provided.
    /// </summary>
    /// <param name="heroBattleID">battleID of the BattleHero (Edgar)</param>
    /// <param name="targets">A Stack of GameObjects to propel bolts at (enemy sprites)</param>
    public void Aim(int heroBattleID, Stack<GameObject> targets)
    {
        this.heroBattleID = heroBattleID;
        targetObjs = targets;
        boltsRemaining = 4;
    }

    // Makes the tool visible and sets its sprite according to BattleMode
    public void UseTool (BattleMode mode)
    {
        // Display tool on Edgar
        gameObject.SetActive(true);

        // Determine behavior according to BattleMode (which tool?)
        switch (mode)
        {
            case BattleMode.Tools_AutoCrossbow:
                spriteRenderer.sprite = crossbowSprite[0]; // Shows a loaded crossbow
                loaded = true;
                crossbowFireTimer.Run();   // Begins timer (timer plays crossbow sound)
                break;

            case BattleMode.Tools_Drill:
                spriteRenderer.sprite = drillSprite;
                AudioManager.PlaySound(AudioClipName.Drill);

                break;
        }
    }

    // Either Reloads the crossbow or fires a bolt, based on loaded. Called by the timer only.
    void Load_Fire()
    {
        if (loaded && boltsRemaining > 0)
        {
            // fire
            AudioManager.PlaySound(AudioClipName.Crossbow_Bolt);
            
            // swap to empty crossbow
            spriteRenderer.sprite = crossbowSprite[1]; 

            // Store reference to target GameObject (enemy sprite)
            GameObject target = targetObjs.Peek();
            
            // Make and position a bolt. Bolt prefabs are already offset to appear 
            // to come from the crossbow.
            GameObject bolt = Instantiate<GameObject>(prefabBolt);
            bolt.transform.position += gameObject.transform.position;
            
            // send the bolt at its target. 
            // Note: The bolt has a PropelEdgarBolt component, not PropelObject. Polymorphism.
            bolt.GetComponent<PropelObject>().Propel(
                heroBattleID, bolt.transform.position,
                target, 0, BattleMode.Tools_AutoCrossbow); // might not need targetBattleID because it refreshes on collision?
            
            // deduct a bolt from the quiver
            boltsRemaining--;
            targetObjs.Pop();
            
            // restart timer
            crossbowFireTimer.Run();
        }
        else if (boltsRemaining > 0)
        {
            // show loaded sprite
            spriteRenderer.sprite = crossbowSprite[0];
            
            // run timer to empty it
            crossbowFireTimer.Run();
        }
        
        // else: there are no more bolts so don't restart the timer.

        loaded = !loaded;


    }
}
