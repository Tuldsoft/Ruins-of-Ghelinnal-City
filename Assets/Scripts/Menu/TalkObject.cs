using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to a Talk gameObject with a button and text object referenced. It is given a
/// long string of text to display, and will toggle its text on or off when its button is clicked.
/// Manipulated by TalkMonitor and through clicks.
/// </summary>
public class TalkObject : MonoBehaviour
{
    // Reference to the text object and the button that displays it.
    [SerializeField]
    Text talkText = null;
    [SerializeField]
    Button talkButton = null;

    // A unique identifying number owned by this instance of TalkObject.
    // Equivalent to the index in TalkData
    int talkNum;
    
    // A reference to its controller, so that this can use its public Toggle() method
    TalkMonitor talkMonitor;

    // Used once by TalkMonitor to determine the template's original text position
    public Vector2 OrigTextPosition { get { return talkText.transform.position; } }

    // Load the text, its position, and its numeric identifier
    public void SetText(int talkNum, TalkMonitor talkMonitor, Vector2 textPos)
    {
        // Set IDs and references
        this.talkNum = talkNum;
        this.talkMonitor = talkMonitor;

        // Set block of text
        talkButton.GetComponent<Text>().text = TalkData.Titles[talkNum];
        talkText.text = TalkData.Texts[talkNum];

        // Position text (calculated by TalkMonitor
        talkText.transform.position = textPos;
    }

    // Shows or hides the text object, but not its button
    public void SetEnable(bool enabled)
    {
        talkText.gameObject.SetActive(enabled);
    }

    // Used by the TalkButton On_Click(). Tells the TalkMonitor the ID of what was clicked.
    public void Click_Talk()
    {
        AudioManager.Chirp();
        talkMonitor.Toggle(talkNum);
    }
}
