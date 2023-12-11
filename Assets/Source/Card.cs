
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]

public class Card : ScriptableObject
{
    public Sprite cardImage;
    [HideInInspector]
    public int value; // This value will be 10 for Jacks, Queens, and Kings
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    [HideInInspector]
    public Suit cardSuit;
    public bool isFlipped = false;
    public Sprite cardBack;
    public void Initialize()
    {
        // Assuming cardImage is already set and has a name like "Clovers_2_white"
        string[] parts = cardImage.name.Split('_');
        if (parts.Length >= 2)
        {
            // Parse the suit
            cardSuit = parts[0] switch
            {
                "Hearts" => Suit.Hearts,
                "Tiles" => Suit.Diamonds,
                "Clovers" => Suit.Clubs,
                "Pikes" => Suit.Spades,
                _ => Suit.Clubs // Default or error handling
            };

            // Parse the value
            if (int.TryParse(parts[1], out int cardValue))
            {
                value = cardValue;
            }
            else
            {
                // Handle face cards
                value = parts[1] switch
                {
                    "Jack" => 10,
                    "Queen" => 10,
                    "King" => 10,
                    "A" => 1, // Or 11 depending on how you want to handle Aces
                    _ => 0 // Error handling
                };
            }
        }
        isFlipped = false;
        //Used for auto naming the scriptable object, do not need it anymore
        /*
                //Set the scriptable object name

                string newObjectName = GetCardName() + " SO";
                name = newObjectName;

        #if UNITY_EDITOR
                // Update the asset's filename to match the Scriptable Object's name
                string assetPath = AssetDatabase.GetAssetPath(this);
                AssetDatabase.RenameAsset(assetPath, newObjectName);
                AssetDatabase.SaveAssets();
        #endif*/
    }
    public string GetCardName()
    {
        string valueName;
        if (cardImage.name.Contains("Jack"))
            valueName = "Jack";
        else if (cardImage.name.Contains("Queen"))
            valueName = "Queen";
        else if (cardImage.name.Contains("King"))
            valueName = "King";
        else if (cardImage.name.Contains("A"))
            valueName = "Ace";
        else
            valueName = value.ToString();

        return $"{valueName} of {cardSuit}";
    }

    private string GetFaceCardName()
    {
        // You can add logic here to differentiate between Jack, Queen, and King
        // This might involve adding an additional property or parameter to the Card class
        return name; // Placeholder: Replace with actual logic to determine if it's a Jack, Queen, or King
    }
}
