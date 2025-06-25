namespace BlackjackPlugin.GameLogic;

public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum Rank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public int GetBlackjackValue()
    {
        return Rank switch
        {
            Rank.Ace => 11, // Sera ajusté si nécessaire
            Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)Rank
        };
    }

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
