using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the Main Camera of the Dungeon scene, where the user selects which level to enter.
/// </summary>

public class DungeonMenuMonitor : MonoBehaviour
{
    #region Fields
    // A "node" is a button that will take the user to a dungeon level. They are created at runtime.
    // DungeonNode is attached to one such button.
    List<DungeonNode> dungeonLevelNodes = new List<DungeonNode>(BattleLoader.MaxDungeonLevelAccess);
    
    // Default access to levels can be set in the inspector.
    [SerializeField]
    int levelAccess = 1;

    // Scale for the buttons
    float scale = 2f;
    Vector3 localScale;

    // Prefabs for iterating for each accessible level of the dungeon
    [SerializeField]
    GameObject prefabNode = null;
    [SerializeField]
    GameObject parentCanvas = null;
    #endregion

    #region Methods
    // Awake() is called before any Start()
    private void Awake()
    {
        Initializer.Run();
    }

    // Called before the first frame update.
    private void Start()
    {
        // Set the access level. A user can enter a dungeon level if the previous level is cleared.
        if (BattleLoader.DungeonLevelAccess > levelAccess) 
        { 
            levelAccess = BattleLoader.DungeonLevelAccess; 
        }

        // Scale is used for positioning buttons
        localScale = new Vector3(scale, scale, scale);

        // Instantiate a template node, and assign its scale and parent.
        GameObject firstNode = Instantiate(prefabNode);
        firstNode.transform.SetParent(parentCanvas.transform);
        firstNode.transform.localScale = localScale;

        // Store the initial positoin of the first node
        Vector2 position = firstNode.transform.position;
        
        // Raise the first button depending on how many buttons are to be created
        position.y = (dungeonLevelNodes.Capacity * 32 / 2 + 16) * scale;

        // Now that its position has been captured, destroy the template
        Destroy(firstNode);

        // Create all nodes and place them
        for (int i = 1; i <= dungeonLevelNodes.Capacity; i++)
        {
            MakeDungeonNode(i, position);
            position = dungeonLevelNodes[i - 1].gameObject.transform.localPosition;
        }
    }

    /// <summary>
    /// Creates a new button, containing a DungeonNode. DungeonNode stores an int identifier,
    /// which it will then relay back when it is clicked.
    /// </summary>
    /// <param name="level">The dungeon level that this node represents.</param>
    /// <param name="position">Position of the template node.</param>
    void MakeDungeonNode (int level, Vector2 position)
    {
        // Make a new node
        GameObject node = Instantiate(prefabNode);
        node.transform.SetParent(parentCanvas.transform);
        node.transform.localScale = localScale;

        // Position it
        position.y -= 32 * node.transform.localScale.y;
        node.transform.localPosition = position;

        // Update the text
        node.GetComponentInChildren<Text>().text = "Dungeon Level " + level;

        // enable or disable
        if (level <= levelAccess)
        {
            node.GetComponentInChildren<Button>().interactable = true;
        }
        else
        {
            node.GetComponentInChildren<Button>().interactable = false;
        }

        // Add the DungeonNode to the list and initialize it 
        DungeonNode nodeScript = node.GetComponent<DungeonNode>();
        nodeScript.SetLevel(level);
        dungeonLevelNodes.Add(nodeScript);
    }

    // Cheat button that enables all levels without clearing them first
    public void Click_Cheat_MaxAccess()
    {
        BattleLoader.DungeonLevelAccess = int.MaxValue;
        BattleLoader.GameCompleted = true;
        MenuManager.GoToMenu(MenuName.DungeonTown);
    }

    // Returns to the preivous menu
    public void Click_ReturnToTown()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.DungeonTown);
    }
    #endregion
}
