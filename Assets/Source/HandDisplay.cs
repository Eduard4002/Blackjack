using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public GameObject cardPrefab;
    public float cardSpacing = 0.5f; // Adjust this value as needed for spacing between cards
    // Singleton pattern
    public static HandDisplay Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want the deck to persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there's only one instance
        }
    }

    public void DisplayCardForPlayer(Card card, Vector3 handPosition, int cardCount)
    {
        // Calculate position based on handPosition and number of cards
        Vector3 cardPosition = new Vector3(handPosition.x, handPosition.y, 0) + new Vector3(cardSpacing * cardCount, 0, -cardCount);

        GameObject newCard = Instantiate(cardPrefab, cardPosition, Quaternion.identity);

        if (card.isFlipped)
        {
            newCard.GetComponent<SpriteRenderer>().sprite = card.cardBack;
        }
        else
        {
            newCard.GetComponent<SpriteRenderer>().sprite = card.cardImage;

        }
    }


}
