using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This makes a GameObject "pulse" when the mouse hovers over it.
// Currently used on the StartButton GameObject
public class PulseResizer : MonoBehaviour
{

    // Support for mouseover resizing
    //[SerializeField]
    float resizeCycleDuration = 0.35f;
    float elapsedResizeSeconds = 0;

    //[SerializeField]
    float scaleFactorPerSecond = 1.8f;
    int scaleFactorSignMultiplier = 1;

    Vector3 objectOriginalScale;

    // Start is called before the first frame update
    void Start()
    {
        // This Vector3 variable needs to be defined, for later use in OnMouseExit()
        objectOriginalScale = transform.localScale;

    }

    // To let you know an object something, I have it pulse
    // when you move you mouse over it.
    void OnMouseOver()
    {
            
        // This is based on the teddybear resizing exercise
        float addSize = scaleFactorSignMultiplier * scaleFactorPerSecond * Time.deltaTime;

        // Resizes the object.
        transform.localScale = new Vector3(transform.localScale.x + addSize,
            transform.localScale.y + addSize, 1);

        // Adds to time
        elapsedResizeSeconds += Time.deltaTime;

        // Reverses the growth / shrinkage, cyclically
        if (elapsedResizeSeconds > resizeCycleDuration 
            || transform.localScale.x < objectOriginalScale.x )
        {
            scaleFactorSignMultiplier *= -1;
            elapsedResizeSeconds = 0;
        }
        
    }

    /// <summary>
    /// If the mouse is not over the button, stop pulsing and revert to normal size.
    /// </summary>
    private void OnMouseExit()
    {
        // Resets to the original size.
        transform.localScale = objectOriginalScale;
        // Start the next OnMouseOver() with growth.
        scaleFactorSignMultiplier = 1;

    }

    
    private void Update()
    {
        
    }


}
