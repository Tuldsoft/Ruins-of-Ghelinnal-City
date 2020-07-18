using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

/// <summary>
/// A static utility that serializes a SaveFile object. It writes and reads files from device storage.
/// </summary>
static public class FileUtil
{
    // Called by the FileMenuMonitor, it takes a SaveFile object and turns it into a file.
    public static void WriteData(SaveFile save)
    {
        // Create the path and filename as a string
        string path = Application.persistentDataPath
            + "/SaveSlot" + save.SaveSlot.ToString() + ".RoGCsave";
        Debug.Log("Saving to: " + path);

        // Serialize and create the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, save);
        
        // Close the file (necessary!)
        file.Close();
    }

    /// <summary>
    /// Retrieves a file from device storage and creates a SaveFile object from it.
    /// Runs the LoadDate() method on the SaveFile to export its values throughout the program.
    /// </summary>
    /// <param name="saveSlot">Numerical slot number of the file, as shown in its filename.</param>
    /// <returns>Returns true if the file load was successful.</returns>
    // eturns true if the fileload was successful
    public static bool LoadData(int saveSlot)
    {
        // Create path to the file
        string path = Application.persistentDataPath
            + "/SaveSlot" + saveSlot.ToString() + ".RoGCsave";

        if (File.Exists(path))
        {
            // Load file into a savefile
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            SaveFile saveFile = (SaveFile)bf.Deserialize(file);
            file.Close(); // necessary!!

            // Load info from the savefile into the program (mostly BattleLoader and Shop)
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

    // Nuke existing data and start fresh (Moved to Initializer)
    public static void ResetData()
    {
        // reset BattleLoader
        BattleLoader.ResetData();

        // reset Shop
        Shop.ResetData();
    }

}
