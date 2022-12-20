using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerController[] players;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private GameRound currentRound;

    //[Header("Synchronized Variables")]
    //[SerializeField] private NetworkVariable<int> currentRoundIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    [Header("Local Test Variables")]
    [SerializeField] private bool isRunning;
    [SerializeField] private UnityEvent isRunningupdate;

    [Header("Current Game")]
    [SerializeField] private float timeSinceRoundStart;
    [SerializeField] private int currentRoundIndex;

    [Header("Client Events")]
    public UnityEvent OnLaunchGame;
    public UnityEvent OnNewRoundStart;
    public UnityEvent OnEndGame;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //isRunningupdate.AddListener(GameOnUpdate);
    }
    

    #region ServerRpc
    //[ServerRpc]
    //public void LaunchGameServerRpc()
    public void LaunchGame()
    {
        /* Preparation */
        currentRoundIndex = 0;
        TriggerOnNewRoundStart();

        

        foreach (PlayerController playerController in players)
            playerController.OnPlayerDeath.AddListener(CheckGameState);

        /* Actual Launching */
        isRunning = true;
        isRunningupdate.Invoke();
        //OnLaunchGame.Invoke();
    }



    #endregion

    #region EventRpcTriggers
    //[ClientRpc]
    //private void TriggerOnGameOnUpdateClientRpc()
    private void TriggerOnGameOnUpdate()
    {
        if (isRunning)
            OnLaunchGame.Invoke();
        else
            OnEndGame.Invoke();
    }

    //[ClientRpc]
    //private void TriggerOnNewRoundStartClientRpc()
    private void TriggerOnNewRoundStart()
    {
        OnNewRoundStart.Invoke();
    }

    #endregion

    #region ServerOnly
    private void GameRoundUpdate()
    {
        currentRound = gameMode.rounds[currentRoundIndex];

        timeSinceRoundStart = 0f;
        OnNewRoundStart.Invoke();
    }

    private void Update()
    {
        //if (!IsServer)
        //    return;

        timeSinceRoundStart += Time.deltaTime;

        if (timeSinceRoundStart >= currentRound.Duration)
        {
            Invoke("NextRound", gameMode.timeBetweenRounds);
        }
        else
        {
            
        }
    }

    private void NextRound()
    {
        if (gameMode.rounds.Length > currentRoundIndex+1)
        {
            currentRoundIndex++;
            TriggerOnNewRoundStart();
        }
        else
            EndGame();
    }

    private void EndGame()
    {
        isRunning = false;
        //TriggerOnGameEnd();
    }
    
    private void CheckGameState()
    {
        Debug.LogWarning("Need to Check if any Player is still alive !");

        /*
        foreach (PlayerController player in players)
        {
            if(player.IsAlive)
                return;
        }*/

        EndGame();
    }

    #endregion
}
