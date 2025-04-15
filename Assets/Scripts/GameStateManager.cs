using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    public void OnWin()
    {
        player.SetActive(false);
        if(loseScreen.activeSelf) return;
        winScreen.SetActive(true);
    }
    public void OnLose()
    {
        player.SetActive(false);
        if(winScreen.activeSelf) return;
        loseScreen.SetActive(true);
    }
}
