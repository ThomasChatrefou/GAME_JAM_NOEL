using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn", menuName = "ScriptableObjects/GameLoop/EnemySpawn")]
public class EnemySpawn : ScriptableObject
{
    //public Enemy EnemyPrefab;
    public int MinEnemySpawn;
    public int MaxEnemySpawn;
    public AnimationCurve SpawnCurve;
    public SpawnZone SpawnZone;
}

[Serializable]
public struct SpawnZone
{
    [SerializeField] private Vector2 topLeft;
    [SerializeField] private Vector2 bottomRight;
}