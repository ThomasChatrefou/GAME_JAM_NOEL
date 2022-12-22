using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject beforeLaunch;
    [Space]
    [SerializeField] private GameObject gameLaunch;
    [SerializeField] private GameObject roundLaunch;
    [Space]
    [SerializeField] private GameObject victoriousGameEnd;
    [SerializeField] private GameObject loserGameEnd;

    [Header("Settings")]
    [SerializeField] private float gameLaunchShowTime;
    [SerializeField] private float roundLaunchShowTime;
    [SerializeField] private float gameEndShowTime;

    public void ShowBeforeLaunch()
    {
        beforeLaunch.SetActive(true);
    }

    public void ShowGameLaunch()
    {
        beforeLaunch.SetActive(false);
        gameLaunch.SetActive(true);
    }

    public void HideGameLaunch()
    {
        gameLaunch.SetActive(false);
    }

    public void ShowRoundLaunch()
    {
        roundLaunch.SetActive(true);
        Invoke("HideRoundLaunch", roundLaunchShowTime);
    }

    private void HideRoundLaunch()
    {
        roundLaunch.SetActive(false);
    }

    public void ShowGameEnd(bool victory)
    {
        if(victory)
            victoriousGameEnd.SetActive(true);
        else
            loserGameEnd.SetActive(true);

        Invoke("HideEndGame", roundLaunchShowTime);
    }

    private void HideEndGame()
    {
        victoriousGameEnd.SetActive(false);
        loserGameEnd.SetActive(false);

        ShowBeforeLaunch();
    }
}
