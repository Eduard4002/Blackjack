using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Slider betSlider;
    [Header("Texts")]
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI currentPlayerBetText;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI handValueText;
    public TextMeshProUGUI currentBetText;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnHitButtonPressed() => GameManager.Instance.PlayerHit();
    public void OnStandButtonPressed() => GameManager.Instance.PlayerStand();
    public void OnDoubleDownButtonPressed() => GameManager.Instance.PlayerDoubleDown();

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
        string result = player.hasWon ? "Won " + player.wonAmount + " $" : "Lost";
        return $"Player {player.name}: {result} : Current funds: {player.funds} $";
    }

    public void HideEndRoundSummary()
    {
        summaryPanel.SetActive(false);
    }


    public void SetBetSlider(float maxBet, string playerName)
    {
        betPanel.SetActive(true);
        //betSlider.gameObject.SetActive(true);
        //placeBetButton.SetActive(true);
        //betAmountText.gameObject.SetActive(true);

        //ShowInputButtons(false);
        betSlider.maxValue = maxBet;
        betSlider.value = betSlider.minValue; // Reset to minimum value or a default value
        UpdateBetAmountText(betSlider.value);
        currentPlayerBetText.text = "Player: " + playerName;
    }

    public void HideBetSlider()
    {
        betPanel.SetActive(false);
        //betSlider.gameObject.SetActive(false);
        //placeBetButton.SetActive(false);
        //betAmountText.gameObject.SetActive(false);
        //ShowInputButtons(true);

    }

    public void OnBetSliderChanged(float value)
    {
        UpdateBetAmountText(value);
    }
    public void ShowPlayerInfo(bool show)
    {
        currentPlayerText.gameObject.SetActive(show);
        handValueText.gameObject.SetActive(show);
        currentBetText.gameObject.SetActive(show);
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
}
