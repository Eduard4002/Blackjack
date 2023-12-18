using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{
    public readonly List<Card> hand = new List<Card>();
    public List<Card> splitHand = new List<Card>(); // New list for the split hand

    public Vector3 splitHandOffset; // Offset for the split hand

    public Vector3 splitHandPosition; // Position of the split hand
    public bool hasSplit = false;

    public int handValue = 0;

    public float funds = 100;  // Starting funds
    public float currentBet = 0;

    public float wonAmount = 0;

    public new string name = "Player";



    private Canvas playerCanvas;
    private TextMeshProUGUI text;

    public short roundResult; // 0 = loss, 1 = win, 2 = push

    void Awake()
    {
        playerCanvas = GetComponentInChildren<Canvas>();
        if (playerCanvas != null)
        {
            // Find the TextMeshProUGUI component within the Canvas
            text = playerCanvas.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    void Start()
    {
        splitHandPosition = transform.position + splitHandOffset; // Adjust the offset as needed



    }

    public bool PlaceBet(float amount)
    {
        if (amount <= funds)
        {
            currentBet = amount;
            funds -= amount;
            return true;
        }
        return false;  // Not enough funds
    }

    // Methods to handle win/loss
    public void WinBet(bool isBlackJack)
    {
        if (isBlackJack)
        {
            funds += currentBet + (currentBet * 1.5f);  // 3:2 payout for Blackjack
            wonAmount = currentBet + (currentBet * 1.5f);
        }
        else
        {
            funds += currentBet * 2;  // 1:1 payout for regular win
            wonAmount = currentBet * 2;
        }
        roundResult = 1;
    }
    public void LoseBet()
    {
        roundResult = 0;
    }
    public void PushBet()
    {
        funds += currentBet;  // Return the bet to the player
        roundResult = 2;
    }
    public void Reset()
    {
        currentBet = 0;
        roundResult = 0;
        wonAmount = 0;

        hasSplit = false;
        ResetHand();
    }


    // Add a card to the player's hand and update the hand value
    public void TakeCard(Card card)
    {
        hand.Add(card);
        handValue = CalculateHandValueForHand(hand);

    }
    void ResetHand()
    {
        hand.Clear();
        splitHand.Clear();
        handValue = 0;
    }


    //Calculate the value of the player's hand
    public int CalculateHandValueForHand(List<Card> handToEvaluate)
    {
        int value = 0;
        int aceCount = 0;
        foreach (var card in handToEvaluate)
        {
            if (card.value == 1) // Assuming Ace has a value of 1
            {
                aceCount++;
                value += 11; // Temporarily treat Ace as 11
            }
            else if (card.value > 10) // For face cards (Jack, Queen, King)
            {
                value += 10;
            }
            else
            {
                value += card.value;
            }
        }

        // Adjust for Aces if total value exceeds 21
        while (value > 21 && aceCount > 0)
        {
            value -= 10; // Change an Ace from 11 to 1
            aceCount--;
        }

        return value;
    }
    public void ShowCanvas(bool show)
    {
        playerCanvas.gameObject.SetActive(show);

        if (playerCanvas != null)
        {
        }
    }
    public void DisplayOnCanvas(string textToDisplay, Color color)
    {
        if (text == null) return;

        text.text = textToDisplay;
        text.color = color;

    }



    public void DoubleDown()
    {
        if (CanDoubleDown())
        {
            funds -= currentBet; // Deduct the additional bet
            currentBet *= 2;
        }
    }
    public void Split()
    {
        if (CanSplit())
        {
            funds -= currentBet; // Deduct the additional bet
            currentBet *= 2;
            hasSplit = true;
            splitHand.Add(hand[1]); // Move the second card to the split hand
            hand.RemoveAt(1); // Remove the second card from the original hand

        }
    }
    public bool CanSplit()
    {
        // Can split if the player has exactly 2 cards of the same rank
        return hand.Count == 2 && hand[0].value == hand[1].value && funds >= currentBet && !hasSplit;
    }
    public bool CanDoubleDown()
    {
        // Usually, double down is allowed only if the player has exactly 2 cards and has not split
        return hand.Count == 2 && funds >= currentBet && !hasSplit;
    }

}
