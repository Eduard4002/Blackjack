using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject spawnCard;
    public float cardSpacing = 0.5f; // Adjust this value as needed for spacing between cards
    public Vector2 handPosition = new Vector2(0, -2); // Adjust this value as needed for the position of the hand

    public List<Card> hand = new List<Card>();
    public int handValue = 0;
    public bool isStanding = false;
    bool isBust = false;

    // Add a card to the player's hand and update the hand value
    public void Hit()
    {
        // Draw a card from the deck
        Card card = Deck.Instance.GetCard();

        hand.Add(card);
        // Update handValue
        handValue = CalculateHandValue();
        Debug.Log(handValue);
        // Spawn the card
        // Calculate the position for the new card
        Vector3 newPosition = new Vector3(handPosition.x, handPosition.y, 0) + new Vector3(cardSpacing * hand.Count, 0, -hand.Count);

        // Spawn the card at the new position
        GameObject newCard = Instantiate(spawnCard, newPosition, Quaternion.identity);
        newCard.GetComponent<SpriteRenderer>().sprite = card.cardImage;
    }

    // Player decides to stand
    public void Stand()
    {
        isStanding = true;
    }
    void Update()
    {
        if (isBust) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hit();
        }
        if (handValue > 21)
        {
            Debug.Log("Bust");
            isBust = true;
        }
    }

    // Calculate the total value of the hand
    private int CalculateHandValue()
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
