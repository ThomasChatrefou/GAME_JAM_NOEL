using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

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
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform bottomRight;
}


// Class to spawn Enemy during the round
public class SpawnEnemyInRound : EnemySpawn
{
    public int enemyInWave;
    public int enemyInvoke;

    private void NumberEnemy()
    {
        enemyInWave = Random.Range(MinEnemySpawn, MaxEnemySpawn);
    }

    // Spawn the requested number of enemies
    
}