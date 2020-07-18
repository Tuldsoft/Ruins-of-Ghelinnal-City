using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specialized version of Follow that replaces the camera's Follow component in Dungeon 6.
/// It forces camera movement to move with the PhantomTrain, moving evenly to the right.
/// SwitchClampToTrain() is called by Enemy_Dungeon_Train, attached to the train.
/// </summary>
public class FollowTrain : Follow
{
    GameObject trainObj;          // The train gameObject to follow
    bool followingTrain = false;  // Whether the object is following the train at present.
    const float xOffset = 5.86f;  // The camera's location is exactly this amount to the right of the train.

    // Called by the Phantom Train's Enemy_Dungeon_Train
    public void SwitchClampToTrain(GameObject trainObj)
    {
        // Blow the horn and change the music. Sppoky!
        AudioManager.PlaySound(AudioClipName.TrainHorn);
        AudioManager.PlayMusic(AudioClipName.Music_The_Phantom_Train);
        
        // Enable movement in FixedUpdate()
        followingTrain = true;

        // Store reference to the train attached to Enemy_Dungeon_Train
        this.trainObj = trainObj;
    }


    // Update is called once per frame. This overrides the FixedUpdate() of Follow.
    protected override void FixedUpdate()
    {
        // followingTrain is set True in Enemy_Dungeon_Train.OnBecameVisible()
        if (followingTrain)
        {
            // Set position
            Vector2 position = gameObject.transform.position;
            position.x = trainObj.transform.position.x + xOffset;

            // Limit camera's Y position to be at or above the train.
            zeroY = trainObj.transform.position.y;
            clampPositiveY = true; 

            // Interpolate into position
            float interpolation = speed * Time.deltaTime;

            Vector3 otherPos = objectToFollow.transform.position;

            if (Mathf.Abs(position.y - otherPos.y) > followDistanceY)
            {
                position.y = Mathf.Lerp(position.y, otherPos.y, interpolation);
                position.y = Mathf.Max(position.y, zeroY);
                //if (clampNegativeY) { position.y = Mathf.Min(position.y, zeroY); }
            }

            gameObject.transform.position = new Vector3(position.x, position.y,
                Camera.main.transform.position.z);
            
        }
        else
        {
            // If not followingTrain, act like a normal Follow script (following hero)
            base.FixedUpdate();
        }
    }



}
