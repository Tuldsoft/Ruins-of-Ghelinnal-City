using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonLevelMonitor : MonoBehaviour
{
    [SerializeField]
    public int dungeonLevel = 1;

    [SerializeField]
    Text partyGoldText = null;

    private void Awake()
    {
        Initializer.Run();
    }

    // Start is called before the first frame update
    void Start()
    {
        BattleLoader.LoadLevel(dungeonLevel);
        partyGoldText.text = BattleLoader.Party.Gold.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.FindGameObjectWithTag("PauseMenu") == null)
            {
                MenuManager.GoToMenu(MenuName.Pause);
            }
        }
    }
}
