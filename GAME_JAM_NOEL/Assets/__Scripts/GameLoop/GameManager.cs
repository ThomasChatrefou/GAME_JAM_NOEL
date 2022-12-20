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

    //[Header("Synchronized Variables")]
    //[SerializeField] private NetworkVariable<bool> isRunning = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Local Test Variables")]
    [SerializeField] private bool isRunning;

    [Header("Client Events")]
    public UnityEvent OnLaunchGame;
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
        roundManager.Init(gameMode);
        roundManager.OnRoundStart.AddListener(TriggerOnNewRoundStart);

        //lacks smth but can't remember what

        //foreach (PlayerController playerController in players)
        //    playerController.OnPlayerDeath.AddListener(CheckGameState);

        /* Actual Launching */
        isRunning = true;
        isRunningupdate.Invoke();
        TriggerOnNewRoundStart();
        roundManager.LaunchRound(gameMode.rounds[0]);
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
    public void EndGame()
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
