using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to an explosion animated object, used both in dungeons and battles.
/// </summary>
public class Explosion : MonoBehaviour
{	
	// Reference to the object's animator
	Animator anim;

    // Start is run before the first frame update
    void Start()
    {
		// Store reference to the Animator
        anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
		// Destroy the game object if the explosion has finished its animation
		if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
			Destroy(gameObject);
		}
	}
}
