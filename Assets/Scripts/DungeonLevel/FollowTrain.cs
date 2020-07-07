using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrain : Follow
{


    GameObject trainObj;
    bool followingTrain = false;
    const float xOffset = 5.86f;

    //GameObject followPoint;

    public void SwitchClampToTrain(GameObject trainObj)
    {
        AudioManager.PlaySound(AudioClipName.TrainHorn);
        AudioManager.PlayMusic(AudioClipName.Music_The_Phantom_Train);
        
        followingTrain = true;
        this.trainObj = trainObj;

        /*float screenZ = -Camera.main.transform.position.z;
        Vector3 upperRightCornerScreen = new Vector3(
            Screen.width, Screen.height, screenZ);
        *//*Vector3 upperRightCornerScreen = new Vector3(
            Camera.main.pixelWidth, Camera.main.pixelHeight, screenZ);*//*
        Vector3 urCornerAsWorldPoint =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);

        trainObj.TryGetComponent<PolygonCollider2D>(out PolygonCollider2D pc2d);

        float maxHalfWidth = 0;
        if (pc2d != null)
        {
            foreach (Vector2 point in pc2d.points)
            {
                float pointX = point.x * trainObj.transform.localScale.x;
                maxHalfWidth = pointX < maxHalfWidth ? pointX : maxHalfWidth;
                *//*float pointY = Mathf.Abs(point.y) * transform.localScale.y;
                maxHalfHeight = pointY > maxHalfHeight ? pointY : maxHalfHeight;*//*
            }
            maxHalfWidth *= -1;
        }
         
        xOffset = urCornerAsWorldPoint.x - Camera.main.transform.position.x - (maxHalfWidth);*/

        /*followPoint = new GameObject();
        followPoint.transform.position = new Vector2(trainObj.transform.position.x + xOffset, trainObj.transform.position.y);*/

        //objectToFollow = followPoint;
        //followDistanceX = 0f;
        //followDistanceY = 0f;
    }


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if (followingTrain)
        {
            Vector2 position = gameObject.transform.position;
            position.x = trainObj.transform.position.x + xOffset;


            /*followPoint.transform.position = new Vector2(trainObj.transform.position.x + xOffset, trainObj.transform.position.y);
            zeroX = followPoint.transform.position.x;
            base.FixedUpdate();*/
            zeroY = trainObj.transform.position.y;
            clampPositiveY = true;

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
            base.FixedUpdate();
        }
    }



}
