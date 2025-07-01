using System;
using System.Collections.Generic;
using BlackjackPlugin.Localization;
using static BlackjackPlugin.Localization.Localization;


namespace BlackjackPlugin.GameLogic;

// États possibles du jeu de blackjack
public enum GameState
{
    WaitingForBet,   // En attente de la mise du joueur
    PlayerTurn,      // Tour du joueur
    DealerTurn,      // Tour du croupier
    GameOver         // Fin de la partie
}

// Résultats possibles d'une partie
public enum GameResult
{
    None,             // Aucun résultat (partie en cours)
    PlayerWin,        // Joueur gagne
    DealerWin,        // Croupier gagne
    Push,             // Égalité
    PlayerBlackjack,  // Blackjack du joueur
    PlayerBust,       // Joueur dépasse 21
    DealerBust        // Croupier dépasse 21
}

// Classe principale gérant une partie de blackjack
public class BlackjackGame
{
    private Deck deck; // Pioche de cartes
    public List<Card> PlayerHand { get; private set; } // Main du joueur
    public List<Card> DealerHand { get; private set; } // Main du croupier
    public GameState State { get; private set; }       // État actuel du jeu
    public GameResult Result { get; private set; }     // Résultat de la partie
    public int CurrentBet { get; private set; }        // Mise actuelle
    public bool DealerHoleCardRevealed { get; private set; } // Carte cachée du croupier révélée ?
    public Language GameLanguage { get; set; } = Language.English; // Langue du jeu

    public event Action<string>? OnGameEvent; // Événements pour l'UI ou les logs

    // Constructeur : initialise le jeu
    public BlackjackGame()
    {
        deck = new Deck();
        PlayerHand = new List<Card>();
        DealerHand = new List<Card>();
        State = GameState.WaitingForBet;
        Result = GameResult.None;
    }

    // Démarre une nouvelle partie avec une mise donnée
    public void StartNewGame(int bet)
    {
        CurrentBet = bet;
        PlayerHand.Clear();
        DealerHand.Clear();
        DealerHoleCardRevealed = false;
        Result = GameResult.None;

        // Distribuer les cartes initiales (2 à chaque)
        PlayerHand.Add(deck.DrawCard());
        DealerHand.Add(deck.DrawCard());
        PlayerHand.Add(deck.DrawCard());
        DealerHand.Add(deck.DrawCard());

        OnGameEvent?.Invoke(Get("cards_dealt", GameLanguage));

        // Vérifier le blackjack immédiat
        if (GetHandValue(PlayerHand) == 21)
        {
            DealerHoleCardRevealed = true;
            if (GetHandValue(DealerHand) == 21)
            {
                Result = GameResult.Push;
                State = GameState.GameOver;
                OnGameEvent?.Invoke(Get("double_blackjack", GameLanguage));
            }
            else
            {
                Result = GameResult.PlayerBlackjack;
                State = GameState.GameOver;
                OnGameEvent?.Invoke(Get("blackjack", GameLanguage));
            }
        }
        else
        {
            State = GameState.PlayerTurn;
        }
    }

    // Le joueur pioche une carte
    public void Hit()
    {
        if (State != GameState.PlayerTurn) return;

        var card = deck.DrawCard();
        PlayerHand.Add(card);
        OnGameEvent?.Invoke(Get("card_drawn", GameLanguage, card.GetDisplayName()));
        
        // Si le joueur dépasse 21, il perd
        if (GetHandValue(PlayerHand) > 21)
        {
            DealerHoleCardRevealed = true;
            Result = GameResult.PlayerBust;
            State = GameState.GameOver;
            OnGameEvent?.Invoke(Get("bust_message", GameLanguage));
        }
    }

    // Le joueur s'arrête, c'est au tour du croupier
    public void Stand()
    {
        if (State != GameState.PlayerTurn) return;

        DealerHoleCardRevealed = true;
        State = GameState.DealerTurn;
        OnGameEvent?.Invoke(Get("dealer_reveals", GameLanguage));
        PlayDealerTurn();
    }

    // Logique du tour du croupier
    private void PlayDealerTurn()
    {
        // Le croupier pioche jusqu'à 17 ou plus
        while (GetHandValue(DealerHand) < 17)
        {
            var card = deck.DrawCard();
            DealerHand.Add(card);
            OnGameEvent?.Invoke(Get("dealer_draws", GameLanguage, card.GetDisplayName()));
        }

        int playerValue = GetHandValue(PlayerHand);
        int dealerValue = GetHandValue(DealerHand);

        // Déterminer le résultat de la partie
        if (dealerValue > 21)
        {
            Result = GameResult.DealerBust;
            OnGameEvent?.Invoke(Get("dealer_bust", GameLanguage));
        }
        else if (playerValue > dealerValue)
        {
            Result = GameResult.PlayerWin;
            OnGameEvent?.Invoke(Get("you_win", GameLanguage));
        }
        else if (dealerValue > playerValue)
        {
            Result = GameResult.DealerWin;
            OnGameEvent?.Invoke(Get("dealer_wins", GameLanguage));
        }
        else
        {
            Result = GameResult.Push;
            OnGameEvent?.Invoke(Get("tie", GameLanguage));
        }

        State = GameState.GameOver;
    }

    // Calcule la valeur d'une main (gère les As)
    public int GetHandValue(List<Card> hand)
    {
        int value = 0;
        int aces = 0;

        foreach (var card in hand)
        {
            if (card.Rank == Rank.Ace)
            {
                aces++;
                value += 11;
            }
            else
            {
                value += card.GetBlackjackValue();
            }
        }

        // Ajuster les As si la main dépasse 21
        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    // Calcule les gains selon le résultat
    public int GetWinnings()
    {
        return Result switch
        {
            GameResult.PlayerBlackjack => CurrentBet + (int)(CurrentBet * 1.5f), // Blackjack paie 3:2
            GameResult.PlayerWin or GameResult.DealerBust => CurrentBet * 2,      // Gain égal à la mise
            GameResult.Push => CurrentBet,                                        // Mise rendue
            GameResult.DealerWin or GameResult.PlayerBust => 0,                   // Perte totale
            _ => 0
        };
    }

    // Calcule le résultat net (gains - mise)
    public int GetNetResult()
    {
        return GetWinnings() - CurrentBet;
    }

    // Vérifie si le joueur peut doubler
    public bool CanDoubleDown()
    {
        return State == GameState.PlayerTurn && PlayerHand.Count == 2;
    }

    // Double la mise et pioche une carte
    public void DoubleDown()
    {
        if (!CanDoubleDown()) return;

        CurrentBet *= 2;
        Hit();
        
        // Si le joueur n'a pas bust, il doit s'arrêter
        if (State == GameState.PlayerTurn)
        {
            Stand();
        }
    }
}
