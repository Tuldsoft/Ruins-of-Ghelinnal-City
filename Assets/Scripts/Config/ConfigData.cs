using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;

/// <summary>
/// ConfigData is a transfer container class, used for reading information from a csv file. The
/// information stored into it, and then passed on to a static class such as BattleAbilityData
/// as BattleAbilityData.
/// REFACTOR: Create a parent and children classes
/// </summary>
public class ConfigData 
{
    #region Fields
    // File names that this class handles
    const string EnemyDataFileName = "EnemyData.csv";
    const string InvDataFileName = "InvData.csv";
    const string AbilityDataFileName = "AbilityData.csv";

    // Dictionaries to store the data once read from the csv file. 
    Dictionary<EnemyName, BattleStats> enemyStatData = new Dictionary<EnemyName, BattleStats>();
    Dictionary<InvNames, InvItem> invStatData = new Dictionary<InvNames, InvItem>();
    Dictionary<BattleMode, BattleAbility> abilityStatData = new Dictionary<BattleMode, BattleAbility>();

    // used for printing messages on-screen
    static string errorMessage = null;

    #endregion

    #region Properties

    // Dictionaries to store the data and pass it on to a static class
    public Dictionary<EnemyName, BattleStats> EnemyStatData { get { return enemyStatData; } }

    public Dictionary<InvNames, InvItem> InvStatData { get { return invStatData; } }

    public Dictionary<BattleMode, BattleAbility> AbilityStatData { get { return abilityStatData; } }

    #endregion

    #region Constructor

    /// <summary>
    /// Reads configuration data from a file. If the file read fails, the constructor contains
    /// default values for each kind of configuration data
    /// REFACTOR: Break this into three different constructors
    /// </summary>
    public ConfigData(ConfigDataType type)
    {
        // Read and save configuration data from file
        StreamReader input = null;

        // Three kinds of ConfigData
        switch (type)
        {
            case ConfigDataType.EnemyData:
                #region EnemyData

                try
                {
                    // Create stream reader input
                    input = File.OpenText(Path.Combine(
                        Application.streamingAssetsPath, EnemyDataFileName));

                    // Populate StatNames from header row 
                    string currentLine = input.ReadLine();
                    string[] names = currentLine.Split(',');
                    BattleStatNames[] statHeader = new BattleStatNames[names.Length];

                    // Validate the header row before importing
                    BattleStats validate = new BattleStats();

                    for (int i = 1; i < names.Length; i++)
                    {
                        statHeader[i] = (BattleStatNames)Enum.Parse(typeof(BattleStatNames), names[i]);
                        if (!validate.ValidateOrder(i - 1, statHeader[i]))
                        {
                            errorMessage = "Headers do not match BattleStats.\nUsing default settings.";
                            Debug.Log(errorMessage);
                            SetEnemyStatDataDefaultValues();
                            break;
                        }
                    }

                    // Only procede forward if there is no error with the headers
                    if (errorMessage == null)
                    {
                        // Populate values for enemyData
                        currentLine = input.ReadLine();
                        while (currentLine != null)
                        {
                            // Parse current line into name and stat values
                            string[] tokens = currentLine.Split(',');
                            EnemyName enemyName =
                                (EnemyName)Enum.Parse(
                                    typeof(EnemyName), tokens[0]);

                            int[] intTokens = new int[tokens.Length - 1];

                            for (int i = 0; i < tokens.Length - 1; i++)
                            {
                                int bStat;
                                intTokens[i] = int.TryParse(tokens[i + 1], out bStat) ? bStat : 0; 
                            }

                            // import the array of ints into a new BattleStats
                            BattleStats battleStats = new BattleStats();
                            battleStats.Import(intTokens);

                            // add to enemyStatData
                            enemyStatData.Add(enemyName, battleStats);

                            // queue next line in csv
                            currentLine = input.ReadLine();
                        }
                    }

                }
                catch (Exception e)
                {
                    // send the error to the console for debugging
                    Debug.Log(e);
                    errorMessage = "Problem loading file. \nUsing default settings.\n";
                    Debug.Log(errorMessage);

                    SetEnemyStatDataDefaultValues();
                }
                finally
                {
                    // close files that were opened
                    if (input != null)
                    {
                        input.Close();
                    }
                }
                
                break; // end of EnemyData
                #endregion EnemyData
            
            case ConfigDataType.InvData:
                #region InvData
                
                try
                {
                    // create stream reader input
                    input = File.OpenText(Path.Combine(
                        Application.streamingAssetsPath, InvDataFileName));

                    // populate StatNames from header row 
                    string currentLine = input.ReadLine();
                    string[] headings = currentLine.Split(',');
                    BattleStatNames[] statHeader = new BattleStatNames[headings.Length];

                    // Validate the header row before importing
                    // 0 name, 1 fullname, 2 description, 3 id, 4 price, 5 type, 6 subtype, 7 slot,
                    // 8 rank, 9+ Battlestats
                    BattleStats validate = new BattleStats();
                    
                    for (int i = 9; i < headings.Length; i++)
                    {
                        statHeader[i] = (BattleStatNames)Enum.Parse(typeof(BattleStatNames), headings[i]);
                        if (!validate.ValidateOrder(i - 9, statHeader[i]))
                        {
                            errorMessage = "Headers do not match BattleStats.\nUsing default settings.";
                            Debug.Log(errorMessage);
                            SetInvDataDefaultValues();
                            break;
                        }
                    }

                    // only procede forward if there is no error with the headers
                    if (errorMessage == null)
                    {
                        // populate values for enemyData
                        currentLine = input.ReadLine();
                        while (currentLine != null)
                        {
                            // parse current line into name and stat values
                            // 0 name, 1 fullname, 2 description, 3 id, 4 price, 5 type, 6 subtype, 7 slot, 8 rank

                            // 0 name
                            string[] tokens = currentLine.Split(',');
                            InvNames invName =
                                (InvNames)Enum.Parse(
                                    typeof(InvNames), tokens[0]);
                                                        
                            // 1 fullname
                            string fullName = tokens[1];

                            // 2 description
                            string description = tokens[2];

                            // 3 id
                            int id = int.Parse(tokens[3]);

                            // 4 price
                            int price = int.Parse(tokens[4]);

                            // 5 type
                            InvType invType = (InvType)Enum.Parse(
                                    typeof(InvType), tokens[5]);

                            // 6 subtype
                            InvSubtype invSubtype = (InvSubtype)Enum.Parse(
                                typeof(InvSubtype), tokens[6]);

                            Sprite sprite = Resources.Load<Sprite>(@"Sprites\InvItems\"
                                + invType.ToString() + @"\" 
                                + invSubtype.ToString() + @"\"
                                + invName.ToString());

                            // 7 slot
                            EquipSlots slot = (EquipSlots)Enum.Parse(
                                typeof(EquipSlots), tokens[7]);

                            // 8 rank
                            int rank = int.Parse(tokens[8]);
                                                        

                            // BattleStats is only included for InvEqItems (equipment and tomes)
                            // This is an InvEqItem
                            if (invType == InvType.Tome || invType == InvType.Weapon || invType == InvType.Armor)
                            {

                                // 9+ BattleStats

                                int[] intTokens = new int[tokens.Length - 9];

                                for (int i = 0; i < tokens.Length - 9; i++)
                                {
                                    int bStat;
                                    intTokens[i] = int.TryParse(tokens[i + 9], out bStat) ? bStat : 0;
                                }

                                // import the array of ints into a new BattleStats
                                BattleStats battleStats = new BattleStats();
                                battleStats.Import(intTokens);

                                // create a new InvEqItem
                                InvEqItem item = new InvEqItem(invName, fullName, description, sprite, 
                                    id, price, 0, invType, invSubtype, slot, rank, battleStats);

                                // add to invStatData
                                invStatData.Add(invName, item);
                            }
                            // else it is a potion, so add a regular InvItem
                            else
                            {
                                // create a new InvEqItem
                                InvItem item = new InvItem(invName, fullName, description, sprite,
                                    id, price, 0, invType, invSubtype, slot, rank);

                                // add to invStatData
                                invStatData.Add(invName, item);
                            }

                            // queue next line in csv
                            currentLine = input.ReadLine();
                        }
                    }

                }
                catch (Exception e)
                {
                    // send the error to the console for debugging
                    Debug.Log(e);
                    errorMessage = "Problem loading file. \nUsing default settings.\n";
                    Debug.Log(errorMessage);

                    SetInvDataDefaultValues();
                }
                finally
                {
                    // close files that were opened
                    if (input != null)
                    {
                        input.Close();
                    }
                }

                break;
                #endregion InvData

            case ConfigDataType.AbilityData:
                #region AbilityData
                try
                {
                    // create stream reader input
                    input = File.OpenText(Path.Combine(
                        Application.streamingAssetsPath, AbilityDataFileName));

                    // populate StatNames from header row 
                    string currentLine = input.ReadLine();
                    string[] headings = currentLine.Split(',');
                    
                    // ignoring the header row
                    
                    // populate values for abilityData
                    currentLine = input.ReadLine();
                    while (currentLine != null)
                    {
                        // parse current line into name and stat values
                        // 0 name, 1 isPhysical, 2 mp, 3 noReduction, 4 modifier, 
                        // 5 noMiss, 6 hitOverride, 7 noCrit

                        // 0 name
                        string[] tokens = currentLine.Split(',');
                        BattleMode name =
                            (BattleMode)Enum.Parse(
                                typeof(BattleMode), tokens[0]);

                        // 1 isPhyical
                        bool isPhysical = bool.Parse(tokens[1].ToLower());

                        // 2 mp
                        int? mp = null;
                        if (int.TryParse(tokens[2], out int num))
                        {
                            mp = num;
                        }
                        else
                        {
                            mp = null;
                        }

                        // 3 noReduction
                        bool noReduction = bool.Parse(tokens[3].ToLower());

                        // 4 modifier
                        float modifier = float.Parse(tokens[4]);

                        // 5 noMiss
                        bool noMiss = bool.Parse(tokens[5].ToLower());

                        // 6 hitOverride
                        int? hitOverride = null;
                        if (int.TryParse(tokens[6], out int num2))
                        {
                            hitOverride = num2;
                        }
                        else
                        {
                            hitOverride = null;
                        }

                        // 7 noCrit
                        bool noCrit = bool.Parse(tokens[7].ToLower());


                        // create a new battleAbility
                        BattleAbility ability = new BattleAbility(name, isPhysical, mp, noReduction, modifier, noMiss, hitOverride, noCrit);

                        // add to invStatData
                        abilityStatData.Add(name, ability);

                        // queue next line in csv
                        currentLine = input.ReadLine();
                    }

                }
                catch (Exception e)
                {
                    // send the error to the console for debugging
                    Debug.Log(e);
                    errorMessage = "Problem loading file. \nUsing default settings.\n";
                    Debug.Log(errorMessage);

                    SetAbilityDataDefaultValues();
                }
                finally
                {
                    // close files that were opened
                    if (input != null)
                    {
                        input.Close();
                    }
                }

                break;
            #endregion AbilityData

            default:
                break;
        }
    }

    #endregion

    #region Methods
    // Sets default values for all* enemies (incomplete list). Used only in case of a read error.
    // REFACTOR: Finish typing out stats, or set all to stats of a wererat.
    void SetEnemyStatDataDefaultValues()
    {
        enemyStatData.Clear();
        
        EnemyName name;
        BattleStats stats = new BattleStats();

        /*baseHPMax, baseMPMax, 
         strength, defense, magic, resistance,
         stamina, agility, baseHit, baseEvade, critChance,
         minReward, maxReward*/

        name = EnemyName.none;
        stats.Import(new int[13]
        {   1,   1,   1,   1,   1,   1,   1,   1,   1,   1,   1, 1, 1 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Wererat;
        stats = new BattleStats();
        stats.Import(new int[13]
        {   7,   7,   2,   1,   1,   1,   1,   5,  10,   5,   5, 11, 21 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Ultros;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  20,  20,   7,   5,   5,   5,  15,  10,  25,   0,   0, 61, 89 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Guard;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  12,  20,   6,   8,   3,   8,  10,  10,   5,   5,   0, 22, 44 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Wolf;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  20,  10,   8,   4,   3,   4,  20,  20,  10,  10,   5, 20, 40 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Whelk;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  45,  30,  20,  20,  12,  20,  30,  20,   5,   0,   0, 70, 150 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Mechanic;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  16,  15,   7,   3,   4,   5,  15,  15,  10,  10,   5, 24, 50 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Grease_Monkey;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  22,  30,   8,   4,   8,   6,  15,  20,  10,  10,   5 , 27, 55});
        enemyStatData.Add(name, stats);

        name = EnemyName.Roper;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  20,  40,   8,  10,   6,   9,  30,  10,  10,   0,   0, 21, 30 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Magitek_Armor;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  38,  50,  22,  22,  22,  22,  40,  20,   5,   0,  10, 85, 150 });
        enemyStatData.Add(name, stats);

        name = EnemyName.Tunnel_Armor;
        stats = new BattleStats();
        stats.Import(new int[13]
        {  65,  60,  35,  35,  25,  25,  50,  50,   5,  10,  10, 175, 300});
        enemyStatData.Add(name, stats);

    }

    // Gives all inventory items mentioned in InvNames the statistics of a tiny health potion.
    // Used only in case of a read error.
    void SetInvDataDefaultValues()
    {
        invStatData.Clear();
        foreach (InvNames name in InvNames.GetValues(typeof(InvNames)))
        {
            invStatData.Add(name, new InvItem(InvNames.Potion_Health_Tiny, "default fullname", "default description", Resources.Load<Sprite>(@"Sprites\InvItems\Potion\Health\" + name.ToString()), 1, 0, 0, InvType.Potion, InvSubtype.Health, EquipSlots.none, 1));
        }
    }

    // Constructs default ability data for each ability named in BattleMode.
    // Used only in case of read error.
    void SetAbilityDataDefaultValues() 
    {
        abilityStatData.Clear();
        foreach (BattleMode name in BattleMode.GetValues(typeof(BattleMode)))
        {
            abilityStatData.Add(name, new BattleAbility(name));
        }
    }

    /*static string GetExceptionDetails(Exception exception)
    {
        PropertyInfo[] properties = exception.GetType().GetProperties();
        List<string> fields = new List<string>();
        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(exception, null);
            fields.Add(String.Format(
                             "{0} = {1}",
                             property.Name,
                             value != null ? value.ToString() : String.Empty
            ));
        }
        return String.Join("\n", fields.ToArray());
    }*/

    #endregion
}
