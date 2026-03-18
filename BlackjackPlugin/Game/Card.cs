namespace BlackjackPlugin.GameLogic;

// Enumération pour les couleurs (suits) des cartes
public enum Suit
{
    Hearts,    // Cœurs
    Diamonds,  // Carreaux
    Clubs,     // Trèfles
    Spades     // Piques
}

// Enumération pour les valeurs (rangs) des cartes
public enum Rank
{
    Ace = 1,    // As
    Two = 2,    // Deux
    Three = 3,  // Trois
    Four = 4,   // Quatre
    Five = 5,   // Cinq
    Six = 6,    // Six
    Seven = 7,  // Sept
    Eight = 8,  // Huit
    Nine = 9,   // Neuf
    Ten = 10,   // Dix
    Jack = 11,  // Valet
    Queen = 12, // Dame
    King = 13   // Roi
}

// Classe représentant une carte à jouer
public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }
    
    // Valeurs mises en cache pour éviter les allocations à chaque frame (60 fps)
    public string DisplayName { get; }
    public string ButtonText { get; }
    public uint CardColor { get; }
    public int BlackjackValue { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
        
        BlackjackValue = rank switch
        {
            Rank.Ace => 11,
            Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)rank
        };

        var suitSymbol = suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => ""
        };

        var rankName = rank switch
        {
            Rank.Ace => "A",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            _ => ((int)rank).ToString()
        };

        DisplayName = $"{rankName}{suitSymbol}";
        ButtonText = $" {DisplayName} ";

        CardColor = suit switch
        {
            Suit.Hearts or Suit.Diamonds => 0xFF0000FF,
            Suit.Clubs or Suit.Spades => 0x000000FF,
            _ => 0xFFFFFFFF
        };
    }

    public int GetBlackjackValue() => BlackjackValue;
    public string GetDisplayName() => DisplayName;
    public uint GetCardColor() => CardColor;
}
