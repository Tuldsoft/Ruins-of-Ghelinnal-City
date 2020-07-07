using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    
    GameObject prefabBolt;
    Sprite[] crossbowSprite = new Sprite[2];
    Sprite drillSprite;

    EventTimer crossbowFireTimer;
    const float CrossbowFireTimerDuration = 0.12f;
    bool loaded = true;
    int boltsRemaining = 0;
    public int BoltsRemaining { get { return boltsRemaining; } }

    int heroBattleID = -1;
    Stack<GameObject> targetObjs;


    // Start is called before the first frame update
    void Start()
    {
        prefabBolt = Resources.Load<GameObject>(@"BattlePrefabs\EdgarBolt");
        crossbowSprite[0] = Resources.Load<Sprite>(@"Sprites\Hero\EdgarAutoCrossbow0");
        crossbowSprite[1] = Resources.Load<Sprite>(@"Sprites\Hero\EdgarAutoCrossbow1");
        drillSprite = Resources.Load<Sprite>(@"Sprites\Hero\EdgarDrill");

        crossbowFireTimer = gameObject.AddComponent<EventTimer>();
        crossbowFireTimer.Duration = CrossbowFireTimerDuration;
        crossbowFireTimer.AddListener_Finished(Load_Fire);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void Aim(int heroBattleID, Stack<GameObject> targets)
    {
        this.heroBattleID = heroBattleID;
        targetObjs = targets;
        boltsRemaining = 4;
    }

    public void UseTool (BattleMode mode)
    {
        gameObject.SetActive(true);

        switch (mode)
        {
            case BattleMode.Tools_AutoCrossbow:
                spriteRenderer.sprite = crossbowSprite[0];
                loaded = true;
                crossbowFireTimer.Run();
                break;

            case BattleMode.Tools_Drill:
                spriteRenderer.sprite = drillSprite;
                AudioManager.PlaySound(AudioClipName.Drill);

                break;
        }
    }

    void Load_Fire()
    {
        if (loaded && boltsRemaining > 0)
        {
            // fire
            AudioManager.PlaySound(AudioClipName.Crossbow_Bolt);
            
            spriteRenderer.sprite = crossbowSprite[1];

            GameObject target = targetObjs.Peek();
            
            GameObject bolt = Instantiate<GameObject>(prefabBolt);
            bolt.transform.position += gameObject.transform.position;
            bolt.GetComponent<PropelObject>().Propel(
                heroBattleID, bolt.transform.position,
                target, 0, BattleMode.Tools_AutoCrossbow); // might not need targetBattleID because it refreshes on collision?
            boltsRemaining--;
            targetObjs.Pop();
            
            crossbowFireTimer.Run();
        }
        else if (boltsRemaining > 0)
        {
            // load
            spriteRenderer.sprite = crossbowSprite[0];
            crossbowFireTimer.Run();
        }
        
        loaded = !loaded;


    }
}
