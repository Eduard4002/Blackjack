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

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        InitializePlayers();
        Deck.Instance.ShuffleCards();
        currentPlayerIndex = 0;
        //Will be changed to placing bets
        SetState(GameState.PlacingBets);
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
                //Activate the slider inside the UIManager
                UIManager.Instance.ShowPlayerInfo(false);
                UIManager.Instance.SetBetSlider(players[currentPlayerIndex].funds, players[currentPlayerIndex].name);
                break;
            case GameState.DealingInitialCards:
                DealInitialCardsToAll();
                break;
            case GameState.PlayerTurn:
                UIManager.Instance.ShowPlayerInfo(true);
                UIManager.Instance.ShowInputButtons(true);
                StartPlayerTurn();
                break;
            case GameState.DealerTurn:
                UIManager.Instance.ShowInputButtons(false);

                StartCoroutine(DealerTurn());
                break;
            case GameState.EvaluatingWinner:
                EvaluateWinners();
                break;
            case GameState.RoundEnd:
                UIManager.Instance.ShowEndRoundSummary(players);

                break;
        }
    }



    public void PlaceBet(float betAmount)
    {
        if (CurrentState != GameState.PlacingBets) return;
        players[currentPlayerIndex].PlaceBet(betAmount);
        // Proceed to the next player or next state
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            UIManager.Instance.HideBetSlider();
            SetState(GameState.DealingInitialCards);
        }
        else
        {
            UIManager.Instance.SetBetSlider(players[currentPlayerIndex].funds, players[currentPlayerIndex].name);
        }
    }
    public void PlayerDoubleDown()
    {
        if (CurrentState != GameState.PlayerTurn)
        {
            return;
        }

        Player currentPlayer = players[currentPlayerIndex];
        if (currentPlayer.CanDoubleDown())
        {
            currentPlayer.DoubleDown();
            UIManager.Instance.UpdateCurrentBetText(currentPlayer.currentBet);

            // Deal one card and move to the next player
            Card card = Deck.Instance.GetCard();
            currentPlayer.TakeCard(card);
            HandDisplay.Instance.DisplayCard(card, currentPlayer.transform.position, currentPlayer.hand.Count);
            UIManager.Instance.UpdateHandValueText(currentPlayer.CalculateHandValue());

            // Proceed to the next player's turn
            currentPlayerIndex++;
            StartPlayerTurn();
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
    void EvaluateWinners()
    {
        int dealerValue = dealer.CalculateHandValue();
        bool dealerBust = dealerValue > 21;

        foreach (Player player in players)
        {
            int playerValue = player.CalculateHandValue();
            bool playerBust = playerValue > 21;

            if (playerBust)
            {
                // Player busts and loses their bet
                player.LoseBet();
            }
            else if (dealerBust || playerValue > dealerValue)
            {
                // Player wins
                bool isBlackJack = playerValue == 21 && player.hand.Count == 2;
                player.WinBet(isBlackJack);
            }
            else if (playerValue == dealerValue)
            {
                // Push - player gets their bet back
                player.PushBet();
            }
            else
            {
                // Player loses
                player.LoseBet();
            }
        }

        // Proceed to the next state or round
        SetState(GameState.RoundEnd);
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
            HandDisplay.Instance.DisplayCard(card, user.transform.position, user.hand.Count, isDealer);

        }
    }
    void StartPlayerTurn()
    {
        // Color update for active/inactive players
        for (int i = 0; i < players.Count; i++)
        {
            HandDisplay.Instance.SetHandColor(players[i], i == currentPlayerIndex ? CardColor.Active : CardColor.Inactive);
        }
        if (currentPlayerIndex >= players.Count)
        {
            // All players have had their turn, move to dealer's turn
            SetState(GameState.DealerTurn);
            return;
        }

        Player currentPlayer = players[currentPlayerIndex];

        // UI Updates
        UIManager.Instance.UpdateCurrentPlayerText("Player: " + players[currentPlayerIndex].name);
        UIManager.Instance.UpdateHandValueText(currentPlayer.CalculateHandValue());
        UIManager.Instance.UpdateCurrentBetText(currentPlayer.currentBet);
        UIManager.Instance.ShowDoubleDownButton(currentPlayer.CanDoubleDown());


        // Check if currentPlayer's hand value is over 21
        if (currentPlayer.CalculateHandValue() > 21)
        {
            currentPlayerIndex++;
            StartPlayerTurn(); // Move to the next player
        }
    }

    public void PlayerHit()
    {
        if (CurrentState != GameState.PlayerTurn)
        {
            return;
        }

        // Handle hit logic
        Card card = Deck.Instance.GetCard();
        players[currentPlayerIndex].TakeCard(card);
        HandDisplay.Instance.DisplayCard(card, players[currentPlayerIndex].transform.position, players[currentPlayerIndex].hand.Count);

        UIManager.Instance.UpdateHandValueText(players[currentPlayerIndex].CalculateHandValue());
        UIManager.Instance.ShowDoubleDownButton(players[currentPlayerIndex].CanDoubleDown());

        if (players[currentPlayerIndex].CalculateHandValue() > 21)
        {

            // Player busts, move to the next player
            currentPlayerIndex++;
            StartPlayerTurn();
        }
    }

    public void PlayerStand()
    {
        if (CurrentState != GameState.PlayerTurn)
        {
            return;
        }

        // Player stands, move to the next player
        currentPlayerIndex++;
        StartPlayerTurn();
    }

    IEnumerator DealerTurn()
    {
        // Reveal dealer's hidden card
        dealer.RevealHiddenCard();
        HandDisplay.Instance.UpdateCardSprite(dealer.GetHiddenCard());

        UIManager.Instance.UpdateCurrentPlayerText("Dealer");
        UIManager.Instance.UpdateHandValueText(dealer.CalculateHandValue());
        UIManager.Instance.UpdateCurrentBetText(" ");

        // Keep hitting until the dealer's hand value reaches or exceeds the threshold
        while (dealer.CalculateHandValue() < dealerStandValue)
        {
            yield return new WaitForSeconds(1); // Wait time between hits for better readability
            Card newCard = Deck.Instance.GetCard();
            dealer.TakeCard(newCard);
            HandDisplay.Instance.DisplayCard(newCard, dealer.transform.position, dealer.hand.Count, true);
            UIManager.Instance.UpdateHandValueText(dealer.CalculateHandValue());

        }

        // Wait for 3 seconds after the dealer finishes their turn
        yield return new WaitForSeconds(3);

        // Transition to evaluating winner
        SetState(GameState.EvaluatingWinner);
    }

    public void RestartGame()
    {
        foreach (Player player in players)
        {

            player.Reset();

        }
        HandDisplay.Instance.ClearAllHands();

        // Reset the deck
        Deck.Instance.ShuffleCards();

        // Reset the dealer
        dealer.Reset();

        // Reset the UI
        UIManager.Instance.HideEndRoundSummary();

        // Reset the state
        currentPlayerIndex = 0;

        UIManager.Instance.SetBetSlider(players[currentPlayerIndex].funds, players[currentPlayerIndex].name);


        SetState(GameState.PlacingBets);
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
