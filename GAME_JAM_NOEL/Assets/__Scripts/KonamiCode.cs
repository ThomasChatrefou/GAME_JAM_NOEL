using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.Netcode;

public class KonamiCode : MonoBehaviour
{

    
    public Text successText;
    public bool success;

    public GameObject playerCheat;

    private NetworkManager networkManager;
    private GameObject playerKonamiCode;

    private List<string> _keyStrokeHistory;

    void Awake()
    {
        _keyStrokeHistory = new List<string>();
    }

    void Update()
    {
        KeyCode keyPressed = DetectKeyPressed();
        AddKeyStrokeToHistory(keyPressed.ToString());
        Debug.Log("HISTORY: " + GetKeyStrokeHistory());
        if (GetKeyStrokeHistory().Equals("UpArrow,UpArrow,DownArrow,DownArrow,LeftArrow,RightArrow,LeftArrow,RightArrow,B,A"))
        {
            successText.text = "KONAMI CODE Réussi !!";
            ClearKeyStrokeHistory();
            success = true;
}

        if (success == true)
        {
            successText.gameObject.SetActive(true);

            playerKonamiCode = GameObject.FindGameObjectWithTag("NetworkManager");
            if (playerKonamiCode != null)
            {
                Debug.Log("playerKonamiCode :" + playerKonamiCode);
            }
            networkManager = playerKonamiCode.GetComponent<NetworkManager>();
            Debug.Log("networkManager :" + networkManager);
            networkManager.NetworkConfig.PlayerPrefab = playerCheat;
            success = false;
        }
    }

    private KeyCode DetectKeyPressed()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }

    private void AddKeyStrokeToHistory(string keyStroke)
    {
        if (!keyStroke.Equals("None"))
        {
            _keyStrokeHistory.Add(keyStroke);
            if (_keyStrokeHistory.Count > 10)
            {
                _keyStrokeHistory.RemoveAt(0);
            }
        }
    }

    private string GetKeyStrokeHistory()
    {
        return String.Join(",", _keyStrokeHistory.ToArray());
    }

    private void ClearKeyStrokeHistory()
    {
        _keyStrokeHistory.Clear();
    }


    
}
