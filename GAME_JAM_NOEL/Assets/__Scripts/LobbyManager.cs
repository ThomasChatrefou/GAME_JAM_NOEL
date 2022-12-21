using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    static public LobbyManager Instance;

    // for now, we don't want to modify this value in game
    static public int MaxPlayers = 4;

    private Lobby joinedLobby;
    private Lobby hostLobby;

    private float heartbeatTimer;

    private string playerName;
    private string lobbyToCreateName;

    [SerializeField] private string relayCodeKey;

    private float lobbyPollTimer = 0f;
    [SerializeField] private float lobbyPollingFrequency = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (joinedLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                lobbyPollTimer = lobbyPollingFrequency;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                if (joinedLobby.Data[relayCodeKey].Value != "0")
                {
                    if (!IsLobbyHost())
                    {
                        TestRelay.Instance.JoinRelay(joinedLobby.Data[relayCodeKey].Value);
                        UserInterfaceManager.Instance.OnHostJoiningGame();
                    }

                    joinedLobby = null;
                }
            }
        }
    }

    public async void SignIn()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log(playerName);

        ListLobbies();
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        print("Signed out");
    }

    public async void ListLobbies()
    {
        print("LISTING LOBBY");
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                UserInterfaceManager.Instance.AddLobbyItemButton(lobby.Name, lobby.MaxPlayers, lobby.AvailableSlots, lobby.Id);
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.AvailableSlots + " " + lobby.Id);
            }
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateLobby()
    {
        print("CREATING LOBBY");
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { relayCodeKey, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyToCreateName, MaxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            ListPlayers(hostLobby);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        Debug.Log("GET PLAYER " + playerName);
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };

    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    public async void JoinLobbyById(string lobbyId)
    {
        print("JOINING LOBBY BY ID");
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };

            Lobby lobbyToJoin = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            joinedLobby = lobbyToJoin;
            ListPlayers(joinedLobby);
            PrintPlayers(joinedLobby);
            Debug.Log("Joined Lobby with id " + lobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    public async void JoinLobbyByCode(string lobbyCode)
    {
        print("JOINING LOBBY");
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby lobbyToJoin = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobbyToJoin;
            ListPlayers(joinedLobby);
            PrintPlayers(joinedLobby);
            Debug.Log("Joined Lobby with code " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void ListPlayers()
    {
        ListPlayers(joinedLobby);
    }

    private void ListPlayers(Lobby lobby)
    {
        Debug.Log("LIST PLAYERS IN LOBBY " + lobby.Name);
        UserInterfaceManager.Instance.UpdateLobbyInfos(lobby.Name, lobby.MaxPlayers, lobby.Players.Count);
        foreach (Player player in lobby.Players)
        {
            UserInterfaceManager.Instance.AddMemberItem(player.Data["PlayerName"].Value);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            print("LEAVING LOBBY");
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                hostLobby = null;
                joinedLobby = null;
                ListLobbies();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void StartGame()
    {
        if (joinedLobby != null)
        {
            print("START GAME");
            try
            {
                string relayCode = await TestRelay.Instance.CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { relayCodeKey, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return hostLobby != null;
    }

    // Setters / Getters
    public void SetPlayerName(string inPlayerName)
    {
        playerName = inPlayerName;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetLobbyToCreateName(string inLobbyName)
    {
        lobbyToCreateName = inLobbyName;
    }

    public string GetLobbyToCreateName()
    {
        return lobbyToCreateName;
    }
}
