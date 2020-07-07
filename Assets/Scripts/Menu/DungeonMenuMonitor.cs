using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Provides methods for menu navigation
public class DungeonMenuMonitor : MonoBehaviour
{
    List<DungeonNode> dungeonLevelNodes = new List<DungeonNode>(BattleLoader.MaxDungeonLevelAccess);
    [SerializeField]
    int levelAccess = 1;

    float scale = 2f;
    Vector3 localScale;

    [SerializeField]
    GameObject prefabNode = null;
    [SerializeField]
    GameObject parentCanvas = null;

    private void Awake()
    {
        Initializer.Run();
    }

    private void Start()
    {
        if (BattleLoader.DungeonLevelAccess > levelAccess) 
        { 
            levelAccess = BattleLoader.DungeonLevelAccess; 
        }

        localScale = new Vector3(scale, scale, scale);

        GameObject firstNode = Instantiate(prefabNode);
        firstNode.transform.SetParent(parentCanvas.transform);
        //localScaleY = firstNode.transform.localScale.y;
        firstNode.transform.localScale = localScale;

        Vector2 position = firstNode.transform.position;
        
        position.y = (dungeonLevelNodes.Capacity * 32 / 2 + 16) * scale;
        Destroy(firstNode);

        for (int i = 1; i <= dungeonLevelNodes.Capacity; i++)
        {
            MakeDungeonNode(i, position);
            position = dungeonLevelNodes[i - 1].gameObject.transform.localPosition;
        }

        /*for (int i = 0; i < BattleLoader.Party.Hero.Length; i++)
        {
            BattleLoader.Party.Hero[i].HealAll();
        }*/

    }

    void MakeDungeonNode (int level, Vector2 position)
    {
        // make a new node
        GameObject node = Instantiate(prefabNode);
        node.transform.SetParent(parentCanvas.transform);
        node.transform.localScale = localScale;

        // position it
        position.y -= 32 * node.transform.localScale.y;
        node.transform.localPosition = position;

        // update text
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

        // add to list and initialize node
        DungeonNode nodeScript = node.GetComponent<DungeonNode>();
        nodeScript.SetLevel(level);
        dungeonLevelNodes.Add(nodeScript);
    }

    public void Click_Cheat_MaxAccess()
    {
        BattleLoader.DungeonLevelAccess = int.MaxValue;
        BattleLoader.GameCompleted = true;
        MenuManager.GoToMenu(MenuName.DungeonTown);
    }

    public void Click_ReturnToTown()
    {
        AudioManager.Close();
        MenuManager.GoToMenu(MenuName.DungeonTown);
    }

}
