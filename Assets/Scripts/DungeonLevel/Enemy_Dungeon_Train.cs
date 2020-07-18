using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached only to the Phantom Train. Commands the camera to follow it instead of the
/// hero when the train comes into view, and then gets the train moving.
/// </summary>
public class Enemy_Dungeon_Train : MonoBehaviour
{
    // Called when the train comes into view of the camera
    private void OnBecameVisible()
    {
        // Instead of the camera following the hero, follow the train
        Camera.main.GetComponent<FollowTrain>().SwitchClampToTrain(gameObject);
        
        // This will move the train.
        gameObject.GetComponent<Enemy_Dungeon_FX>().isFlying = true;
    }
}
