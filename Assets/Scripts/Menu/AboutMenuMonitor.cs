using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Attached to prefabAboutMenuMonitor. Only for the close button
// REFACTOR: Use this as a master template for most prefab menus.

public class AboutMenuMonitor : MonoBehaviour
{
           
    public void Click_CloseButton()
    {
        AudioManager.Chirp();
        Destroy(gameObject);
    }
}
