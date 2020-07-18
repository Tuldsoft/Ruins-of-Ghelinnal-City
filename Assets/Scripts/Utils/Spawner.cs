using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An inspector-configurable component. When attached to a gameObject, will
/// make that game object a spawner of other, prefab gameObjects. 
/// Objects are either spawned at the location of the gameObject, or just off camera.
/// Uses an EventTimer as its core mechanic.
/// Used primarily in DungeonLevel scenes to spawn chasing enemies.
/// Similar to the CoinSpawner used in Battle scenes.
/// </summary>

public class Spawner : MonoBehaviour
{
    #region Fields
    // Timer settings
    [SerializeField]
    protected float spawnInterval = 3f;
    [SerializeField]
    protected float intervalVariance = 0.75f;

    // When any of these is set to true, it enables location spawning along that edge of the camera.
    [SerializeField]
    bool randomOffScreenRight = false, randomOffScreenLeft = false, randomOffScreenTop = false, randomOffScreenBottom = false;

    // Timer component for spawning
    protected EventTimer spawnTimer;

    // Prefab GameObject to be spawned
    [SerializeField]
    protected GameObject prefabSpawn;

    // Values used to position the spawn off camera
    float maxHalfHeight;
    float maxHalfWidth;
    
    // Collider of the spawn, if there is one
    PolygonCollider2D pc2d = null;
    CircleCollider2D cc2d = null;

    // More values used to position the spawn off camera
    float maxX;
    float maxY;
    #endregion

    #region Methods
    // Start() is called before the first frame update
    private void Start()
    {
        // Add a timer, set it to Spawn()
        spawnTimer = gameObject.AddComponent<EventTimer>();
        spawnTimer.AddListener_Finished(Spawn);
        
        // Find halfheight and halfwidth if the object has a circle or polygon collider
        GameObject testSpawn = GameObject.Instantiate(prefabSpawn);
        testSpawn.TryGetComponent<PolygonCollider2D>(out pc2d);
        testSpawn.TryGetComponent<CircleCollider2D>(out cc2d);

        if (pc2d != null)
        {
            // For a polygon collider, find the furthest x and y among its points
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
            // For a circle collider, just use radius for both height and width
            maxHalfHeight = cc2d.radius;
            maxHalfWidth = maxHalfHeight;
        }
        Destroy(testSpawn);

        // Get current camera screen width and store a maxX and maxY from its corners
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

        // Set timer duration and run
        spawnTimer.Duration = spawnInterval + Random.Range(-intervalVariance, intervalVariance); 
        spawnTimer.Run();
    }

    // Create a spawn and set its location
    protected void Spawn()
    {
        // Make a copy of the spawn using its prefab
        GameObject newSpawn = GameObject.Instantiate(prefabSpawn);
        
        // Position the spawn
        newSpawn.transform.position = Position();
        
        // Start the timer until the next Spawn()
        spawnTimer.Duration = spawnInterval + Random.Range(-intervalVariance, intervalVariance); 
        spawnTimer.Run();
    }

    // Position a new spawn
    Vector2 Position()
    {
        Vector2 position = gameObject.transform.position;

        // Randomize X and/or Y based on randomOffScreen toggles
        if (randomOffScreenBottom || randomOffScreenLeft || randomOffScreenTop || randomOffScreenRight)
        {
            float randX = Random.Range(-maxX, maxX);
            float randY = Random.Range(-maxY, maxY);

            // Create a list of position options
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

            // Shuffle the options list with the Randomizer
            options = Randomize<Vector2>.List(options);

            // Position at option[0] (this could have been done with options[Random.Range(0,4)]
            position = options[0];

            // Add this as an offset to the current camera position.
            position.x += Camera.main.transform.position.x;
            position.y += Camera.main.transform.position.y;

        }

        return position;
    }
    #endregion
}
