using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : Player
{
    private Card hiddenCard;


    public void RevealHiddenCard()
    {
        if (hiddenCard != null)
        {
            hiddenCard.isFlipped = false;
            // Add logic to handle the card reveal in terms of game logic
        }
    }
    public void SetHiddenCard(Card card)
    {
        hiddenCard = card;
    }
    public Card GetHiddenCard()
    {
        return hiddenCard;
    }
}
