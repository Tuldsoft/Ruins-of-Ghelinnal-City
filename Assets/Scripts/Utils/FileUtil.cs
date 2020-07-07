using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

static public class FileUtil
{
    public static void WriteData(SaveFile save)
    {
        string path = Application.persistentDataPath
            + "/SaveSlot" + save.SaveSlot.ToString() + ".RoGCsave";
        Debug.Log("Saving to: " + path);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, save);
        file.Close();
    }


    // returns true if the fileload was successful
    public static bool LoadData(int saveSlot)
    {
        string path = Application.persistentDataPath
            + "/SaveSlot" + saveSlot.ToString() + ".RoGCsave";

        if (File.Exists(path))
        {
            // load file into a savefile
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            SaveFile saveFile = (SaveFile)bf.Deserialize(file);
            file.Close();

            // load info from the savefile
            if (saveFile.VersionNum == Application.version)
            {
                saveFile.LoadData();
                return true;
            }
            else
            {
                Debug.Log("Version Number Mismatch");
                return false;
            }
            
        }
        else
        {
            Debug.Log("File does not exist at path.");
            return false;
        }

    }


    public static void ResetData()
    {
        // reset BattleLoader
        BattleLoader.ResetData();

        // reset Shop
        Shop.ResetData();
    }

}
