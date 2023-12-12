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

    public GameObject placeBetButton;
    public GameObject hitButton;
    public GameObject standButton;
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
}
