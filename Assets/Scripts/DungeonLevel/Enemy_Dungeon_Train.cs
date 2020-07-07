using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dungeon_Train : MonoBehaviour
{

    private void OnBecameVisible()
    {
        Camera.main.GetComponent<FollowTrain>().SwitchClampToTrain(gameObject);
        gameObject.GetComponent<Enemy_Dungeon_FX>().isFlying = true;
    }
}
