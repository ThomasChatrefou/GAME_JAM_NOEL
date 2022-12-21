using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RoundManager : NetworkBehaviour
{
    [Header("Refs")]
    private GameManager gameManager;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private Transform enemiesParentTransform;

    [Header("Round Settings")]
    [SerializeField] private GameRound currentRound;

    [Header("Round Lifetime Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;

    //RoundLife needs
    private int currentRoundIndex;
    private List<SpawnParams> spawns;
    private List<CompilatedSpawnParams> tests;
    private float timeSinceRoundStart;
    private Vector2 topLeft, bottomRight;
    
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
            
            StartCoroutine(Spawn((int)paramSet.AmountToSpawn, tests[i]));
            paramSet.AmountToSpawn -= 1f*(int)paramSet.AmountToSpawn;
            paramSet.currentTime -= paramSet.period;

            tests[i] = paramSet;
        }

        foreach (var paramSet in tests)
            if (paramSet.spawnTimeLeft > 0)
                return;

        foreach (var paramSet in tests)
            if (paramSet.AmountToSpawn >= 0)
                StartCoroutine(Spawn((int)paramSet.AmountToSpawn, paramSet));

        tests.Clear();
    }

    private IEnumerator Spawn(int totalAmount, CompilatedSpawnParams param)
    {
        int leftAmount = totalAmount;

        while (leftAmount > 0)
        {
            Instantiate(param.prefab, param.param.RandomPosInRectTopLeftToBottomRight(topLeft, bottomRight), Quaternion.identity, param.parentTransform);

            if(leftAmount <= totalAmount * param.param.WeaponRate)
                Debug.Log($"A new {param.prefab.name} will carry a Weapon !");
            
            leftAmount--;
            yield return null;
        }
    }
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

