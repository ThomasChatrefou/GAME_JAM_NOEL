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
    [SerializeField] private Vector2 topLeft;
    [SerializeField] private Vector2 bottomRight;
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
    private IEnumerator SpawnEnemy()
    {
       while(enemyInvoke<enemyInWave)
        {
            int indexEnemy = Random.Range(0, 3);
            int indexSpawn = Random.Range(0, 5);

            // Change "TypeOfEnemy" by a table where the different type of ennemies are stocked ; Change "PositionOfSpawner" by a table where the position of spawner are stocked
            //Instantiate(TypeOfEnemy[indexEnemy], PositionOfSpawner[spawn], new Quaternion()); 

            enemyInvoke++;

            yield return new WaitForSeconds(0.2f); // Change value for spawning ennemies more and low fast
        }
    }
}