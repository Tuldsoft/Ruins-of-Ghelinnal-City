using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabTalkMenu. Controls which texts of lore are available for reading,
/// and the toggle buttons to display them. Creates "TalkObjects", similar to panels, for
/// each entry in TalkData. A TalkObject contains both a button for displaying text and its text,
/// and its text display is scrollable.
/// </summary>
public class TalkMonitor : MonoBehaviour
{
    #region Fields
    // A template TalkObject gameObject
    [SerializeField]
    GameObject talkObj = null;

    // A list of all talkObjs that have been instantiated
    List<TalkObject> talkObjs = new List<TalkObject>();

    // A flare 'cursor' that shows which talkObj is being displayed
    [SerializeField]
    GameObject flareObj = null;
    #endregion

    #region Methods
    // Start() is called before the first frame update
    private void Start()
    {
        // Set the first talkObj

        // Set the parent, so that location references are calculated properly
        Transform talkParent = talkObj.transform.parent;
        
        // Offset required for distancing moving texts in relation to their buttons
        Vector2 yOffset = new Vector2(0f, -48f);

        // Store this instance of talkObj as the first in talkObjs
        talkObjs.Add(talkObj.GetComponent<TalkObject>());

        // Store the first talkObj's position within the TalkMenu canvas
        Vector2 origTextPosition = talkObjs[0].OrigTextPosition;
        
        // Set the text of the first talkObj
        talkObjs[0].SetText(0, this, origTextPosition);

        // Create and set up addtional talkObjs for each entry in TalkData
        for (int i = 1; i < TalkData.MaxTalks; i++)
        {
            // If the accessLevel of this TalkData entry > DungeonAccessLevel, break.
            if (TalkData.AccessLevel[i] <= BattleLoader.DungeonLevelAccess 
                || BattleLoader.GameCompleted)
            {
                GameObject newTalkObj = Instantiate<GameObject>(talkObj, talkParent); // create talkObj
                Vector2 position = newTalkObj.transform.localPosition;               
                position += (yOffset * i);                                      
                newTalkObj.transform.localPosition = position;                        // move talkObj
                TalkObject newTextObject = newTalkObj.GetComponent<TalkObject>();     // get TalkObject
                newTextObject.SetText(i, this, origTextPosition);                     // set text of talkObj
                talkObjs.Add(newTextObject);                                          // add to talkObjs
            }
            else
            {
                break;
            }
        }

        // BattleLoader.LastTalkingPoint stores what the talkObj was last seen by the user.
        // If there is a new talkObj that has not been read yet, toggle the next, new talkObj
        if (BattleLoader.LastTalkingPoint < talkObjs.Count - 1) { BattleLoader.LastTalkingPoint++; }

        Toggle(BattleLoader.LastTalkingPoint);
    }

    // Used by each individual TalkObject's TalkButton. Shows or Hides the block of text.
    public void Toggle(int tab)
    {
        // Find the intended talkObj to display, others are hidden
        for (int i = 0; i < talkObjs.Count; i++)
        {
            if (i == tab) 
            {
                // Display the text
                talkObjs[i].SetEnable(true);
                // Position the flare over the relevant tab
                flareObj.transform.position = talkObjs[i].transform.position;
                // Set this talk as the current talking point, if it is past the previous talking point
                if (BattleLoader.LastTalkingPoint < i) { BattleLoader.LastTalkingPoint = i; }
            }
            else
            {
                talkObjs[i].SetEnable(false);
            }
        }
    }

    // Used by CloseButton On_Click(). Closes this menu.
    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }
    #endregion
}

