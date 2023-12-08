using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] playerPositions; // Assign in Inspector
    public int numberOfPlayers = 2; // Set this to the desired number of players
    public readonly List<Player> players = new List<Player>();

    void Start()
    {
        InitializePlayers(); // Example for 2 players
        Deck.Instance.ShuffleCards();

        //TODO: Players place bets


        // Example: Deal initial cards to players
        foreach (var player in players)
        {
            DealInitialCards(player);
        }
        //TODO: Deal initial cards to dealer


    }

    void InitializePlayers()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i < playerPositions.Length)
            {
                Player player = playerPositions[i].GetComponent<Player>();
                if (player != null)
                {
                    player.gameObject.SetActive(true);
                    // Perform additional player initialization here
                    players.Add(player);
                }
            }
        }
    }

    void DealInitialCards(Player player)
    {
        for (int i = 0; i < 2; i++) // Deal 2 cards initially
        {
            Card card = Deck.Instance.GetCard();
            player.TakeCard(card);
            // Here, you would also update the UI to show the player's hand
            HandDisplay.Instance.DisplayCardForPlayer(card, player.transform.position, player.hand.Count);

        }
    }
}
