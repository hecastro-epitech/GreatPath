using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    static GameObject youDied;

    static GameObject youWon;

    void Start()
    {
        if (youDied == null)
        {
            youDied = youDied != null ? youDied : GameObject.Find("You Died");
            if (youDied != null) youDied.SetActive(false);
        }

        if (youWon == null)
        {
            youWon = youWon != null ? youWon : GameObject.Find("You Won");
            if (youWon != null) youWon.SetActive(false);
        }
    }

    static void SetPlayerMoviment(bool active)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; ++i)
        {
            players[i].GetComponent<PlayerControler>().allowMoviment = active;
        }
    }

    public void PauseGame()
    {
        SetPlayerMoviment(false);
        Time.timeScale--;
    }

    public void ResumeGame()
    {
        SetPlayerMoviment(true);
        Time.timeScale++;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public static void ActivateYouDied()
    {
        SetPlayerMoviment(false);
        youDied.SetActive(true);
    }

    public static void ActivateYouWon()
    {
        SetPlayerMoviment(false);
        youWon.SetActive(true);
    }

    public void ReloadLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
