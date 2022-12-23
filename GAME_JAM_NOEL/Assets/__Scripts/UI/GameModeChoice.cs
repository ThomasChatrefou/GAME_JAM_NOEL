using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject))]
public class GameModeChoice : NetworkBehaviour
{
    [SerializeField] private GameMode[] options;
    [Space]
    [SerializeField] private Button prevButton;
    [SerializeField] private TextMeshProUGUI choiceName;
    [SerializeField] private Button nextButton;

    private int choice = 0;

    private void Start()
    {
        choiceName.text = "";
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsHost && !IsServer)
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            return;
        }

        prevButton.onClick.AddListener(PreviousChoice);
        nextButton.onClick.AddListener(NextChoice);
        OnUpdate();
    }

    private void PreviousChoice()
    {
        if (choice > 0)
            choice--;

        OnUpdate();
    }

    private void NextChoice()
    {
        if (choice+1 < options.Length)
            choice++;

        OnUpdate();
    }

    private void OnUpdate()
    {
        choiceName.text = options[choice].name;
        GameManager.Instance.UpdateGameMode(options[choice]);
    }
}
