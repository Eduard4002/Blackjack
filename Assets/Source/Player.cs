using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Card Settings")]
    //public Vector2 handPosition = new Vector2(0, -2); // Adjust this value as needed for the position of the hand

    public readonly List<Card> hand = new List<Card>();
    public List<Card> splitHand = new List<Card>(); // New list for the split hand

    public Vector3 splitHandOffset; // Offset for the split hand

    public Vector3 splitHandPosition; // Position of the split hand
    public bool hasSplit = false;

    public int handValue = 0;

    public float funds = 100;  // Starting funds
    public float currentBet = 0;

    public float wonAmount = 0;
    public bool hasWon = false;

    public new string name = "Player";

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
        wonAmount = 0;

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

    public bool CanDoubleDown()
    {
        // Usually, double down is allowed only if the player has exactly 2 cards
        return hand.Count == 2 && funds >= currentBet * 2;
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
        Debug.Log("Player wants to split");
        if (CanSplit())
        {
            Debug.Log("Splitting");
            hasSplit = true;
            splitHand.Add(hand[1]); // Move the second card to the split hand
            hand.RemoveAt(1); // Remove the second card from the original hand

        }
    }
    public bool CanSplit()
    {
        // Can split if the player has exactly 2 cards of the same rank
        return hand.Count == 2 && hand[0].value == hand[1].value;
    }

}
