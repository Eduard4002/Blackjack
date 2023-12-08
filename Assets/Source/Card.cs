
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]

public class Card : ScriptableObject
{
    public Sprite cardImage;
    public new string name;
    public int value; // This value will be 10 for Jacks, Queens, and Kings
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public Suit cardSuit;

    public string GetCardName()
    {

        /*
        string valueName = value switch
        {
            1 => "Ace",
            10 => GetFaceCardName(),
            _ => value.ToString()
        };*/

        return $"{value} of {cardSuit}";
    }

    private string GetFaceCardName()
    {
        // You can add logic here to differentiate between Jack, Queen, and King
        // This might involve adding an additional property or parameter to the Card class
        return name; // Placeholder: Replace with actual logic to determine if it's a Jack, Queen, or King
    }
}
