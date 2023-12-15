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

    public void DisplayCard(Card card, Vector3 handPosition, int cardCount, bool isDealer = false)
    {
        // Calculate position based on handPosition and number of cards
        //Vector3 cardPosition = new Vector3(handPosition.x, handPosition.y, 0) + new Vector3(cardSpacing * cardCount, 0, -cardCount);
        float verticalSpacing = isDealer ? 0 : 0.1f; // No vertical offset for dealer

        Vector3 cardPosition;
        if (isDealer)
        {
            cardPosition = new Vector3(handPosition.x, handPosition.y, 0) + new Vector3(cardSpacing * cardCount, 0, -cardCount);
        }
        else
        {
            cardPosition = new Vector3(handPosition.x + cardSpacing * cardCount, handPosition.y + cardSpacing * cardCount, -cardCount);
        }

        GameObject newCard = Instantiate(cardPrefab, cardPosition, Quaternion.identity);


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
        // Add the card and GameObject association to the dictionary
        cardGameObjectMap[card] = newCard;
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
    public void SetHandColor(Player player, CardColor color)
    {
        foreach (Card card in player.hand)
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
