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
        Debug.Log("Player has won!");
        Debug.Log("Has Blackjack: " + isBlackJack);
        if (isBlackJack)
        {
            funds += currentBet + (currentBet * 1.5f);  // 3:2 payout for Blackjack
        }
        else
        {
            funds += currentBet * 2;  // 1:1 payout for regular win
        }
        currentBet = 0;
    }
    public void LoseBet()
    {
        Debug.Log("Player has lost!");
        currentBet = 0;  // Bet is already subtracted when placed
    }
    public void PushBet()
    {
        Debug.Log("Player has pushed!");
        funds += currentBet;  // Return the bet to the player
        currentBet = 0;
    }


    // Add a card to the player's hand and update the hand value
    public void TakeCard(Card card)
    {
        hand.Add(card);
        handValue = CalculateHandValue();

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
