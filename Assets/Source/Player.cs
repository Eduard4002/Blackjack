using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Card Settings")]
    //public Vector2 handPosition = new Vector2(0, -2); // Adjust this value as needed for the position of the hand

    public readonly List<Card> hand = new List<Card>();
    public int handValue = 0;
    public bool isStanding = false;
    bool isBust = false;

    // Add a card to the player's hand and update the hand value
    public void TakeCard(Card card)
    {
        hand.Add(card);
        handValue = CalculateHandValue();

    }


    // Player decides to stand
    public void Stand()
    {
        isStanding = true;
    }
    void Update()
    {
        if (isBust) return;

        if (handValue > 21)
        {
            Debug.Log("Bust");
            isBust = true;
        }
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
