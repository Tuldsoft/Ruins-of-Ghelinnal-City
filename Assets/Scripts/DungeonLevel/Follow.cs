using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{

    [SerializeField]
    protected GameObject objectToFollow;

    [SerializeField]
    bool findHeroAtStart = false;

    [SerializeField]
    bool followX = true, followY = false;

    [SerializeField]
    protected bool clampPositiveX = false, clampPositiveY = false, clampNegativeY = false;
    protected float zeroX = 0f, zeroY = 0f;

    [SerializeField]
    protected float speed = 2.0f;
    [SerializeField]
    protected float followDistanceX = 1.75f;
    [SerializeField]
    protected float followDistanceY = 0.75f;

    private void Start()
    {
        if (findHeroAtStart)
        {
            objectToFollow = GameObject.FindGameObjectWithTag("Hero_Dungeon");
        }
    }

    protected virtual void FixedUpdate()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = this.transform.position;
        Vector3 otherPos = objectToFollow.transform.position;
        
        if (followX && Mathf.Abs(position.x - otherPos.x) > followDistanceX)
        {
            position.x = Mathf.Lerp(position.x, otherPos.x, interpolation);
            if (clampPositiveX) { position.x = Mathf.Max(position.x, zeroX); }
        }
        if (!followX) { position.x = 0f; }
        
        //position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

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