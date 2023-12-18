using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Slider betSlider;
    [Header("Texts")]
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI currentPlayerBetText;
    public TextMeshProUGUI playerFundsText;


    [Header("Buttons")]
    public GameObject placeBetButton;
    public GameObject hitButton;
    public GameObject standButton;
    public GameObject doubleDownButton;
    public GameObject splitButton;



    [Header("Panels")]
    public GameObject summaryPanel;
    public GameObject betPanel;
    public TextMeshProUGUI[] playerSummaryTexts; // Assuming 3 text objects
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    public void OnHitButtonPressed() => GameManager.Instance.PlayerHit();
    public void OnStandButtonPressed() => GameManager.Instance.PlayerStand();
    public void OnDoubleDownButtonPressed() => GameManager.Instance.PlayerDoubleDown();
    public void OnSplitButtonPressed() => GameManager.Instance.PlayerSplit();

    public void ShowEndRoundSummary(List<Player> players)
    {
        summaryPanel.SetActive(true);
        betPanel.SetActive(false);
        ShowInputButtons(false);

        // Activate only the needed text objects based on the number of players
        for (int i = 0; i < playerSummaryTexts.Length; i++)
        {
            if (i < players.Count)
            {
                playerSummaryTexts[i].gameObject.SetActive(true);
                playerSummaryTexts[i].text = FormatPlayerSummary(players[i]);
            }
            else
            {
                playerSummaryTexts[i].gameObject.SetActive(false);
            }
        }
    }
    private string FormatPlayerSummary(Player player)
    {
        return player.roundResult switch
        {
            0 => $"Player {player.name}: Lost : Current funds: {player.funds} $",
            1 => $"Player {player.name}: Won {player.wonAmount} $ : Current funds: {player.funds} $",
            2 => $"Player {player.name}: Push : Current funds: {player.funds} $",
            _ => $"Player {player.name}: Lost : Current funds: {player.funds} $",
        };
    }

    public void HideEndRoundSummary()
    {
        summaryPanel.SetActive(false);
    }


    public void SetBetSlider(Player player)
    {
        betPanel.SetActive(true);
        summaryPanel.SetActive(false);

        betSlider.maxValue = player.funds;
        betSlider.value = betSlider.minValue; // Reset to minimum value or a default value
        UpdateBetAmountText(betSlider.value);
        currentPlayerBetText.text = "Player: " + player.name;
        playerFundsText.text = "Funds: " + player.funds + "$";
    }

    public void HideBetSlider()
    {
        betPanel.SetActive(false);
    }

    public void OnBetSliderChanged(float value)
    {
        UpdateBetAmountText(value);
    }



    private void UpdateBetAmountText(float value)
    {
        if (betAmountText != null)
        {
            betAmountText.text = "Bet: " + value + " $";
        }
    }
    public void OnPlaceBetButtonClicked()
    {
        float betAmount = betSlider.value;
        GameManager.Instance.PlaceBet(betAmount);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
    public void ShowInputButtons(bool show)
    {
        hitButton.SetActive(show);
        standButton.SetActive(show);
        ShowDoubleDownButton(show);
        ShowSplitButton(show);
    }
    public void ShowDoubleDownButton(bool show)
    {
        doubleDownButton.SetActive(show);
    }
    public void ShowSplitButton(bool show)
    {
        splitButton.SetActive(show);
    }

    public void ShowPlayerUI(bool show)
    {
        foreach (Player player in GameManager.Instance.players)
        {
            player.ShowCanvas(show);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
