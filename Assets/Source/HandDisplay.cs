using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CardColor
{
    Active, Inactive
}
public class HandDisplay : MonoBehaviour
{
    private Dictionary<Card, GameObject> cardGameObjectMap = new Dictionary<Card, GameObject>();

    public GameObject cardPrefab;
    public float cardSpacing = 0.6f; // Adjust this value as needed for spacing between cards

    public Color activeColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color backColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public float dealSpeed = 5f; // Adjust this value as needed for the speed of the card animation
    public Vector3 initialHandPosition = new Vector3(-2.5f, -3.5f, 0f); // Adjust this value as needed for the initial position of the hand
    // Singleton pattern
    public static HandDisplay Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if the deck should persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there's only one instance
        }
    }
    public void DisplayCard(Card card, Vector3 handPosition, int cardCount, bool isDealer = false, bool isSplitHand = false)
    {
        // Check if the card already has a GameObject and needs to be repositioned
        if (cardGameObjectMap.TryGetValue(card, out GameObject existingCardObject))
        {
            Debug.Log("Repositioning card object");
            Debug.Log("Hand pos: " + handPosition);
            // Reposition the existing card GameObject
            // Move existing card
            StartCoroutine(MoveCard(existingCardObject, handPosition, dealSpeed));
            //existingCardObject.transform.position = handPosition;
        }
        else
        {
            // Instantiate and position the new card GameObject
            Vector3 cardPosition = CalculateCardPosition(handPosition, cardCount, isDealer, isSplitHand);
            Debug.Log("Card pos: " + cardPosition);
            GameObject newCard = Instantiate(cardPrefab, initialHandPosition, Quaternion.identity);
            if (card.isFlipped)
            {
                newCard.GetComponent<SpriteRenderer>().sprite = card.cardBack;
                newCard.GetComponent<SpriteRenderer>().color = backColor;
            }
            else
            {
                newCard.GetComponent<SpriteRenderer>().sprite = card.cardImage;
                newCard.GetComponent<SpriteRenderer>().color = activeColor;
            }

            cardGameObjectMap[card] = newCard;
            StartCoroutine(MoveCard(newCard, cardPosition, dealSpeed));

        }
    }
    private IEnumerator MoveCard(GameObject cardObject, Vector3 targetPosition, float speed)
    {
        while (cardObject.transform.position != targetPosition)
        {
            cardObject.transform.position = Vector3.MoveTowards(cardObject.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private Vector3 CalculateCardPosition(Vector3 handPosition, int cardCount, bool isDealer, bool isSplitHand)
    {
        float horizontalSpacing = isDealer ? 0.5f : 0.5f; // Adjust as needed
        float verticalSpacing = isDealer ? 0 : 0.3f; // No vertical offset for dealer

        // Calculate the horizontal offset for split hand
        float splitHandOffset = isSplitHand ? 4f : 0; // Horizontal offset to separate the split hand

        // Adjust the y position for split hands
        float splitHandYOffset = isSplitHand ? -0.3f : 0; // Adjust as needed

        return new Vector3(handPosition.x + horizontalSpacing * (cardCount - 1),
                           handPosition.y + verticalSpacing * (cardCount - 1),
                           -cardCount);
    }



    public void UpdateCardSprite(Card card)
    {
        if (cardGameObjectMap.TryGetValue(card, out GameObject cardObject))
        {
            // Update the sprite of the card GameObject
            SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
            renderer.sprite = card.isFlipped ? card.cardBack : card.cardImage;
            renderer.color = card.isFlipped ? backColor : activeColor;
        }
        else
        {
            Debug.LogError("Card GameObject not found in map.");
        }
    }
    public void SetHandColor(List<Card> hand, CardColor color)
    {
        foreach (Card card in hand)
        {
            if (cardGameObjectMap.TryGetValue(card, out GameObject cardObject))
            {
                SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
                renderer.color = color == CardColor.Active ? activeColor : inactiveColor; // Set the new colors
            }
        }
    }
    public void ClearHand(Player player)
    {
        foreach (Card card in player.hand)
        {
            if (cardGameObjectMap.TryGetValue(card, out GameObject cardObject))
            {
                Debug.Log("Destroying card object");
                Destroy(cardObject); // Destroy the card GameObject
            }
        }
        cardGameObjectMap.Clear(); // Clear the dictionary
    }
    public void ClearAllHands()
    {
        foreach (var pair in cardGameObjectMap)
        {
            Destroy(pair.Value); // Destroy the card GameObject
        }
        cardGameObjectMap.Clear(); // Clear the dictionary
    }

}
