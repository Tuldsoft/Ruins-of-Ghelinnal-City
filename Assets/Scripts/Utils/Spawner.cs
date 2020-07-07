using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    protected float spawnInterval = 3f;
    [SerializeField]
    protected float intervalVariance = 0.75f;

    [SerializeField]
    bool randomOffScreenRight = false, randomOffScreenLeft = false, randomOffScreenTop = false, randomOffScreenBottom = false;


    protected EventTimer spawnTimer;

    [SerializeField]
    protected GameObject prefabSpawn;

    float maxHalfHeight;
    float maxHalfWidth;
    PolygonCollider2D pc2d = null;
    CircleCollider2D cc2d = null;

    float maxX;
    float maxY;

    private void Start()
    {
        spawnTimer = gameObject.AddComponent<EventTimer>();
        spawnTimer.AddListener_Finished(Spawn);
        
        // find halfheight and halfwidth if the object has a circle or polygon collider
        GameObject testSpawn = GameObject.Instantiate(prefabSpawn);
        testSpawn.TryGetComponent<PolygonCollider2D>(out pc2d);
        testSpawn.TryGetComponent<CircleCollider2D>(out cc2d);

        if (pc2d != null)
        {
            foreach (Vector2 point in pc2d.points)
            {
                float pointX = Mathf.Abs(point.x) * transform.localScale.x;
                maxHalfWidth = pointX > maxHalfWidth ? pointX : maxHalfWidth;
                float pointY = Mathf.Abs(point.y) * transform.localScale.y;
                maxHalfHeight = pointY > maxHalfHeight ? pointY : maxHalfHeight;
            }
        }
        else if (cc2d != null)
        {
            maxHalfHeight = cc2d.radius;
            maxHalfWidth = maxHalfHeight;
        }
        Destroy(testSpawn);

        if (randomOffScreenBottom || randomOffScreenLeft || randomOffScreenTop || randomOffScreenRight)
        {
            float screenZ = -Camera.main.transform.position.z;
            Vector3 upperRightCornerScreen = new Vector3(
                Screen.width, Screen.height, screenZ);
            Vector3 urCorner =
                Camera.main.ScreenToWorldPoint(upperRightCornerScreen);
            
            maxX = urCorner.x - maxHalfWidth - Camera.main.transform.position.x;
            maxY = urCorner.y - maxHalfHeight - Camera.main.transform.position.y;
                       
        }


        spawnTimer.Duration = spawnInterval + Random.Range(-intervalVariance, intervalVariance); 
        spawnTimer.Run();

    }



    protected void Spawn()
    {
        GameObject newSpawn = GameObject.Instantiate(prefabSpawn);
        newSpawn.transform.position = Position();
        spawnTimer.Duration = spawnInterval + Random.Range(-intervalVariance, intervalVariance); 
        spawnTimer.Run();
    }

    Vector2 Position()
    {
        Vector2 position = gameObject.transform.position;
        if (randomOffScreenBottom || randomOffScreenLeft || randomOffScreenTop || randomOffScreenRight)
        {
            float randX = Random.Range(-maxX, maxX);
            float randY = Random.Range(-maxY, maxY);

            List<Vector2> options = new List<Vector2>();

            if (randomOffScreenTop)
            {
                Vector2 option = new Vector2();
                option.x = randX;
                option.y = maxY;
                options.Add(option);
            }
            if (randomOffScreenLeft)
            {
                Vector2 option = new Vector2();
                option.x = -maxX;
                option.y = randY;
                options.Add(option);
            }
            if (randomOffScreenBottom)
            {
                Vector2 option = new Vector2();
                option.x = randX;
                option.y = -maxY;
                options.Add(option);
            }
            if (randomOffScreenRight)
            {
                Vector2 option = new Vector2();
                option.x = maxX;
                option.y = randY;
                options.Add(option);
            }
            options = Randomize<Vector2>.List(options);
            position = options[0];
            position.x += Camera.main.transform.position.x;
            position.y += Camera.main.transform.position.y;

        }

        return position;

    }
}
