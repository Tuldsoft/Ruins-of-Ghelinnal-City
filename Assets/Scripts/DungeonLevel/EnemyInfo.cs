using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A container class for dungeon enemies
public class EnemyInfo
{
    public int ID { get; set; }
    public EnemyName EnemyName { get; set; }
    public Vector2 Position { get; set; }
    public bool IsDead { get; set; }

}
