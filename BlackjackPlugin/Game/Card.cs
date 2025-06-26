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
    // Propriété pour la couleur de la carte
    public Suit Suit { get; }
    // Propriété pour la valeur de la carte
    public Rank Rank { get; }

    // Constructeur de la carte
    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // Retourne la valeur de la carte pour le Blackjack
    public int GetBlackjackValue()
    {
        return Rank switch
        {
            Rank.Ace => 11, // L'as vaut 11 (sera ajusté si nécessaire)
            Rank.Jack or Rank.Queen or Rank.King => 10, // Figures valent 10
            _ => (int)Rank // Les autres gardent leur valeur numérique
        };
    }

    // Retourne le nom affiché de la carte (ex: "A♥", "10♠")
    public string GetDisplayName()
    {
        var suitSymbol = Suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => ""
        };

        var rankName = Rank switch
        {
            Rank.Ace => "A",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            _ => ((int)Rank).ToString()
        };

        return $"{rankName}{suitSymbol}";
    }

    // Retourne la couleur de la carte sous forme de code couleur (uint)
    public uint GetCardColor()
    {
        return Suit switch
        {
            Suit.Hearts or Suit.Diamonds => 0xFF0000FF, // Rouge
            Suit.Clubs or Suit.Spades => 0x000000FF,   // Noir
            _ => 0xFFFFFFFF
        };
    }
}
