using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    private int currentPlayerIndex = 0;
    public int dealerStandValue = 17;


    public GameObject[] playerPositions; // Assign in Inspector
    public int numberOfPlayers = 2; // Set this to the desired number of players
    public readonly List<Player> players = new List<Player>();

    public Dealer dealer; // Assign in Inspector

    void Start()
    {
        InitializePlayers();
        Deck.Instance.ShuffleCards();
        //Will be changed to placing bets
        SetState(GameState.DealingInitialCards);
    }
    void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChange();
    }

    void OnStateChange()
    {
        switch (CurrentState)
        {
            case GameState.PlacingBets:
                // Logic for placing bets
                break;
            case GameState.DealingInitialCards:
                DealInitialCardsToAll();
                break;
            case GameState.PlayerTurn:
                Debug.Log("H to Hit and S to Stand");
                StartPlayerTurn();
                break;
            case GameState.DealerTurn:
                StartCoroutine(DealerTurn());
                break;
            case GameState.EvaluatingWinner:
                // Evaluate the winner of the round
                break;
            case GameState.RoundEnd:
                // Wrap up the round
                break;
        }
    }
    void DealInitialCardsToAll()
    {
        foreach (var player in players)
        {
            DealInitialCards(player);
        }
        DealInitialCards(dealer, true);
        currentPlayerIndex = 0; // Reset to first player
        SetState(GameState.PlayerTurn); // Start player turns
    }

    void InitializePlayers()
    {
        // Determine the starting index based on the number of players
        int startIndex = numberOfPlayers == 1 ? 1 : 0; // Assuming index 1 is opposite the dealer

        for (int i = 0; i < numberOfPlayers; i++)
        {
            int positionIndex = (startIndex + i) % playerPositions.Length;
            if (positionIndex < playerPositions.Length)
            {
                Player player = playerPositions[positionIndex].GetComponent<Player>();
                if (player != null)
                {
                    player.gameObject.SetActive(true);
                    players.Add(player);
                }
            }
        }
    }

    void DealInitialCards(Player user, bool isDealer = false)
    {

        for (int i = 0; i < 2; i++) // Deal 2 cards initially
        {
            Card card = Deck.Instance.GetCard();

            if (isDealer && i == 1)
            {
                card.isFlipped = true;
                if (user is Dealer dealer)
                {
                    dealer.SetHiddenCard(card); // Set the hidden card for the dealer
                }
            }

            user.TakeCard(card);

            // Here, you would also update the UI to show the player's hand
            HandDisplay.Instance.DisplayCard(card, user.transform.position, user.hand.Count);

        }
    }
    void StartPlayerTurn()
    {
        if (currentPlayerIndex >= players.Count)
        {
            // All players have had their turn, move to dealer's turn
            SetState(GameState.DealerTurn);
            return;
        }

        Player currentPlayer = players[currentPlayerIndex];
        // Check if currentPlayer's hand value is over 21
        if (currentPlayer.CalculateHandValue() > 21)
        {
            currentPlayerIndex++;
            Debug.Log("Bust: " + currentPlayerIndex);
            StartPlayerTurn(); // Move to the next player
        }
        else
        {
            // Wait for player input (H to Hit, S to Stand)
            StartCoroutine(WaitForPlayerInput(currentPlayer));
        }
    }
    IEnumerator WaitForPlayerInput(Player player)

    {
        bool turnEnded = false;
        while (!turnEnded)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Card card = Deck.Instance.GetCard();
                player.TakeCard(card);
                HandDisplay.Instance.DisplayCard(card, player.transform.position, player.hand.Count);
                if (player.CalculateHandValue() > 21)
                {
                    turnEnded = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                turnEnded = true;
            }
            yield return null;
        }

        currentPlayerIndex++;
        StartPlayerTurn(); // Proceed to the next player's turn
    }
    IEnumerator DealerTurn()
    {
        // Reveal dealer's hidden card
        dealer.RevealHiddenCard();
        HandDisplay.Instance.UpdateCardSprite(dealer.GetHiddenCard());

        // Keep hitting until the dealer's hand value reaches or exceeds the threshold
        while (dealer.CalculateHandValue() < dealerStandValue)
        {
            yield return new WaitForSeconds(1); // Wait time between hits for better readability
            Card newCard = Deck.Instance.GetCard();
            dealer.TakeCard(newCard);
            HandDisplay.Instance.DisplayCard(newCard, dealer.transform.position, dealer.hand.Count);
        }

        // Once the dealer is done, move to the next state
        SetState(GameState.EvaluatingWinner);
    }
}
public enum GameState
{
    PlacingBets,       // State where players place their bets
    DealingInitialCards, // State for dealing the initial two cards to each player and the dealer
    PlayerTurn,        // State for player's actions (hit, stand, etc.)
    DealerTurn,        // State for dealer's actions
    EvaluatingWinner,  // State for determining the winner of the round
    RoundEnd           // State for concluding the round and preparing for the next one
}
