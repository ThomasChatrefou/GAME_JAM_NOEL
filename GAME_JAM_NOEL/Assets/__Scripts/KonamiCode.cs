using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class KonamiCode : MonoBehaviour
{
    private const float WaitTime = 1f;
    public Text successText;
    public bool success;

    public GameObject playerCheat;
    

    private NetworkManager networkManager;
    private GameObject playerKonamiCode;

    private KeyCode[] keys = new KeyCode[]
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    


    IEnumerator Code()
    {
        float timer = 0f;
        int index = 0;

        while (true)
        {
            if (Input.GetKeyDown(keys[index]))
            {

                Debug.Log(keys[index]);                
                index++;
                

                if (index == keys.Length)
                {
                    Debug.Log("Konami code réussi !");
                    success = true;
                    timer = 0f;
                    index = 0;
                }
                else
                {
                    timer = WaitTime;
                }
            }
            else if (Input.anyKeyDown)
            {
                
                Debug.Log("Mauvaise touche, Konami Code cassé");
                timer = 0;
                index = 0;
            }

            
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    Debug.Log("Trop lent pour le konami code :/");
                    timer = 0;
                    index = 0;
                }
            }
            

            yield return null;
        }
    }


    void Update()
    {

        StartCoroutine(Code());


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
}
