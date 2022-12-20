using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.WSA;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private GameMode gameMode;

    [Header("Round Settings")]
    [SerializeField] private int currentRoundIndex;
    [SerializeField] private GameRound currentRound;
    [SerializeField] private List<Spawn> spawns;

    [Space]
    [SerializeField] private float timeSinceRoundStart;

    [Header("Round Lifetime Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;

    public void Init(GameMode _gameMode)
    {
        gameMode = _gameMode;
        OnRoundEnd.AddListener(AskForNextRound);
    }

    public void AskForNextRound()
    {
        StartCoroutine(CoroutineNommee());
    }

    private IEnumerator CoroutineNommee()
    {
        yield return new WaitForSeconds(gameMode.timeBetweenRounds);
        NextRound();
    }

    public void LaunchRound(GameRound round)
    {
        currentRound = round;

        spawns = currentRound.Spawn;
    }
    public void NextRound()
    {
        if (currentRoundIndex + 1 != gameMode.rounds.Length)
        {
            currentRoundIndex++;
            currentRound = gameMode.rounds[currentRoundIndex];
            timeSinceRoundStart = 0f;
            
        }
        else
            GameManager.Instance.EndGame();
    }

    private void Update()
    {
        //if(!IsServer)
        //    return;
        
        timeSinceRoundStart += Time.deltaTime;

        foreach (var spawn in spawns)
        {
            if (timeSinceRoundStart >= spawn.timeBeforeActivate)
            {
                StartCoroutine(Activate(spawn));
                spawns.Remove(spawn);
            }
        }
    }

    public IEnumerator Activate(Spawn spawn)
    {
        while (enemyInvoke < nbEnemiesInWave)
        {
            int indexEnemy = Random.Range(minEnemy, maxEnemy);
            int indexSpawn = Random.Range(0, 5);

            if(indexEnemy == 0 && nbTrashMob > 0){
                //Instantiate(TypeOfEnemy[0], PositionOfSpawner[spawn], new Quaternion());
                nbTrashMob--;
                enemyInvoke++;
                yield return new WaitForSeconds(timeBetweenEnemy); // Change value for spawning ennemies more and low fast
            }
            else if(indexEnemy == 1 && nbSaplin > 0){
                //Instantiate(TypeOfEnemy[1], PositionOfSpawner[spawn], new Quaternion());
                nbSaplin--;
                enemyInvoke++;
                yield return new WaitForSeconds(timeBetweenEnemy); // Change value for spawning ennemies more and low fast
            }
            else if(indexEnemy == 2 && nbSnowman > 0){
                //Instantiate(TypeOfEnemy[2], PositionOfSpawner[spawn], new Quaternion());
                nbSnowman--;
                enemyInvoke++;
                yield return new WaitForSeconds(timeBetweenEnemy); // Change value for spawning ennemies more and low fast
            }

            // Change "TypeOfEnemy" by a table where the different type of ennemies are stocked ; Change "PositionOfSpawner" by a table where the position of spawner are stocked
            //Instantiate(TypeOfEnemy[indexEnemy], PositionOfSpawner[spawn], new Quaternion()); 
        }
    }

    //Temporaire, à supprimer à la fin !

    public int nbEnemiesInWave = 150;   // La Wave dure 60sec
    public int nbTrashMob = 100;        // 30sec => 50 Enemies ; 45sec => 75 Enemies ; 60sec => 100 Enemies
    public int nbSaplin = 50;           // 30sec => 00 Enemies ; 45sec => 25 Enemies ; 60sec => 050 Enemies
    public int nbSnowman = 50;          // 30sec => 00 Enemies ; 45sec => 00 Enemies ; 60sec => 050 Enemies

    public int minEnemy = 0;
    public int maxEnemy = 1;

    public int enemyInvoke = 0;
    public float timeBetweenEnemy = 0.2f;
}
