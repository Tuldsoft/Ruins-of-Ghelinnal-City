using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkMonitor : MonoBehaviour
{
    [SerializeField]
    GameObject talkObj = null;

    List<TalkObject> talkObjs = new List<TalkObject>();

    [SerializeField]
    GameObject flareObj = null;

    private void Start()
    {
        Transform talkParent = talkObj.transform.parent;
        Vector2 yOffset = new Vector2(0f, -48f);

        talkObjs.Add(talkObj.GetComponent<TalkObject>());

        Vector2 origTextPosition = talkObjs[0].OrigTextPosition;
        
        talkObjs[0].SetText(0, this, origTextPosition);

        for (int i = 1; i < TalkData.MaxTalks; i++)
        {
            if (TalkData.AccessLevel[i] <= BattleLoader.DungeonLevelAccess 
                || BattleLoader.GameCompleted)
            {
                GameObject newTalkObj = Instantiate<GameObject>(talkObj, talkParent);
                //newTalkObj.transform.SetParent(talkParent);
                Vector2 position = newTalkObj.transform.localPosition;
                position += (yOffset * i);
                newTalkObj.transform.localPosition = position;
                TalkObject newTextObject = newTalkObj.GetComponent<TalkObject>();
                newTextObject.SetText(i, this, origTextPosition);
                talkObjs.Add(newTextObject);
            }
            else
            {
                break;
            }
        }

        if (BattleLoader.LastTalkingPoint < talkObjs.Count - 1) { BattleLoader.LastTalkingPoint++; }

        Toggle(BattleLoader.LastTalkingPoint);
    }

    public void Toggle(int tab)
    {
        for (int i = 0; i < talkObjs.Count; i++)
        {
            if (i == tab) 
            {
                talkObjs[i].SetEnable(true);
                flareObj.transform.position = talkObjs[i].transform.position;
                if (BattleLoader.LastTalkingPoint < i) { BattleLoader.LastTalkingPoint = i; }
            }
            else
            {
                talkObjs[i].SetEnable(false);
            }
        }
    }

    

    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }
}

