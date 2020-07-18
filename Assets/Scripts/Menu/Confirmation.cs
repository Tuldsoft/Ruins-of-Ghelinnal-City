using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Attached to prefabConfirmation, which displays simply a message, OK, and Cancel.
// Returns its event to whomever might be listening
// REFACTOR: Use paused Coroutines instead of event system.

public class Confirmation : MonoBehaviour
{
    // Store buttons
    [SerializeField]
    Text okButton = null, cancelButton = null;

    // Event containing choice of OK or Cancel
    ConfirmationEvent confirmationEvent = new ConfirmationEvent();

    // Start is called before the first frame update
    private void Start()
    {
        // Pause game while dialog box is open
        Time.timeScale = 0;

        // Set button text. Variants can replace this with Yes/No or other binary choices.
        okButton.text = "OK";
        cancelButton.text = "Cancel";
        EventManager.AddInvoker_Confirmation(this);
    }

    // For the OKButton On_Click() method
    public void Click_OK ()
    {
        AudioManager.Close();
        confirmationEvent.Invoke(true); // Send results of true through event
        Time.timeScale = 1;             // Unpause
        Destroy(gameObject);
    }

    // For the CancelButton On_Click() method
    public void Click_Cancel()
    {
        AudioManager.Close();
        confirmationEvent.Invoke(false); // Send result of false through event
        Time.timeScale = 1;              // Unpause
        Destroy(gameObject);
    }

    // Used to identify this as an invoker to listeners.
    public void AddListener_Confirmation(UnityAction<bool> listener)
    {
        confirmationEvent.AddListener(listener);
    }
}
