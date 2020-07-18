using UnityEngine;
using System.Collections;

/// <summary>
/// This is a multi-purpose follow script. The object it is attached to will follow a target
/// gameObject. It does not move in tandem, but moves towards a target, going faster the further
/// it is from the target.
/// 
/// Two main uses: A dungeon camera has this script attached to follow the hero. It can be clamped
/// to prevent movement, Ex: A hero can move vertically, but the camera only moves horizontally,
/// stays within the horizontal edges of the scene.
/// 
/// A "follow" FX enemy will follow the hero using this script, hurrying to close the distance
/// between itself and the hero, and slowing when closer. These are often explodable, spawned enemies.
/// 
/// Protected allows FollowTrain to use a modified version of this script
/// </summary>
public class Follow : MonoBehaviour
{
    // The object to follow (often the hero)
    [SerializeField]
    protected GameObject objectToFollow;

    // Whether or not to search for the hero when loaded into the scene
    // Used when the hero cannot be set in the inspector, ex. during runtime at reload.
    [SerializeField]
    bool findHeroAtStart = false;

    // Whether to enable horizontal and/or vertical movement
    [SerializeField]
    bool followX = true, followY = false;

    // Clamps enable outer boundaries for the follower (usually the camera)
    // Ex: clampNegativeY forces the camera to only move downward from a defined Y (Y <= zeroY)
    [SerializeField]
    protected bool clampPositiveX = false, clampPositiveY = false, clampNegativeY = false;
    protected float zeroX = 0f, zeroY = 0f;

    // Default speed of the follower
    [SerializeField]
    protected float speed = 2.0f;
    
    // How far from the target the follower can get before it starts trying to catch up.
    // Ex: A hero moving in the middle of the screen does not cause the camera to move.
    // A hero must move towards the edge to get the camera to follow.
    [SerializeField]
    protected float followDistanceX = 1.75f;
    [SerializeField]
    protected float followDistanceY = 0.75f;

    // Start() is run once before the first frame update
    private void Start()
    {
        // find the hero if findHeroAtStart
        if (findHeroAtStart)
        {
            objectToFollow = GameObject.FindGameObjectWithTag("Hero_Dungeon");
        }
    }

    // FixedUpdate is run once per defined time interval
    protected virtual void FixedUpdate()
    {
        // The following math adjusts the speed and direction of the follower,
        // based on its distance to the target and using clamps and Mathf.Lerp()
        
        float interpolation = speed * Time.deltaTime;

        Vector3 position = this.transform.position;
        Vector3 otherPos = objectToFollow.transform.position;
        
        if (followX && Mathf.Abs(position.x - otherPos.x) > followDistanceX)
        {
            position.x = Mathf.Lerp(position.x, otherPos.x, interpolation);
            if (clampPositiveX) { position.x = Mathf.Max(position.x, zeroX); }
        }
        if (!followX) { position.x = 0f; }
        

        if (followY && Mathf.Abs(position.y - otherPos.y) > followDistanceY)
        {
            position.y = Mathf.Lerp(position.y, otherPos.y, interpolation);
            if (clampPositiveY) { position.y = Mathf.Max(position.y, zeroY); }
            if (clampNegativeY) { position.y = Mathf.Min(position.y, zeroY); }
        }
        if (!followY) { position.y = 0f; }

        this.transform.position = position;
    }
}