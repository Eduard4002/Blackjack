using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenuManager : MonoBehaviour
{
    public TMP_InputField playerCountInputField; // Reference to the input field
    public void OnPlayButtonClicked()
    {
        int playerCount;
        if (int.TryParse(playerCountInputField.text, out playerCount) && playerCount > 0 && playerCount <= 3)
        {
            // Save the player count for use in the game scene
            PlayerPrefs.SetInt("PlayerCount", playerCount);

            // Load the game scene
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogError("Invalid player count. Please enter a number between 1 and 3.");
            // Optionally, display this error to the user through the UI.
        }
    }

    public void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();
    }
}
