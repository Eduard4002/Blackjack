using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Slider betSlider;
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI handValueText;
    public TextMeshProUGUI currentBetText;

    public GameObject placeBetButton;
    public GameObject hitButton;
    public GameObject standButton;



    public GameObject summaryPanel;
    public TextMeshProUGUI[] playerSummaryTexts; // Assuming 3 text objects
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnHitButtonPressed()
    {
        GameManager.Instance.PlayerHit();
    }

    public void OnStandButtonPressed()
    {
        GameManager.Instance.PlayerStand();
    }
    public void ShowEndRoundSummary(List<Player> players)
    {
        summaryPanel.SetActive(true);

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
        Debug.Log(player.currentBet);
        string result = player.hasWon ? "Won: " + player.wonAmount : "Lost";
        return $"Player {player.name}: {result} : Current funds: {player.funds}";
    }

    public void HideEndRoundSummary()
    {
        summaryPanel.SetActive(false);
    }


    public void SetBetSlider(float maxBet)
    {
        betSlider.gameObject.SetActive(true);
        placeBetButton.SetActive(true);
        hitButton.SetActive(false);
        standButton.SetActive(false);
        betSlider.maxValue = maxBet;
        betSlider.value = betSlider.minValue; // Reset to minimum value or a default value
        UpdateBetAmountText(betSlider.value);
    }

    public void HideBetSlider()
    {
        betSlider.gameObject.SetActive(false);
        placeBetButton.SetActive(false);
        hitButton.SetActive(true);
        standButton.SetActive(true);

    }

    public void OnBetSliderChanged(float value)
    {
        UpdateBetAmountText(value);
        // Optionally, you can call a method in GameManager to update the current bet
    }

    private void UpdateBetAmountText(float value)
    {
        if (betAmountText != null)
        {
            betAmountText.text = "Bet: " + value;
        }
    }
    public void OnPlaceBetButtonClicked()
    {
        float betAmount = betSlider.value;
        GameManager.Instance.PlaceBet(betAmount);
    }

    public void UpdateCurrentPlayerText(string playerText)
    {
        currentPlayerText.text = playerText;
    }

    public void UpdateHandValueText(int handValue)
    {
        handValueText.text = "Hand Value: " + handValue;
    }

    public void UpdateCurrentBetText(float currentBet)
    {
        currentBetText.text = "Current Bet: " + currentBet;
    }
    public void UpdateCurrentBetText(string currentBet)
    {
        currentBetText.text = currentBet;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
