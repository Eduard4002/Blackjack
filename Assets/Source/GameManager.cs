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

    bool isPlayingSplitHand = false;

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
                foreach (Player player in players)
                {
                    Debug.Log("YES");
                    player.ShowCanvas(false);
                }
                UIManager.Instance.ShowInputButtons(false);
                //Activate the slider inside the UIManager
                UIManager.Instance.SetBetSlider(players[currentPlayerIndex].funds, players[currentPlayerIndex].name);
                break;
            case GameState.DealingInitialCards:
                UIManager.Instance.ShowInputButtons(true);

                DealInitialCardsToAll();
                break;
            case GameState.PlayerTurn:
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

    void DealInitialCardsToAll()
    {
        foreach (var player in players)
        {
            player.ShowCanvas(true);

            DealInitialCards(player);
            int handValue = player.CalculateHandValueForHand(player.hand);
            string showOnCanvas;

            if (handValue == 21)
            {
                showOnCanvas = "BLACKJACK";
                player.DisplayOnCanvas(showOnCanvas, new Color(1.0f, 1.0f, 0.0f, 1.0f));

            }
            else if (handValue > 21)
            {
                showOnCanvas = "BUST";
                player.DisplayOnCanvas(showOnCanvas, Color.red);

            }
            else
            {
                showOnCanvas = handValue.ToString() + " \n" + player.currentBet.ToString() + "$";

                player.DisplayOnCanvas(showOnCanvas, new Color(1.0f, 1.0f, 0.0f, 1.0f));

            }
            //string showOnCanvas = player.CalculateHandValueForHand(player.hand).ToString() + " \n" + player.currentBet.ToString() + "$";
        }
        DealInitialCards(dealer, true);
        if (dealer.CalculateHandValueForHand(dealer.hand) == 21)
        {
            SetState(GameState.EvaluatingWinner);
        }
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
        int dealerValue = dealer.CalculateHandValueForHand(dealer.hand);
        bool dealerBust = dealerValue > 21;

        foreach (Player player in players)
        {
            int playerValue = player.CalculateHandValueForHand(player.hand);
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
            HandDisplay.Instance.SetHandColor(players[i].hand, i == currentPlayerIndex ? CardColor.Active : CardColor.Inactive);
            HandDisplay.Instance.SetHandColor(players[i].splitHand, CardColor.Inactive);
        }
        if (currentPlayerIndex >= players.Count)
        {
            // All players have had their turn, move to dealer's turn
            SetState(GameState.DealerTurn);
            return;
        }



        Player currentPlayer = players[currentPlayerIndex];

        // Check if the player has blackjack
        if (currentPlayer.CalculateHandValueForHand(currentPlayer.hand) == 21)
        {
            PlayerStand();
        }
        // UI Updates
        UIManager.Instance.ShowDoubleDownButton(currentPlayer.CanDoubleDown());
        UIManager.Instance.ShowSplitButton(currentPlayer.CanSplit());


        // Check if currentPlayer's hand value is over 21
        if (currentPlayer.CalculateHandValueForHand(currentPlayer.hand) > 21)
        {
            currentPlayer.DisplayOnCanvas("BUST", Color.red);

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

        Player currentPlayer = players[currentPlayerIndex];
        List<Card> targetHand = isPlayingSplitHand ? currentPlayer.splitHand : currentPlayer.hand;
        Card card = Deck.Instance.GetCard();
        targetHand.Add(card);

        Vector3 handPosition = isPlayingSplitHand ? currentPlayer.splitHandPosition : currentPlayer.transform.position;


        HandDisplay.Instance.DisplayCard(card, handPosition, targetHand.Count, false, isPlayingSplitHand);
        UIManager.Instance.ShowDoubleDownButton(false);

        string showOnCanvas = currentPlayer.CalculateHandValueForHand(targetHand).ToString() + " \n" + currentPlayer.currentBet.ToString() + "$";
        currentPlayer.DisplayOnCanvas(showOnCanvas, new Color(1.0f, 1.0f, 0.0f, 1.0f));
        Debug.Log("Is the player playing a split hand? " + isPlayingSplitHand);
        if (currentPlayer.CalculateHandValueForHand(targetHand) > 21)
        {
            currentPlayer.DisplayOnCanvas("BUST", Color.red);
            // Player busts, move to the next player
            PlayerStand();
        }
        else if (currentPlayer.CalculateHandValueForHand(targetHand) == 21)
        {
            PlayerStand();
        }
    }
    public void PlayerStand()
    {
        if (CurrentState != GameState.PlayerTurn)
        {
            return;
        }

        Player currentPlayer = players[currentPlayerIndex];

        if (isPlayingSplitHand)
        {
            // Finished playing the split hand, move to the next player
            isPlayingSplitHand = false;
            currentPlayerIndex++;
            StartPlayerTurn();
        }
        else if (currentPlayer.hasSplit)
        {
            // Move to the split hand
            isPlayingSplitHand = true;
            HandDisplay.Instance.SetHandColor(currentPlayer.hand, CardColor.Inactive); // Set main hand to inactive

            HandDisplay.Instance.SetHandColor(currentPlayer.splitHand, CardColor.Active); // Set split hand to active
        }
        else
        {
            // No split hand, move to the next player
            currentPlayerIndex++;
            StartPlayerTurn();
        }
    }

    public void PlayerSplit()
    {
        if (CurrentState != GameState.PlayerTurn)
        {
            return;
        }
        //Hide double down and split buttons
        UIManager.Instance.ShowDoubleDownButton(false);
        UIManager.Instance.ShowSplitButton(false);

        Player currentPlayer = players[currentPlayerIndex];
        if (currentPlayer.CanSplit())
        {
            currentPlayer.Split();
            Card splitCard = currentPlayer.splitHand[0];

            //Reposition the cards
            HandDisplay.Instance.DisplayCard(splitCard, currentPlayer.splitHandPosition, 0, false, true);

            HandDisplay.Instance.SetHandColor(currentPlayer.hand, CardColor.Active); // Set main hand to active
            HandDisplay.Instance.SetHandColor(currentPlayer.splitHand, CardColor.Inactive); // Set split hand to inactive
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

            // Deal one card and move to the next player
            Card card = Deck.Instance.GetCard();
            currentPlayer.TakeCard(card);
            HandDisplay.Instance.DisplayCard(card, currentPlayer.transform.position, currentPlayer.hand.Count);
            string showOnCanvas = currentPlayer.CalculateHandValueForHand(currentPlayer.hand).ToString() + " \n" + currentPlayer.currentBet.ToString() + "$";
            currentPlayer.DisplayOnCanvas(showOnCanvas, new Color(1.0f, 1.0f, 0.0f, 1.0f));

            // Proceed to the next player's turn
            currentPlayerIndex++;
            StartPlayerTurn();
        }
    }

    IEnumerator DealerTurn()
    {
        // Reveal dealer's hidden card
        dealer.RevealHiddenCard();
        HandDisplay.Instance.UpdateCardSprite(dealer.GetHiddenCard());

        dealer.DisplayOnCanvas(dealer.CalculateHandValueForHand(dealer.hand).ToString(), new Color(1.0f, 1.0f, 0.0f, 1.0f));


        // Keep hitting until the dealer's hand value reaches or exceeds the threshold
        while (dealer.CalculateHandValueForHand(dealer.hand) < dealerStandValue)
        {
            yield return new WaitForSeconds(1); // Wait time between hits for better readability
            Card newCard = Deck.Instance.GetCard();
            dealer.TakeCard(newCard);
            HandDisplay.Instance.DisplayCard(newCard, dealer.transform.position, dealer.hand.Count, true);
            //UIManager.Instance.UpdateHandValueText(dealer.CalculateHandValueForHand(dealer.hand));
            dealer.DisplayOnCanvas(dealer.CalculateHandValueForHand(dealer.hand).ToString(), new Color(1.0f, 1.0f, 0.0f, 1.0f));

        }
        if (dealer.CalculateHandValueForHand(dealer.hand) > dealerStandValue)
        {
            dealer.DisplayOnCanvas("BUST", Color.red);
        }

        // Wait for 3 seconds after the dealer finishes their turn
        yield return new WaitForSeconds(1.5f);

        dealer.DisplayOnCanvas("", new Color(1.0f, 1.0f, 0.0f, 1.0f));


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


        SetState(GameState.PlacingBets);



        //UIManager.Instance.SetBetSlider(players[currentPlayerIndex].funds, players[currentPlayerIndex].name);


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
