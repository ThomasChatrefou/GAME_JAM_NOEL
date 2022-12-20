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
    //[SerializeField] private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Local Test Variables")]
    [SerializeField] private bool isRunning;

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

        //isRunning.OnValueChanged += GameOnUpdate;
        isRunningupdate.AddListener(GameOnUpdate);
    }
    
    #region ServerRpc
    //[ServerRpc]
    //public void LaunchGameServerRpc()
    public void LaunchGame()
    {
        /* Preparation */
        currentRoundIndex = 0;
        currentRound = gameMode.rounds[0];

        //lacks smth but can't remember what

        //foreach (PlayerController playerController in players)
        //    playerController.OnPlayerDeath.AddListener(CheckGameState);

        /* Actual Launching */
        isRunning = true;
        isRunningupdate.Invoke();
        TriggerOnNewRoundStart();
    }
    
    #endregion

    #region EventRpcTriggers

    //[ClientRpc]
    //private void TriggerOnNewRoundStartClientRpc()
    private void TriggerOnNewRoundStart()
    {
        OnNewRoundStart.Invoke();
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
            OnEndGame.Invoke();
    }

    #endregion

    #region ServerOnly

    private void Update()
    {
        //if (!IsServer)
        //    return;

        if(!isRunning)
            return;

        timeSinceRoundStart += Time.deltaTime;

        if (!currentRound)
        {
            if(currentRoundIndex==0)
                currentRound = gameMode.rounds[0];
            else
                CheckGameState();
        }

        if (timeSinceRoundStart >= currentRound.Duration)
        {
            Invoke("NextRound", gameMode.timeBetweenRounds);
        }
        else
        {
            Debug.Log("still in the round");
        }
    }

    private void NextRound()
    {
        if (currentRoundIndex + 1 != gameMode.rounds.Length)
        {
            currentRoundIndex++;
            currentRound = gameMode.rounds[currentRoundIndex];
            timeSinceRoundStart = 0f;
            TriggerOnNewRoundStart();
        }
        else
            EndGame();
    }

    private void EndGame()
    {
        isRunning = false;
        isRunningupdate.Invoke();
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
