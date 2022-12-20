using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
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
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backToLobbySelectionButton;

    [Header("Container Prefabs")]
    [SerializeField] private GameObject lobbyItemPrefab;
    [SerializeField] private GameObject memberItemPrefab;

    private List<UILobbyButtonItem> activeLobbyButtons = new List<UILobbyButtonItem>();

    // [TEMP] should be better handled
    private string currentPlayerName;
    private string lobbyNameToCreate;
    private int lobbyMaxPlayers = 4;

    private void Awake()
    {
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

        AutofillLastPickedNameForInputField(playerNameInput, "NickName");
        AutofillLastPickedNameForInputField(createLobbyNameInput, "LobbyName");
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

    public void AddLobbyItemButton()
    {
        GameObject newLobby = Instantiate(lobbyItemPrefab, lobbiesContainer);
        UILobbyButtonItem newLobbyButton = newLobby.GetComponent<UILobbyButtonItem>();
        if (newLobbyButton != null)
        {
            newLobbyButton.onClick.AddListener(OnJoinLobby);
            newLobbyButton.SetLobby(lobbyNameToCreate, lobbyMaxPlayers, 0);
            activeLobbyButtons.Add(newLobbyButton);
        }
    }

    public void OnJoinLobby()
    {
        print("join lobby");
        lobbySelectionCanvas.SetActive(false);
        lobbyMenuCanvas.SetActive(true);

        // only for host
        startGameButton.interactable = true;

        GameObject newPlayer = Instantiate(memberItemPrefab, membersContainer);
        TMP_Text newPlayerText = newPlayer.transform.GetChild(0).GetComponent<TMP_Text>();
        newPlayerText.text = currentPlayerName;
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
        currentPlayerName = inPlayerName;
        joinButton.interactable = true;
    }

    private void OnJoin()
    {
        mainMenuCanvas.SetActive(false);
        lobbySelectionCanvas.SetActive(true);
    }

    private void OnOpenLobbyCreation()
    {
        lobbySelectionOptionsCanvas.SetActive(false);
        lobbyCreationCanvas.SetActive(true);
    }

    private void OnBackToMenu()
    {
        lobbySelectionCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    private void OnCreateLobby()
    {
        // [TEMP] for tests
        AddLobbyItemButton();
        // OnJoinLobby();
        OnHideLobbyCreation();
    }

    private void OnHideLobbyCreation()
    {
        lobbyCreationCanvas.SetActive(false);
        lobbySelectionOptionsCanvas.SetActive(true);
    }

    public void OnEditCreateLobbyName(string inLobbyName)
    {
        if (inLobbyName.Length == 0 || inLobbyName == null)
        {
            createLobbyButton.interactable = false;
            return;
        }
        
        PlayerPrefs.SetString("LobbyName", inLobbyName);
        lobbyNameToCreate = inLobbyName;
        lobbyNameDisplay.text = inLobbyName;
        createLobbyButton.interactable = true;
    }

    private void OnStartGame()
    {
        startGameButton.interactable = false;
        lobbyMenuCanvas.SetActive(false);
    }

    private void OnBackToLobbySelection()
    {
        lobbyMenuCanvas.SetActive(false);
        lobbySelectionCanvas.SetActive(true);
    }
}