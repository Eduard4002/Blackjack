using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Card Settings")]
    //public Vector2 handPosition = new Vector2(0, -2); // Adjust this value as needed for the position of the hand

    public readonly List<Card> hand = new List<Card>();
    public int handValue = 0;

    public float funds = 100;  // Starting funds
    public float currentBet = 0;

    public float wonAmount = 0;
    public bool hasWon = false;

    public new string name = "Player";

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
        hasWon = true;
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
    }
    public void LoseBet()
    {
        hasWon = false;
    }
    public void PushBet()
    {
        funds += currentBet;  // Return the bet to the player
    }
    public void Reset()
    {
        currentBet = 0;
        hasWon = false;
        ResetHand();
    }


    // Add a card to the player's hand and update the hand value
    public void TakeCard(Card card)
    {
        hand.Add(card);
        handValue = CalculateHandValue();

    }
    void ResetHand()
    {
        hand.Clear();
        handValue = 0;
    }



    // Calculate the total value of the hand
    public int CalculateHandValue()
    {
        int value = 0;
        int aceCount = 0;
        foreach (var card in hand)
        {
            if (card.value == 1) // Assuming Ace has a value of 1
            {
                aceCount++;
                value += 11; // Temporarily treat Ace as 11
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

}
