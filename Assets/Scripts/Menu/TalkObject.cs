using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkObject : MonoBehaviour
{
    [SerializeField]
    Text talkText = null;
    [SerializeField]
    Button talkButton = null;

    int talkNum;
    TalkMonitor talkMonitor;

    public Vector2 OrigTextPosition { get { return talkText.transform.position; } }

    public void SetText(int talkNum, TalkMonitor talkMonitor, Vector2 textPos)
    {
        this.talkNum = talkNum;
        this.talkMonitor = talkMonitor;

        talkButton.GetComponent<Text>().text = TalkData.Titles[talkNum];
        talkText.text = TalkData.Texts[talkNum];
        talkText.transform.position = textPos;
        
        /*Vector2 position = talkText.transform.localPosition;
        position -= yOffset * talkNum * talkText.transform.localScale.y;
        talkText.transform.localPosition = position;*/

    }

    public void SetEnable(bool enabled)
    {
        talkText.gameObject.SetActive(enabled);
    }

    public void Click_Talk()
    {
        AudioManager.Chirp();
        talkMonitor.Toggle(talkNum);
    }
}
