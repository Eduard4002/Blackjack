using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // Singleton pattern
    public static Deck Instance { get; private set; }
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

    public List<Card> cards = new List<Card>();
    //Stack of cards
    private Stack<Card> deck = new();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var card in cards)
        {
            card.Initialize();
        }
        ShuffleCards();
        //PrintDeck();
    }


    public void ShuffleCards()
    {
        //Remove isFlipped from all cards
        foreach (Card card in cards)
        {
            card.isFlipped = false;
        }
        //Shuffle the cards
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);

            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
        //Clear the stack
        deck.Clear();
        //Add the cards to the deck
        foreach (Card card in cards)
        {
            deck.Push(card);
        }
    }
    public Card GetCard()
    {
        if (deck.Count == 0)
        {
            Debug.Log("No cards left");
            return null;
        }
        //Take a card from the deck
        return deck.Pop();
    }
    public void PrintDeck()
    {
        //Print the deck
        foreach (Card card in deck)
        {
            Debug.Log(card.GetCardName());
        }
    }

}
