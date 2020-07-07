using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutMenuMonitor : MonoBehaviour
{
           
    public void Click_CloseButton()
    {
        AudioManager.Chirp();
        Destroy(gameObject);
    }
}
