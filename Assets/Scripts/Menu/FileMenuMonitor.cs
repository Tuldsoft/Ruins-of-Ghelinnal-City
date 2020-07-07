using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileMenuMonitor : MonoBehaviour
{
    [SerializeField]
    Text welcomeLabel = null, newSlotText = null, deleteSaveText = null,
        saveText = null, loadText = null;
    [SerializeField]
    GameObject gridContents = null;
    
    int? selection = null;
    //bool asSave = true;

    GameObject confirmationDialog;

    public void SetupFileMenu(bool asSave)
    {
        EventManager.AddListener_PanelSelect(MakeSelection);
        EventManager.AddListener_Confirmation(ConfirmDeleteSlot);

        //this.asSave = asSave;

        ShowHideButtons(asSave);
        PopulateGrid();


        if (asSave)
        {
            welcomeLabel.text = "Select a file to save.";
        }
        else
        { 
            welcomeLabel.text = "Select a file to load.";
        }
    }

    void ShowHideButtons(bool asSave)
    {
        newSlotText.gameObject.transform.parent.gameObject.SetActive(asSave);
        //deleteSaveText.gameObject.transform.parent.gameObject.SetActive(asSave);
        saveText.gameObject.transform.parent.gameObject.SetActive(asSave);
        loadText.gameObject.transform.parent.gameObject.SetActive(!asSave);

    }

    void PopulateGrid()
    {
        GameObject newPanel;
        GameObject prefabPanel = Resources.Load<GameObject>(@"MenuPrefabs\prefabFileSlotPanel");

        int maxPotentialSlots = PlayerPrefs.HasKey(PlayerPrefsKeys.HighestSlotNum.ToString()) ?
            PlayerPrefs.GetInt(PlayerPrefsKeys.HighestSlotNum.ToString()) : 0;

        for (int i = 0; i < maxPotentialSlots; i++)
        {
            if (PlayerPrefs.HasKey(KeyName(i)))
            {
                newPanel = GameObject.Instantiate(prefabPanel, gridContents.transform);

                PanelSelection filePanel = newPanel.GetComponent<PanelSelection>();
                filePanel.SetupPanel(i, PlayerPrefs.GetString(KeyName(i)));
                
            }
        }

        EnableButtons(false);

    }

    void ClearGrid()
    {
        for (int i = 0; i < gridContents.transform.childCount; i++)
        {
            Destroy(gridContents.transform.GetChild(i).gameObject);
        }
    }


    string KeyName (int slot)
    {
        return "Slot" + slot + "SaveName";
    }

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


    public void Click_NewSlot()
    {
        AudioManager.Chirp();
        Debug.Log("Create new slot");
        int i = 0;
        while (PlayerPrefs.HasKey(KeyName(i)))
        {
            i++;
        }

        PlayerPrefs.SetString(KeyName(i), "No Save Data");

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

        ClearGrid();
        PopulateGrid();

        PlayerPrefs.Save();
    }



    public void Click_DeleteSlot()
    {
        AudioManager.Chirp();
        // load confirmation dialogue
        confirmationDialog = Resources.Load<GameObject>(@"MenuPrefabs\prefabConfirmation");
        GameObject.Instantiate(confirmationDialog);
        //confirmation = confirmationDialog.GetComponent<Confirmation>();
        //confirmation.AddListener_Confirmation(ConfirmDeleteSlot);
    }

    void ConfirmDeleteSlot(bool ok)
    {
        // playerprefs delete key (selection)
        if (ok)
        {
            // delete key from playerprefs
            PlayerPrefs.DeleteKey(KeyName((int)selection));

            // redo grid
            ClearGrid();
            PopulateGrid();

            PlayerPrefs.Save();
        }

    }

    
    public void Click_SaveGame()
    {
        AudioManager.PlaySound(AudioClipName.Save);
        if (selection != null)
        {
            SaveFile saveFile = new SaveFile((int)selection);
            saveFile.SaveData();
            FileUtil.WriteData(saveFile);

            PlayerPrefs.SetString(KeyName((int)selection), saveFile.ToString());

            ClearGrid();
            PopulateGrid();

            PlayerPrefs.Save();
        }
        

    }

    public void Click_LoadGame()
    {
        AudioManager.PlaySound(AudioClipName.Save); 
        if (selection != null)
        {
            FileUtil.LoadData((int)selection);
            MenuManager.GoToMenu(MenuName.MainToTown);
            Destroy(gameObject);
        }
    }


    public void Click_CloseButton()
    {
        AudioManager.Close();
        Destroy(gameObject);
    }
}
