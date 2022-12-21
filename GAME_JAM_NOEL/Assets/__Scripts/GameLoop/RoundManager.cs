using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

public class RoundManager : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private Transform enemiesParentTransform;

    [Header("Round Settings")]
    [SerializeField] private int currentRoundIndex;
    [SerializeField] private GameRound currentRound;
    [SerializeField] private List<SpawnParams> spawns;
    //[SerializeField] private List<TestStruct> tests;

    [Space]
    [SerializeField] private float timeSinceRoundStart;
    [SerializeField] private Vector2 topLeft, bottomRight;

    [Header("Round Lifetime Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    
    public void Init(GameMode _gameMode, GameManager _gameManager)
    {
        gameMode = _gameMode;
        OnRoundEnd.AddListener(NextRound);
        gameManager = _gameManager;
        topLeft     = new Vector2(gameManager.topLeft.position.x, gameManager.topLeft.position.y);
        bottomRight = new Vector2(gameManager.bottomRight.position.x, gameManager.bottomRight.position.y);
        currentRoundIndex = 0;
    }

    public void Launch()
    {
        //tests = new List<TestStruct>();
        StartCoroutine(LaunchRound(gameMode.rounds[0]));
    }

    /*private void LaunchRound(GameRound round)
    {
        OnRoundStart.Invoke();
        currentRound = round;
        
        spawns = currentRound.Spawn;

        foreach (SpawnParams param in spawns)
            tests.Add(new TestStruct(currentRound.Duration, param));

        timeSinceRoundStart = 0f;
    }

    public struct TestStruct
    {
        private SpawnParams param;

        private float start;
        private float stop;
        private float totalDurationInSec;
        private int amountEachPeriod;

        private bool hasStarted;
        private float currentTime;
        private float currentPeriodTime;

        public GameObject prefab => param.EnemyPrefab;
        public Vector3 Pos(Vector2 topLeft, Vector2 bottomRight) => param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight);

        public TestStruct(float _totalDurationInSec, SpawnParams _param)
        {
            hasStarted = false;
            currentTime = 0f;
            currentPeriodTime = 0f;
            totalDurationInSec = _totalDurationInSec;

            param = _param;
            start = param.ActivationTiming * totalDurationInSec;
            stop  = param.StopTiming       * totalDurationInSec;
            
            amountEachPeriod = (int)(param.Amount / ((stop - start) * totalDurationInSec / param.Period));

            Debug.Log($"amount for each period : {amountEachPeriod}");
        }

        public int AddTime(float time)
        {
            currentTime += time;

            if (currentTime < start || currentTime > stop)
                return 0;

            if (!hasStarted)
            {
                currentPeriodTime = currentTime - start * totalDurationInSec;
                hasStarted = true;
            }
            else
                currentPeriodTime += time;

            if (currentPeriodTime < param.Period)
                return 0;

            currentPeriodTime -= param.Period;
            return amountEachPeriod;

        }
    }

    private void Update()
    {
        if (!gameManager.IsRunning || tests==null || tests.Count==0)
            return;

        float time = Time.deltaTime;
        timeSinceRoundStart += time;
        
        if(timeSinceRoundStart >= currentRound.Duration)
            NextRound();

        foreach (TestStruct test in tests)
        {
            int number = test.AddTime(time);
            Debug.Log(number);
            for (int i = 0; i < number; i++)
            {
                GameObject go = Instantiate(test.prefab, enemiesParentTransform);
                Vector3 pos = test.Pos(topLeft, bottomRight);
                Debug.Log(go.name);
                go.name += $" ({pos.x},{pos.y})";
                go.transform.position = pos;
            }
        }
    }*/

    private IEnumerator LaunchRound(GameRound round)
    {
        OnRoundStart.Invoke();
        currentRound = round;
        spawns = currentRound.Spawn;

        Coroutine c;

        foreach (SpawnParams param in spawns)
            StartCoroutine(LaunchEnemiesSpawning(currentRound.Duration, param));

        while (timeSinceRoundStart < currentRound.Duration)
        {
            timeSinceRoundStart += Time.deltaTime;
            enemiesParentTransform.name = $"Enemies {timeSinceRoundStart}";
            yield return null;
        }

        yield return RoundEnd(gameMode.timeBetweenRounds);
    }

    private void NextRound()
    {
        if (currentRoundIndex + 1 < gameMode.rounds.Length)
            StartCoroutine(LaunchRound(gameMode.rounds[++currentRoundIndex]));
        else
            GameManager.Instance.EndGame();
    }

    private IEnumerator RoundEnd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if(gameManager.IsRunning)
            OnRoundEnd.Invoke();
    }

    private IEnumerator LaunchEnemiesSpawning(float totalDurationInSec, SpawnParams param)
    {
        float start = param.ActivationTiming * totalDurationInSec;
        yield return new WaitForSeconds(start);
        Debug.Log($"{param.EnemyPrefab.name} {totalDurationInSec}");
        
        float stop  = param.StopTiming       * totalDurationInSec;

        float timeInPercentage = stop - start;
        float timeInSec = timeInPercentage * totalDurationInSec;
        float numberOfPeriods = timeInSec / param.Period;
        float amountEachPeriod = param.Amount * 1f / numberOfPeriods;

        float total = param.Amount;
        float nbSec = timeInSec;
        float attente = (nbSec/60f) / total;
        Debug.Log(attente);
        yield return null;

        float amount = 0f;

        GameObject round = new GameObject
        {
            name = currentRound.name,
            transform =
            {
                parent = enemiesParentTransform
            }
        };

        yield return null;

        int index = 0;
        float timeSinceSpawnStart = 0f;
        
        do
        {
            amount += amountEachPeriod;
            Debug.Log(amount);

            while (amount>=1)
            {
                amount--;
                Instantiate(param.EnemyPrefab,
                     param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight),
                            Quaternion.identity,
                            round.transform).name += $" {++index}";
            }

            float time = Time.deltaTime;
            yield return null;
            timeSinceSpawnStart += time;
            round.name = $"{currentRound.name} {stop-timeSinceSpawnStart}";
        } while (timeSinceSpawnStart <= stop);
    }

    /*private void Update()
    {
        //if(!IsServer)
        //    return;
        
        //timeSinceRoundStart += Time.deltaTime;

        foreach (var spawn in spawns)
        {
            if (timeSinceRoundStart >= spawn.ActivationTiming * )
            {
                StartCoroutine(Activate(spawn));
                spawns.Remove(spawn);
            }
        }
    }*/

    /*public IEnumerator Activate(SpawnParams spawnParams)
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
    public float timeBetweenEnemy = 0.2f;*/
}

