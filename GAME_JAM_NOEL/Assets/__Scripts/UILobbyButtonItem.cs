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

    [HideInInspector] public ButtonClickedEvent onClick;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick = onClick;
    }

    public void SetLobby(string inName, int inMaxPLayers, int inPlayerCount)
    {
        lobbyName = inName; ;
        lobbySize = inMaxPLayers;
        playerCount = inPlayerCount;
        nameText.text = inName;
        sizeText.text = inPlayerCount + "/" + inMaxPLayers;
    }
}
