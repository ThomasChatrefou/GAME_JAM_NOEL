using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
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
    [SerializeField] private List<CompilatedSpawnParams> tests;

    [Space]
    [SerializeField] private bool canSpawn;
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
        tests = new List<CompilatedSpawnParams>();
        StartCoroutine(LaunchRound(gameMode.rounds[0]));
    }

    /*private void LaunchRound(GameRound round)
    {
        OnRoundStart.Invoke();
        currentRound = round;
        
        spawns = currentRound.Spawn;

        foreach (SpawnParams _param in spawns)
            tests.Add(new TestStruct(currentRound.Duration, _param));

        timeSinceRoundStart = 0f;
    }

    public struct TestStruct
    {
        private SpawnParams _param;

        private float start;
        private float stop;
        private float totalDurationInSec;
        private int AmountOverTime;

        private bool hasStarted;
        private float currentTime;
        private float currentPeriodTime;

        public GameObject prefab => _param.EnemyPrefab;
        public Vector3 Pos(Vector2 topLeft, Vector2 bottomRight) => _param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight);

        public TestStruct(float _totalDurationInSec, SpawnParams _param)
        {
            hasStarted = false;
            currentTime = 0f;
            currentPeriodTime = 0f;
            totalDurationInSec = _totalDurationInSec;

            _param = _param;
            start = _param.ActivationTiming * totalDurationInSec;
            stop  = _param.StopTiming       * totalDurationInSec;
            
            AmountOverTime = (int)(_param.Amount / ((stop - start) * totalDurationInSec / _param.Period));

            Debug.Log($"amount for each period : {AmountOverTime}");
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

            if (currentPeriodTime < _param.Period)
                return 0;

            currentPeriodTime -= _param.Period;
            return AmountOverTime;

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
        timeSinceRoundStart = 0f;
        currentRound = round;
        spawns = currentRound.Spawn;

        GameObject roundGO = new GameObject()
        {
            name = round.name,
            transform = { parent = enemiesParentTransform }
        };

        foreach (SpawnParams param in spawns)
            StartCoroutine
            (
                LaunchEnemiesSpawning
                (
                    currentRound.Duration,
                    param,
                    roundGO.transform
                )
            );

        yield return RoundEnd(currentRound.Duration+gameMode.timeBetweenRounds);
    }

    private void NextRound()
    {
        if (currentRoundIndex + 1 < gameMode.rounds.Length)
            StartCoroutine(LaunchRound(gameMode.rounds[++currentRoundIndex]));
        else
        {
            currentRound = null;
            GameManager.Instance.EndGame();
        }
    }

    private IEnumerator RoundEnd(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if(gameManager.IsRunning)
            OnRoundEnd.Invoke();
    }

    private IEnumerator LaunchEnemiesSpawning(float totalDurationInSec, SpawnParams _param, Transform roundTransform)
    {
        float timeInPercentage = _param.StopTiming - _param.ActivationTiming;
        float timeInSec = timeInPercentage * totalDurationInSec;
        float amountOverTime = _param.Amount * 1f / timeInSec;
        Debug.Log($"{_param.EnemyPrefab.name} : timeInPercentage={timeInPercentage}, timeInSec={timeInSec}");
        
        yield return new WaitForSeconds(_param.ActivationTiming * totalDurationInSec - timeSinceRoundStart);

        canSpawn = true;

        tests.Add(new CompilatedSpawnParams()
            {
                param = _param,
                parentTransform = new GameObject
                {
                    name = _param.EnemyPrefab.name,
                    transform = { parent = roundTransform }
                }.transform,
                AmountOverTime = amountOverTime,
                AmountToSpawn = 0f,
                currentTime = 0f,
                spawnTimeLeft = timeInSec
            }
        );

        /*yield return null;                                                                                    

        int index = 0;
        float timeSinceSpawnStart = 0f;
        
        do
        {
            amount += AmountOverTime;
            while(amount>=1)
            {
                amount--;
                Instantiate(_param.EnemyPrefab,
                     _param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight),
                            Quaternion.identity,
                            round.transform).name += $" {++index}";
            }

            float time = Time.deltaTime;
            yield return new WaitForSeconds(_param.Period-time);
            timeSinceSpawnStart += time;
            round.name = $"{currentRound.name} {stop-timeSinceSpawnStart}";
        } while (timeSinceSpawnStart <= stop);*/
    }

    private void Update()
    {
        //if(!IsServer)
        //    return;

        float time = Time.deltaTime;
        timeSinceRoundStart += time;

        if(tests==null || tests.Count == 0)
            return;

        for(int i=0; i<tests.Count; i++)
        {
            CompilatedSpawnParams paramSet = tests[i];;

            if (paramSet.spawnTimeLeft < 0f)
                continue;

            paramSet.currentTime += time;
            paramSet.spawnTimeLeft -= time;

            paramSet.AmountToSpawn+=time*paramSet.AmountOverTime;

            if (paramSet.currentTime <= paramSet.period)
            {
                tests[i] = paramSet;
                continue;
            }

            Debug.Log($"Should spawn {tests[i].prefab.name} x {(int)paramSet.AmountToSpawn}");
            StartCoroutine(Spawn((int)paramSet.AmountToSpawn, tests[i]));
            paramSet.AmountToSpawn -= 1f*(int)paramSet.AmountToSpawn;
            paramSet.currentTime -= paramSet.period;

            tests[i] = paramSet;
            /*while (paramSet.AmountToSpawn >= 1)
            {
                Instantiate(paramSet.prefab,
                    paramSet.Pos,
                    Quaternion.identity,
                    paramSet.parentTransform);
                paramSet.AmountToSpawn--;
            }*/
        }

        foreach (var paramSet in tests)
            if (paramSet.spawnTimeLeft > 0)
                return;

        foreach (var paramSet in tests)
            if (paramSet.AmountToSpawn >= 0)
                StartCoroutine(Spawn((int)paramSet.AmountToSpawn, paramSet));

        tests.Clear();
    }

    private IEnumerator Spawn(int amount, CompilatedSpawnParams param)
    {
        while (amount>0)
        {
            Instantiate(param.prefab, param.param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight), Quaternion.identity, param.parentTransform);
            amount--;
            yield return null;
        }
    }

    public IEnumerator Activate(SpawnParams spawnParams)
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

[Serializable]
public struct CompilatedSpawnParams
{
    public SpawnParams param;
    public GameObject prefab => param.EnemyPrefab;
    public float period => param.Period;

    public Transform parentTransform;
    public float spawnTimeLeft;
    public float AmountOverTime;
    public float AmountToSpawn;
    public float currentTime;
}

