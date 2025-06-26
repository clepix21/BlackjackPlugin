using System;
using System.Collections.Generic;

#nullable enable

namespace BlackjackPlugin.GameLogic;

/// <summary>
/// Représente un paquet de cartes pour le jeu de Blackjack.
/// </summary>
public class Deck
{
    // Liste des cartes dans le paquet
    private List<Card> cards = new();
    // Générateur de nombres aléatoires pour le mélange
    private Random random = new();

    /// <summary>
    /// Initialise un nouveau paquet mélangé.
    /// </summary>
    public Deck()
    {
        InitializeDeck();
        Shuffle();
    }

    /// <summary>
    /// Remplit le paquet avec toutes les combinaisons de couleurs et valeurs.
    /// </summary>
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

    /// <summary>
    /// Mélange les cartes du paquet.
    /// </summary>
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    /// <summary>
    /// Pioche la première carte du paquet. Si le paquet est vide, il est réinitialisé et mélangé.
    /// </summary>
    /// <returns>La carte piochée.</returns>
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

    /// <summary>
    /// Nombre de cartes restantes dans le paquet.
    /// </summary>
    public int CardsRemaining => cards.Count;
}
