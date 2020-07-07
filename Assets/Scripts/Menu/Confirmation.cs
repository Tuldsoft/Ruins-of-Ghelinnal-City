using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Confirmation : MonoBehaviour
{
    [SerializeField]
    Text okButton = null, cancelButton = null;

    ConfirmationEvent confirmationEvent = new ConfirmationEvent();

    private void Start()
    {
        Time.timeScale = 0;
        okButton.text = "OK";
        cancelButton.text = "Cancel";
        EventManager.AddInvoker_Confirmation(this);
    }

    public void Click_OK ()
    {
        AudioManager.Close();
        confirmationEvent.Invoke(true);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void Click_Cancel()
    {
        AudioManager.Close();
        confirmationEvent.Invoke(false);
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void AddListener_Confirmation(UnityAction<bool> listener)
    {
        confirmationEvent.AddListener(listener);
    }
}
