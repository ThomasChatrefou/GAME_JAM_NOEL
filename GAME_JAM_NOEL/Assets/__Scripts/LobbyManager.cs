using System.Collections;
using System.Collections.Generic;
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

    private async void ListLobbies()
    {
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
                UserInterfaceManager.Instance.AddLobbyItemButton(lobby.Name, lobby.MaxPlayers, lobby.AvailableSlots, lobby.LobbyCode);
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateLobby()
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyToCreateName, MaxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            ListPlayers(hostLobby);

            //UserInterfaceManager.Instance.AddLobbyItemButton(lobby.Name, lobby.MaxPlayers, lobby.AvailableSlots, lobby.LobbyCode);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        Debug.Log(playerName);
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

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby lobbyToJoin = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobbyToJoin;
            ListPlayers(joinedLobby);

            Debug.Log("Joined Lobby with code " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void ListPlayers(Lobby lobby)
    {
        UserInterfaceManager.Instance.UpdateLobbyInfos(lobby.Name, lobby.MaxPlayers, lobby.Players.Count);
        foreach (Player player in lobby.Players)
        {
            UserInterfaceManager.Instance.AddMemberItem(player.Data["PlayerName"].Value);
        }
    }

    public async void LeaveLobby()
    {
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
