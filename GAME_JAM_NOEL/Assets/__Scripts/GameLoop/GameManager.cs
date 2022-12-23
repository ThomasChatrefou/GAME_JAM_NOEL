using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{

    #region Singleton
    private static GameManager instance;
    public static GameManager Instance => instance;

    private void Start()
    {
        if (instance)
            Destroy(instance);

        instance = this;
    }
    #endregion

    [Header("Refs")]
    [SerializeField] private PlayerController[] players;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private RoundManager roundManager;
    public Transform topLeft, bottomRight;

    //[Header("Synchronized Variables")]
    //[SerializeField] private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Local Test Variables")]
    [SerializeField] private bool isRunning;

    public bool CheckForAlivePlayer()
    {
        bool atLeastOnePlayerAlive = false;
        foreach (PlayerController player in players)
            if (!player.IsDead)
                atLeastOnePlayerAlive = true;

        return atLeastOnePlayerAlive;
    }

    public void UpdateGameMode(GameMode _gameMode)
    {
        gameMode = _gameMode;
    }

    public bool IsRunning => isRunning;

    [Header("Client Events")]
    public UnityEvent OnLaunchGame;
    public UnityEvent<bool> OnEndGame;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //isRunning.OnValueChanged += GameOnUpdate;
        isRunningupdate.AddListener(GameOnUpdate);
    }
    
    #region ServerRpc
    //[ServerRpc]
    //public void LaunchGameServerRpc()
    public void LaunchGame()
    {
        /* Preparation */
        roundManager.Init(gameMode, this);

        //lacks smth but can't remember what

        players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController playerController in players)
            playerController.transform.parent = transform;

        /* Actual Launching */
        isRunning = true;
        isRunningupdate.Invoke();
        roundManager.Launch();

        StartCoroutine(WaitForGameEnd());
    }
    
    #endregion

    #region Simulation of sync var changes events
    [Header("A SUPPRIMER DES LA MISE EN SERVEUR")]
    public UnityEvent isRunningupdate;
    //private void GameOnUpdate(bool prev, bool newval)
    private void GameOnUpdate()
    {
        if (isRunning)
            OnLaunchGame.Invoke();
        else
            OnEndGame.Invoke(CheckForAlivePlayer());
    }

    #endregion

    #region ServerOnly
    public void EndGame()
    {
        isRunning = false;
        isRunningupdate.Invoke();
    }
    
    private IEnumerator WaitForGameEnd()
    {
        Debug.Log("launching End Condition Check");

        while (!EndConditionsMet())
        {
            Debug.Log("going through End Condition Check");
            yield return new WaitForSeconds(1f);
        }

        Debug.LogWarning("End Condition was met !");
        EndGame();
    }

    private bool EndConditionsMet()
    {
        if (roundManager.IsSpawning)
            return false;

        Debug.Log("done preparing and spawning");

        bool playersAlive = CheckForAlivePlayer();

        if (!CheckForAlivePlayer())
            return true;
        
        Debug.Log("not all players are dead");

        if (roundManager.AllEnemiesDead())
        {
            Debug.Log("all enemies are dead");
            return true;
        }

        Debug.Log("not all enemies are dead");
        return false;
    }

    #endregion

    public void SaySmtg(string message)
    {
        Debug.Log($"{message}");
    }
}
