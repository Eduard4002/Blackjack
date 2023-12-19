using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    public Toggle toggleOnePlayer;
    public Toggle toggleTwoPlayers;
    public Toggle toggleThreePlayers;
    public Color selectedColor; // Color for a selected toggle
    public Color normalColor; // Color for a normal (unselected) toggle

    private void Start()
    {
        //Default to 1 player
        TogglePlayerCountChanged(1);
    }

    public void TogglePlayerCountChanged(int playerCount)
    {
        if (playerCount == 1)
        {
            SetToggleColors(toggleOnePlayer, true);
            SetToggleColors(toggleTwoPlayers, false);
            SetToggleColors(toggleThreePlayers, false);
        }
        else if (playerCount == 2)
        {
            SetToggleColors(toggleOnePlayer, false);
            SetToggleColors(toggleTwoPlayers, true);
            SetToggleColors(toggleThreePlayers, false);
        }
        else if (playerCount == 3)
        {
            SetToggleColors(toggleOnePlayer, false);
            SetToggleColors(toggleTwoPlayers, false);
            SetToggleColors(toggleThreePlayers, true);
        }

        PlayerPrefs.SetInt("PlayerCount", playerCount);
    }

    private void SetToggleColors(Toggle toggle, bool isSelected)
    {
        Color currColor = isSelected ? selectedColor : normalColor;
        toggle.GetComponentInChildren<Image>().color = currColor;
    }

    public void OnPlayButtonClicked()
    {
        // Load the game scene
        SceneManager.LoadScene("Game");

    }

    public void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();
    }
}
