using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A container class with a constructor. Contains the BattleEnemys used in the battle
/// scene. The constructor determines which enemies are included and their placements, 
/// based on the creature that was contacted in the dungeon. Called by BattleManager.Start(),
/// and stored in BattleLoader.
/// </summary>
public class EnemyParty
{
    #region Fields and Properties
    // EnemyName of the enemy encountered in the dungeon. Ergo, the name of the battle encounter.
    EnemyName partyName;

    // Contain for each KIND of enemy, with an array of positions for each KIND.
    Dictionary<EnemyName, Vector2[]> placements = new Dictionary<EnemyName, Vector2[]>();

    // Remaining enemies in the party
    List<BattleEnemy> enemies = new List<BattleEnemy>();
    public List<BattleEnemy> Enemies { get { return enemies; } }

    // Whether or not this PARTY is the boss encounter of the dungeon
    bool isBoss = false;
    public bool IsBoss { get { return isBoss; } }

    // If there is a boss enemy WITHIN the party, the EnemyName of that boss.
    // Sets the BattleEnemy.IsBoss to true for that EnemyName
    EnemyName? bossID = null;

    #endregion

    #region Constructor
    public EnemyParty(EnemyName party)
    {
        // Recyclable array of positions for a single KIND of enemy
        Vector2[] positions;

        // Based on the enemy encountered in the dungeon, form an EnemyParty
        switch (party)
        {
            case EnemyName.none:
                // this should not occur, but if it does, one wererat
                positions = new Vector2[1];
                positions[0] = new Vector2(2.62f, 0.49f);
                placements.Add(EnemyName.Wererat, positions);
                break;

            case EnemyName.Wererat:
                // three wererats
                positions = new Vector2[3];
                positions[0] = new Vector2(2.62f, 0.49f);
                positions[1] = new Vector2(3.39f, 2.73f);
                positions[2] = new Vector2(5.36f, 1.06f);
                placements.Add(EnemyName.Wererat, positions);
                break;

            case EnemyName.Ultros:
                // boss battle
                // two wererats and one ultros
                positions = new Vector2[2];
                positions[0] = new Vector2(1.94f, 0.71f);
                positions[1] = new Vector2(2.24f, 2.35f);
                placements.Add(EnemyName.Wererat, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(5.44f, 1.50f);
                placements.Add(EnemyName.Ultros, positions);

                isBoss = true;
                bossID = EnemyName.Ultros;
                break;

            case EnemyName.Wolf:
                // two wolves, two guards
                positions = new Vector2[2];
                positions[0] = new Vector2(2.9f, 0.4f);
                positions[1] = new Vector2(3.7f, 2.4f);
                placements.Add(EnemyName.Wolf, positions);

                positions = new Vector2[2];
                positions[0] = new Vector2(5.3f, 0.4f);
                positions[1] = new Vector2(6.1f, 2.4f); 
                placements.Add(EnemyName.Guard, positions);
                break;

            case EnemyName.Whelk:
                // one whelk
                positions = new Vector2[1];
                positions[0] = new Vector2(4.8f, 1.80f);
                placements.Add(EnemyName.Whelk, positions);

                isBoss = true;
                bossID = EnemyName.Whelk;
                break;

            case EnemyName.Mechanic:
                // four mechanics
                positions = new Vector2[4];
                positions[0] = new Vector2(1.5f, 1.50f);
                positions[1] = new Vector2(3.3f, 1.50f);
                positions[2] = new Vector2(5.1f, 1.50f);
                positions[3] = new Vector2(6.9f, 1.50f);
                placements.Add(EnemyName.Mechanic, positions);
                break;

            case EnemyName.Grease_Monkey:
                // three grease monkeys, three mechanics
                positions = new Vector2[3];
                positions[0] = new Vector2(2.3f, 0.8f);
                positions[1] = new Vector2(4.3f, 3.1f);
                positions[2] = new Vector2(6.3f, 0.8f);
                placements.Add(EnemyName.Mechanic, positions);

                positions = new Vector2[3];
                positions[0] = new Vector2(2.3f, 2.4f);
                positions[1] = new Vector2(4.3f, 0.1f);
                positions[2] = new Vector2(6.3f, 2.4f);
                placements.Add(EnemyName.Grease_Monkey, positions);
                break;

            case EnemyName.Roper:
                // three ropers
                positions = new Vector2[3];
                positions[0] = new Vector2(2.6f, 0.2f);
                positions[1] = new Vector2(4.1f, 2.7f);
                positions[2] = new Vector2(6.4f, 0.6f);
                placements.Add(EnemyName.Roper, positions);
                break;

            case EnemyName.Magitek_Armor:
                // two magitek
                positions = new Vector2[2];
                positions[0] = new Vector2(1.7f, 2.1f);
                positions[1] = new Vector2(5.4f, 1.2f);
                placements.Add(EnemyName.Magitek_Armor, positions);
                break;

            case EnemyName.Tunnel_Armor:
                // two magitek
                positions = new Vector2[1];
                positions[0] = new Vector2(3.85f, 1.6f);
                placements.Add(EnemyName.Tunnel_Armor, positions);

                isBoss = true;
                bossID = EnemyName.Tunnel_Armor;
                break;

            case EnemyName.Cirpius:
                // three Cirpius
                positions = new Vector2[3];
                positions[0] = new Vector2(1.8f, 2.2f);
                positions[1] = new Vector2(4.0f, 1.4f);
                positions[2] = new Vector2(6.0f, 2.8f);
                placements.Add(EnemyName.Cirpius, positions);
                break;

            case EnemyName.Brawler:
                // two Brawlers, two Cirpius
                positions = new Vector2[2];
                positions[0] = new Vector2(2.0f, 1.2f);
                positions[1] = new Vector2(4.8f, 0.4f);
                placements.Add(EnemyName.Brawler, positions);

                positions = new Vector2[2];
                positions[0] = new Vector2(3.4f, 3.2f);
                positions[1] = new Vector2(6.1f, 2.4f);
                placements.Add(EnemyName.Cirpius, positions);
                break;

            case EnemyName.Tusker:
                // two tuskers, two brawlers
                positions = new Vector2[2];
                positions[0] = new Vector2(2.6f, 0.7f);
                positions[1] = new Vector2(2.2f, 2.6f);
                placements.Add(EnemyName.Tusker, positions);

                positions = new Vector2[2];
                positions[0] = new Vector2(5.4f, 0.7f);
                positions[1] = new Vector2(5.0f, 2.6f);
                placements.Add(EnemyName.Brawler, positions);
                break;

            case EnemyName.Vargas:
                // two brawlers, one Vargas
                positions = new Vector2[2];
                positions[0] = new Vector2(0.8f, 1.6f);
                positions[1] = new Vector2(2.8f, 0.8f);
                placements.Add(EnemyName.Brawler, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(5.0f, 1.7f);
                placements.Add(EnemyName.Vargas, positions);

                isBoss = true;
                bossID = EnemyName.Vargas;
                break;

            case EnemyName.Bomb:
                // five bombs
                positions = new Vector2[5];
                positions[0] = new Vector2(2.2f, 1.8f);
                positions[1] = new Vector2(3.8f, 0.2f);
                positions[2] = new Vector2(3.4f, 3.3f);
                positions[3] = new Vector2(5.8f, 3.1f);
                positions[4] = new Vector2(6.1f, 1.0f);
                placements.Add(EnemyName.Bomb, positions);
                break;

            case EnemyName.Sky_Armor:
                // three sky armor
                positions = new Vector2[3];
                positions[0] = new Vector2(2.7f, 1.6f);
                positions[1] = new Vector2(5.0f, 3.1f);
                positions[2] = new Vector2(6.6f, 0.9f);
                placements.Add(EnemyName.Sky_Armor, positions);
                break;

            case EnemyName.Spit_Fire:
                // one skyarmor, three bombs, one spitfire
                positions = new Vector2[1];
                positions[0] = new Vector2(1.9f, 0.7f);
                placements.Add(EnemyName.Sky_Armor, positions);

                positions = new Vector2[3];
                positions[0] = new Vector2(3.2f, 3.4f);
                positions[1] = new Vector2(4.2f, 0.9f);
                positions[2] = new Vector2(6.1f, -0.1f);
                placements.Add(EnemyName.Bomb, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(5.8f, 2.2f);
                placements.Add(EnemyName.Spit_Fire, positions);
                break;

            case EnemyName.Chupon:
                // one Ultros_Again, one Chupon
                positions = new Vector2[1];
                positions[0] = new Vector2(1.6f, 1.0f);
                placements.Add(EnemyName.Ultros_Again, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(5.0f, 1.7f);
                placements.Add(EnemyName.Chupon, positions);

                isBoss = true;
                bossID = EnemyName.Chupon;
                break;

            case EnemyName.Ghost:
                // Three ghosts
                positions = new Vector2[3];
                positions[0] = new Vector2(2.9f, 2.3f);
                positions[1] = new Vector2(3.8f, 0.3f);
                positions[2] = new Vector2(5.7f, 1.7f); 
                placements.Add(EnemyName.Ghost, positions);
                break;

            case EnemyName.Poplium:
                // two poplium, two whisper
                positions = new Vector2[2];
                positions[0] = new Vector2(2.5f, 2.2f);
                positions[1] = new Vector2(4.9f, 2.9f);
                placements.Add(EnemyName.Poplium, positions);

                positions = new Vector2[2];
                positions[0] = new Vector2(3.1f, 0.3f);
                positions[1] = new Vector2(5.5f, 0.9f);
                placements.Add(EnemyName.Whisper, positions);
                break;

            case EnemyName.Whisper:
                // three whispers
                positions = new Vector2[3];
                positions[0] = new Vector2(2.4f, 0.2f);
                positions[1] = new Vector2(3.6f, 2.8f);
                positions[2] = new Vector2(5.5f, 0.8f);
                placements.Add(EnemyName.Whisper, positions);
                break;

            case EnemyName.Phantom_Train:
                // one Phantom_train
                positions = new Vector2[1];
                positions[0] = new Vector2(5.22f, 1.60f);
                placements.Add(EnemyName.Phantom_Train, positions);

                isBoss = true;
                bossID = EnemyName.Phantom_Train;
                break;

            case EnemyName.Apokryphos:
                // three apokryphos
                positions = new Vector2[3];
                positions[0] = new Vector2(0.8f, 0.9f);
                positions[1] = new Vector2(3.2f, 3.0f);
                positions[2] = new Vector2(5.3f, 0.3f);
                placements.Add(EnemyName.Apokryphos, positions);
                break;

            case EnemyName.Brainpan:
                // two brainpan, one apok, one misfit
                positions = new Vector2[2];
                positions[0] = new Vector2(3.0f, 0.2f);
                positions[1] = new Vector2(5.8f, 0.6f);
                placements.Add(EnemyName.Brainpan, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(2.0f, 2.5f);
                placements.Add(EnemyName.Apokryphos, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(5.1f, 2.9f);
                placements.Add(EnemyName.Misfit, positions);
                break;

            case EnemyName.Misfit:
                // two misfit, one apok
                positions = new Vector2[2];
                positions[0] = new Vector2(1.4f, 0.5f);
                positions[1] = new Vector2(4.0f, 2.6f);
                placements.Add(EnemyName.Misfit, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(6.2f, 0.5f);
                placements.Add(EnemyName.Apokryphos, positions);
                break;

            case EnemyName.Wirey_Dragon:
                // three wirey dragon
                positions = new Vector2[3];
                positions[0] = new Vector2(0.9f, 1.4f);
                positions[1] = new Vector2(4.0f, 2.8f);
                positions[2] = new Vector2(6.4f, 1.2f);
                placements.Add(EnemyName.Wirey_Dragon, positions);
                break;

            case EnemyName.Ninja:
                // two ninja, one wirey dragon
                positions = new Vector2[2];
                positions[0] = new Vector2(1.5f, 0.5f);
                positions[1] = new Vector2(6.3f, 0.9f);
                placements.Add(EnemyName.Ninja, positions);

                positions = new Vector2[1];
                positions[0] = new Vector2(4.0f, 2.8f);
                placements.Add(EnemyName.Wirey_Dragon, positions);
                break;

            case EnemyName.Behemoth:
                // two behemoth
                positions = new Vector2[2];
                positions[0] = new Vector2(2.2f, 1.6f);
                positions[1] = new Vector2(5.9f, 1.6f);
                placements.Add(EnemyName.Behemoth, positions);
                break;

            case EnemyName.Dragon:
                // one dragon
                positions = new Vector2[1];
                positions[0] = new Vector2(4.45f, 1.63f);
                placements.Add(EnemyName.Dragon, positions);
                break;

            case EnemyName.Atma_Weapon:
                // two poplium, two whisper
                positions = new Vector2[1];
                positions[0] = new Vector2(3.27f, 1.65f);
                placements.Add(EnemyName.Atma_Weapon, positions);

                isBoss = true;
                bossID = EnemyName.Atma_Weapon;
                break;

            default:
                // user for debugging dungeons one wererat
                positions = new Vector2[1];
                positions[0] = new Vector2(2.6f, 0.5f);
                placements.Add(EnemyName.Wererat, positions);
                break;
        }

        // Read through placements, assigning BattleIDs in the process 
        
        // index becomes battleID. It is also the index for the list of enemies.
        int index = 0;

        // For each KIND of enemy (EnemyName)
        foreach (KeyValuePair<EnemyName,Vector2[]> pair in placements)
        {
            // Load a prefab
            GameObject enemyPrefab = Resources.Load<GameObject>(@"BattlePrefabs\Enemies\" 
                + pair.Key.ToString());

            // For each enemy of this Kind (EnemyName)
            for (int i = 0; i < pair.Value.Length; i++)
            {
                // Instantiate the prefab GameObject
                GameObject enemy = GameObject.Instantiate(enemyPrefab);

                // If there are multiple enemies of this Kind, they are
                // differentiated here. (Ex: "Wererat 1", "Wererat 2")
                int choiceNumTag = 0;
                if (pair.Value.Length > 1) { choiceNumTag = i + 1; }

                // Sets this single enemy as IsBoss
                bool boss = false;
                if (pair.Key == bossID) { boss = true; }
                
                // Create a BattleEnemy and add it to the party
                BattleEnemy bEnemy = BattleEnemyData.MakeNewBattleEnemy(pair.Key);
                enemies.Add(bEnemy);

                // Provides BattleEnemy setup info (including its gameObject)
                bEnemy.LoadObjs(index, choiceNumTag, enemy, pair.Value[i], boss);

                // Sets DamageDisplay and HPMPSliders and EnemyDeath
                enemy.GetComponentInChildren<DamageDisplay>(true).SetDamageDisplay(index, 
                    bEnemy.HP, bEnemy.HPMax, bEnemy.MP, bEnemy.MPMax);
                enemy.GetComponentInChildren<EnemyDeath>(true).SetID(index);
                
                // Proceed to the next enemy
                index++;
            }
        }
    }
    #endregion
}
