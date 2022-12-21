using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

[RequireComponent(typeof(Button))]
public class UILobbyButtonItem : MonoBehaviour
{

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text sizeText;

    private Button button;

    private string lobbyName;
    private int lobbySize;
    private int playerCount;

    private string lobbyCode;

    [HideInInspector] public ButtonClickedEvent onClick;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick = onClick;
        button.onClick.AddListener(OnJoinLobby);
    }

    private void OnJoinLobby()
    {
        if (lobbyCode == null) return;
        LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
    }

    public void SetLobby(string inName, int inMaxPLayers, int inPlayerCount, string inLobbyCode)
    {
        lobbyName = inName; ;
        lobbySize = inMaxPLayers;
        playerCount = inPlayerCount;
        nameText.text = inName;
        sizeText.text = inPlayerCount + "/" + inMaxPLayers;
        lobbyCode = inLobbyCode;
    }
}
