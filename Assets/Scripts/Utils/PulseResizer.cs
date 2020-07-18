using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This makes a GameObject "pulse" when the mouse hovers over it. Created for another project.
// Currently used on battle enemy sprites to help identify them as clickable. Requires a collider.

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
        // Store original sclae for later use in OnMouseExit()
        objectOriginalScale = transform.localScale;
    }

    // When the mouse moves over the collider, begin pulsing
    void OnMouseOver()
    {
        // Calculate amount of size to add
        float addSize = scaleFactorSignMultiplier * scaleFactorPerSecond * Time.deltaTime;

        // Resize the object by adjusting localScale
        transform.localScale = new Vector3(transform.localScale.x + addSize,
            transform.localScale.y + addSize, 1);

        // Add to time
        elapsedResizeSeconds += Time.deltaTime;

        // Reverse the growth / shrinkage, cyclically by time
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

}
