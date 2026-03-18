using System;
using System.Collections.Generic;

namespace BlackjackPlugin.GameLogic;

/// <summary>
/// Représente un paquet de cartes pour le jeu de Blackjack.
/// </summary>
public class Deck
{
    // Tableau statique des cartes
    private readonly Card[] cards = new Card[52];
    // Générateur de nombres aléatoires
    private readonly Random random = new();
    // Index de la prochaine carte à piocher
    private int currentIndex = 0;

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
        int i = 0;
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                cards[i++] = new Card(suit, rank);
            }
        }
    }

    /// <summary>
    /// Mélange les cartes du paquet.
    /// </summary>
    public void Shuffle()
    {
        for (int i = cards.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
        currentIndex = 0;
    }

    /// <summary>
    /// Pioche la première carte du paquet. Si le paquet est vide, il est remélangé.
    /// </summary>
    /// <returns>La carte piochée.</returns>
    public Card DrawCard()
    {
        if (currentIndex >= cards.Length)
        {
            Shuffle();
        }

        return cards[currentIndex++];
    }

    /// <summary>
    /// Nombre de cartes restantes dans le paquet.
    /// </summary>
    public int CardsRemaining => cards.Length - currentIndex;
}
