using System;
using System.Collections.Generic;

#nullable enable

namespace BlackjackPlugin.GameLogic;

public class Deck
{
    private List<Card> cards = new();
    private Random random = new();

    public Deck()
    {
        InitializeDeck();
        Shuffle();
    }

    private void InitializeDeck()
    {
        cards.Clear();

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            InitializeDeck();
            Shuffle();
        }

        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public int CardsRemaining => cards.Count;
}
