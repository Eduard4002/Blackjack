using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

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
        PrintDeck();
    }


    public void ShuffleCards()
    {
        //Shuffle the cards
        //https://stackoverflow.com/questions/273313/randomize-a-listt
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
    public void TakeCard()
    {
        //Take a card from the deck
        Card card = deck.Pop();
        //Display the card
        Debug.Log(card.GetCardName());
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
