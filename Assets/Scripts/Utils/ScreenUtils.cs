using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides screen utilities
/// </summary>
public static class ScreenUtils
{
    #region Fields

    // cached for efficient boundary checking
    static float screenLeft;
    static float screenRight;
    static float screenTop;
    static float screenBottom;
    static float screenWidth;
    static float screenHeight;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the left edge of the screen in world coordinates
    /// </summary>
    /// <value>left edge of the screen</value>
    public static float ScreenLeft
    {
        get { return screenLeft; }
    }

    /// <summary>
    /// Gets the right edge of the screen in world coordinates
    /// </summary>
    /// <value>right edge of the screen</value>
    public static float ScreenRight
    {
        get { return screenRight; }
    }

    /// <summary>
    /// Gets the top edge of the screen in world coordinates
    /// </summary>
    /// <value>top edge of the screen</value>
    public static float ScreenTop
    {
        get { return screenTop; }
    }

    /// <summary>
    /// Gets the bottom edge of the screen in world coordinates
    /// </summary>
    /// <value>bottom edge of the screen</value>
    public static float ScreenBottom
    {
        get { return screenBottom; }
    }
    /// <summary>
    /// Gets the width of the screen in world coordinates
    /// </summary>
    /// <value>width of the screen</value>
    public static float ScreenWidth
    {
        get { return screenWidth; }
    }/// <summary>
     /// Gets the height of the screen in world coordinates
     /// </summary>
     /// <value>height of the screen</value>
    public static float ScreenHeight
    {
        get { return screenHeight; }
    }
    #endregion

    #region Methods

    /// <summary>
    /// Initializes the screen utilities
    /// </summary>
    public static void Initialize()
    {
        // save screen edges in world coordinates
        float screenZ = -Camera.main.transform.position.z;
        Vector3 lowerLeftCornerScreen = new Vector3(0, 0, screenZ);
        Vector3 upperRightCornerScreen = new Vector3(
            Screen.width, Screen.height, screenZ);
        Vector3 lowerLeftCornerWorld =
            Camera.main.ScreenToWorldPoint(lowerLeftCornerScreen);
        Vector3 upperRightCornerWorld =
            Camera.main.ScreenToWorldPoint(upperRightCornerScreen);
        screenLeft = lowerLeftCornerWorld.x;
        screenRight = upperRightCornerWorld.x;
        screenTop = upperRightCornerWorld.y;
        screenBottom = lowerLeftCornerWorld.y;
        
        screenWidth = 2 * upperRightCornerWorld.x;
        screenHeight = 2 * upperRightCornerWorld.y;
    }

    public static void AdjustCollider()
    {
        // adjust the edge collider on the camera
        Vector2[] edgeCollPoints = new Vector2[4];
        // bottom right
        edgeCollPoints[0].x = screenRight;
        edgeCollPoints[0].y = screenBottom;
        // up right
        edgeCollPoints[1].x = screenRight;
        edgeCollPoints[1].y = screenTop;
        // up left
        edgeCollPoints[2].x = screenLeft;
        edgeCollPoints[2].y = screenTop;
        // bottom left
        edgeCollPoints[3].x = screenLeft;
        edgeCollPoints[3].y = screenBottom;

        Camera.main.GetComponent<EdgeCollider2D>().points = edgeCollPoints;
    }

    #endregion
}
