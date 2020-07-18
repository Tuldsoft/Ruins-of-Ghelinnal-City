using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the prefabFileMenu. Used as both a Load Game and Save Game menu.
/// Buttons are adjusted based on the scenario.
/// Creates "slots" representing current save game files, which are external files.
/// File operations are handled by FileUtil. Player-specific info is stored in PlayerPrefs.
/// REFACTOR: Make a generic "panel" interface that can be used by this and any inventory menu.
/// </summary>
public class FileMenuMonitor : MonoBehaviour
{
    #region Fields
    // Fields set for the prefab
    [SerializeField]
    Text welcomeLabel = null, newSlotText = null, deleteSaveText = null,
        saveText = null, loadText = null;
    [SerializeField]
    GameObject gridContents = null;
    
    // Index of the selected slot
    int? selection = null;

    // prefab dialog box
    GameObject confirmationDialog;
    #endregion

    #region Methods
    // Sets up the menu as either a Load or Save menu, used intead of Start()
    public void SetupFileMenu(bool asSave)
    {
        // Set up as a listener for selection events and confirmation
        EventManager.AddListener_PanelSelect(MakeSelection);
        EventManager.AddListener_Confirmation(ConfirmDeleteSlot);

        // Shows Load, Save, Create Save, Delete, etc. buttons
        ShowHideButtons(asSave);
        
        // Produces clickable slots for each save file curently existing
        PopulateGrid();

        // Sets the welcome label
        if (asSave)
        {
            welcomeLabel.text = "Select a file to save.";
        }
        else
        { 
            welcomeLabel.text = "Select a file to load.";
        }
    }

    // Chooses which buttons to display or hide, based on whether a Save or Load Game
    void ShowHideButtons(bool asSave)
    {
        newSlotText.gameObject.transform.parent.gameObject.SetActive(asSave);
        //deleteSaveText.gameObject.transform.parent.gameObject.SetActive(asSave); 
        saveText.gameObject.transform.parent.gameObject.SetActive(asSave);
        loadText.gameObject.transform.parent.gameObject.SetActive(!asSave);
    }

    // Creates slots as buttons, representing available save files
    void PopulateGrid()
    {
        // Create template panel (button)
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabFileSlotPanel");

        // PlayerPrefs keeps track of the *maximum* number of slots that has ever
        // been created by the player. This is different from the number of files available.
        int maxPotentialSlots = PlayerPrefs.HasKey(PlayerPrefsKeys.HighestSlotNum.ToString()) ?
            PlayerPrefs.GetInt(PlayerPrefsKeys.HighestSlotNum.ToString()) : 0;

        // PlayerPrefs has an entry for each save file created. The Key is "Slot#SaveName", and
        // the value is string saying either "No Save Data" or listing of the hero's names,
        // the party gold, and the date of the save.

        // Create a button for each key
        for (int i = 0; i < maxPotentialSlots; i++)
        {
            if (PlayerPrefs.HasKey(KeyName(i)))
            {
                // Create from template
                newPanel = GameObject.Instantiate(prefabPanel, gridContents.transform);
                
                // Access FilePanel component
                PanelSelection filePanel = newPanel.GetComponent<PanelSelection>();
                
                // Set up the panel with the string to display
                filePanel.SetupPanel(i, PlayerPrefs.GetString(KeyName(i)));
            }
        }

        // Disable buttons until a slot is clicked
        EnableButtons(false);
    }

    // Destroys the displayed slots, in order to repopulate an updated display
    void ClearGrid()
    {
        for (int i = 0; i < gridContents.transform.childCount; i++)
        {
            Destroy(gridContents.transform.GetChild(i).gameObject);
        }
    }

    // Converts an int to a string, used as a key in PlayerPrefs
    string KeyName (int slot)
    {
        return "Slot" + slot + "SaveName";
    }

    // Selects or deselects a slot button. Invoked when a slot is clicked.
    void MakeSelection(int? index)
    {
        if (index != null)
        {

            if (selection == index)
            {
                selection = null;
                EnableButtons(false);
            }
            else
            {
                selection = index;
                EnableButtons(true);
            }
        }
        else
        {
            Debug.Log("null index sent through events");
        }
    }

    // Sets buttons (Load, Save, etc.) as either interactable or not
    void EnableButtons(bool on)
    {
        Color color = on ? Color.white : Color.gray;
        
        deleteSaveText.GetComponent<Button>().interactable = on;
        deleteSaveText.color = color;
        saveText.GetComponent<Button>().interactable = on;
        saveText.color = color;
        loadText.GetComponent<Button>().interactable = on;
        loadText.color = color;
    }

    // Create a slot button representing an empty save slot
    public void Click_NewSlot()
    {
        AudioManager.Chirp();
        Debug.Log("Create new slot");
        
        // Count how many consecutive save slots are mentioned in PlayerPrefs
        // If a slot has been deleted, this will find the gap.
        int i = 0;
        while (PlayerPrefs.HasKey(KeyName(i)))
        {
            i++;
        }

        // Create an empty entry in PlayerPrefs
        PlayerPrefs.SetString(KeyName(i), "No Save Data");

        // If a new highest is created, store the new highest
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.HighestSlotNum.ToString()))
        {
            int highest = PlayerPrefs.GetInt(PlayerPrefsKeys.HighestSlotNum.ToString());
            if (i > highest)
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.HighestSlotNum.ToString(), i);
            }
        }
        else
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.HighestSlotNum.ToString(), i);
        }

        // Recreate the grid including the new slot
        ClearGrid();
        PopulateGrid();

        PlayerPrefs.Save();
    }

    // Called by the Delete button's On_Click().
    public void Click_DeleteSlot()
    {
        AudioManager.Chirp();
        
        // Load confirmation dialogue, Delete when OK is clicked.
        confirmationDialog = Resources.Load<GameObject>(@"MenuPrefabs\prefabConfirmation");
        GameObject.Instantiate(confirmationDialog);
        
    }

    // Called by confirmation event
    void ConfirmDeleteSlot(bool ok)
    {
        // If cancel, then do nothing
        
        if (ok)
        {
            // Delete the key from playerprefs
            PlayerPrefs.DeleteKey(KeyName((int)selection));

            // Redo the grid without the deleted slot.
            ClearGrid();
            PopulateGrid();

            PlayerPrefs.Save();
        }
    }

    // Called by Save button On_Click()    
    public void Click_SaveGame()
    {
        // Only save when a selection is active
        AudioManager.PlaySound(AudioClipName.Save);
        if (selection != null)
        {
            // Create a new SaveFile (class instance), based on slot selected
            SaveFile saveFile = new SaveFile((int)selection);
            
            // Put game data into the saveFile, then write it into a file
            saveFile.SaveData();
            FileUtil.WriteData(saveFile);

            // Set PlayerPrefs value from saveFile
            PlayerPrefs.SetString(KeyName((int)selection), saveFile.ToString());

            // Redo display
            ClearGrid();
            PopulateGrid();

            PlayerPrefs.Save();
        }
    }

    // Called by Load button On_Click()
    public void Click_LoadGame()
    {
        AudioManager.PlaySound(AudioClipName.Save); 
        if (selection != null)
        {
            // Replace current game data with data from the file
            FileUtil.LoadData((int)selection);

            // Go back to town and exit menu
            MenuManager.GoToMenu(MenuName.MainToTown);
            Destroy(gameObject);
        }
    }

    // Destroy this menu and return to previous
    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }
    #endregion
}
