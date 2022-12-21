using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class UserInterfaceManager : MonoBehaviour
{
    static public UserInterfaceManager Instance;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button joinButton;

    [Header("Lobby Selection Menu")]
    [SerializeField] private GameObject lobbySelectionCanvas;
    [SerializeField] private TMP_Text playerNameDisplay;
    [SerializeField] private Transform lobbiesContainer;

    [Header("Lobby Selection Options Panel")]
    [SerializeField] private GameObject lobbySelectionOptionsCanvas;
    [SerializeField] private Button refreshLobbyListButton;
    [SerializeField] private Button openLobbyCreationButton;
    [SerializeField] private Button backToMenuButton;

    [Header("Lobby Creation panel")]
    [SerializeField] private GameObject lobbyCreationCanvas;
    [SerializeField] private TMP_InputField createLobbyNameInput;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button cancelCreateLobbyButton;

    [Header("Joined Lobby Menu")]
    [SerializeField] private GameObject lobbyMenuCanvas;
    [SerializeField] private TMP_Text lobbyNameDisplay;
    [SerializeField] private TMP_Text membersCountDisplay;
    [SerializeField] private Transform membersContainer;
    [SerializeField] private Button refreshMemberListButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backToLobbySelectionButton;

    [Header("Container Prefabs")]
    [SerializeField] private GameObject lobbyItemPrefab;
    [SerializeField] private GameObject memberItemPrefab;

    [Header("HUD")]
    [SerializeField] private GameObject inGameHUDCanvas;
    [SerializeField] private Button launchButton;

    private List<GameObject> LobbyList = new List<GameObject>();
    private List<GameObject> MemberList = new List<GameObject>();

    [HideInInspector] public ButtonClickedEvent onClick;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;

        joinButton.interactable = false;
        createLobbyButton.interactable = false;

        playerNameInput.onValueChanged.AddListener(OnEditPlayerName);
        createLobbyNameInput.onValueChanged.AddListener(OnEditCreateLobbyName);

        joinButton.onClick.AddListener(OnJoin);
        openLobbyCreationButton.onClick.AddListener(OnOpenLobbyCreation);
        backToMenuButton.onClick.AddListener(OnBackToMenu);
        createLobbyButton.onClick.AddListener(OnCreateLobby);
        cancelCreateLobbyButton.onClick.AddListener(OnHideLobbyCreation);
        startGameButton.onClick.AddListener(OnStartGame);
        backToLobbySelectionButton.onClick.AddListener(OnBackToLobbySelection);
        refreshLobbyListButton.onClick.AddListener(OnRefreshLobbyList);
        refreshMemberListButton.onClick.AddListener(OnRefreshMemberList);

        AutofillLastPickedNameForInputField(playerNameInput, "NickName");
        AutofillLastPickedNameForInputField(createLobbyNameInput, "LobbyName");

        launchButton.onClick = onClick;
        launchButton.onClick.AddListener(OnLaunchClicked);
    }

    private void OnLaunchClicked()
    {
        print("LAUNCH");
    }

    public void AutofillLastPickedNameForInputField(TMP_InputField inPlayerField, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            if (PlayerPrefs.GetString(key) != "")
            {
                inPlayerField.text = PlayerPrefs.GetString(key);
            }
        }
    }

    private void OnEditPlayerName(string inPlayerName)
    {
        if (inPlayerName.Length == 0 || inPlayerName == null)
        {
            joinButton.interactable = false;
            return;
        }

        PlayerPrefs.SetString("NickName", inPlayerName);
        playerNameDisplay.text = inPlayerName;
        LobbyManager.Instance.SetPlayerName(inPlayerName);
        joinButton.interactable = true;
    }

    private void OnJoin()
    {
        mainMenuCanvas.SetActive(false);
        lobbySelectionCanvas.SetActive(true);
        ClearLobbyList();
        LobbyManager.Instance.SignIn();
    }

    public void AddLobbyItemButton(string lobbyName, int maxPlayers, int availableSlots, string inLobbyId)
    {
        GameObject newLobby = Instantiate(lobbyItemPrefab, lobbiesContainer);
        LobbyList.Add(newLobby);
        UILobbyButtonItem newLobbyButton = newLobby.GetComponent<UILobbyButtonItem>();
        if (newLobbyButton != null)
        {
            newLobbyButton.onClick.AddListener(OnJoinLobby);
            newLobbyButton.SetLobby(lobbyName, maxPlayers, maxPlayers - availableSlots, inLobbyId);
        }
    }

    public void ClearLobbyList()
    {
        foreach (GameObject lobby in LobbyList)
        {
            Destroy(lobby);
        }
        LobbyList.Clear();
    }

    private void OnOpenLobbyCreation()
    {
        lobbySelectionOptionsCanvas.SetActive(false);
        lobbyCreationCanvas.SetActive(true);
    }

    public void OnEditCreateLobbyName(string inLobbyName)
    {
        if (inLobbyName.Length == 0 || inLobbyName == null)
        {
            createLobbyButton.interactable = false;
            return;
        }

        PlayerPrefs.SetString("LobbyName", inLobbyName);
        LobbyManager.Instance.SetLobbyToCreateName(inLobbyName);
        lobbyNameDisplay.text = inLobbyName;
        createLobbyButton.interactable = true;
    }

    private void OnCreateLobby()
    {
        LobbyManager.Instance.CreateLobby();
        OnHideLobbyCreation();

        lobbySelectionCanvas.SetActive(false);
        lobbyMenuCanvas.SetActive(true);
        ClearMemberList();
        startGameButton.interactable = true;
    }

    private void OnHideLobbyCreation()
    {
        lobbyCreationCanvas.SetActive(false);
        lobbySelectionOptionsCanvas.SetActive(true);
    }

    private void OnBackToMenu()
    {
        LobbyManager.Instance.SignOut();

        lobbySelectionCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void OnJoinLobby()
    {
        print("join lobby UIManager callback");
        lobbySelectionCanvas.SetActive(false);
        lobbyMenuCanvas.SetActive(true);
        ClearMemberList();
    }

    public void AddMemberItem(string newMemberName)
    {
        GameObject newMember = Instantiate(memberItemPrefab, membersContainer);
        MemberList.Add(newMember);
        TMP_Text newMemberText = newMember.transform.GetChild(0).GetComponent<TMP_Text>();
        newMemberText.text = newMemberName;
    }

    public void ClearMemberList()
    {
        foreach (GameObject member in MemberList)
        {
            Destroy(member);
        }
        MemberList.Clear();
    }

    public void UpdateLobbyInfos(string name, int maxPlayers, int playersCount)
    {
        lobbyNameDisplay.text = name;
        membersCountDisplay.text = playersCount.ToString() + "/" + maxPlayers.ToString();
    }

    private void OnStartGame()
    {
        startGameButton.interactable = false;
        lobbyMenuCanvas.SetActive(false);
        inGameHUDCanvas.SetActive(true);

        LobbyManager.Instance.StartGame();
        // add some code to start game => unity relay
    }

    private void OnBackToLobbySelection()
    {
        startGameButton.interactable = false;
        lobbyMenuCanvas.SetActive(false);
        lobbySelectionCanvas.SetActive(true);
        ClearLobbyList();
        LobbyManager.Instance.LeaveLobby();
    }

    private void OnRefreshLobbyList()
    {
        ClearLobbyList();
        LobbyManager.Instance.ListLobbies();
    }

    private void OnRefreshMemberList()
    {
        ClearMemberList();
        LobbyManager.Instance.ListPlayers();
    }

    public void OnHostJoiningGame()
    {
        lobbyMenuCanvas.SetActive(false);
        inGameHUDCanvas.SetActive(true);
    }
}