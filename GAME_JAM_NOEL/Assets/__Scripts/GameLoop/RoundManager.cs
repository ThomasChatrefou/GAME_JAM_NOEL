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

        {
            while (enemyInvoke < enemyInWave)
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
}
