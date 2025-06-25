using System;
using System.Collections.Generic;

#nullable enable

namespace BlackjackPlugin.GameLogic;

public enum GameState
{
    WaitingForBet,
    PlayerTurn,
    DealerTurn,
    GameOver
}

public enum GameResult
{
    None,
    PlayerWin,
    DealerWin,
    Push,
    PlayerBlackjack,
    PlayerBust,
    DealerBust
}

public class BlackjackGame
{
    private Deck deck;
    public List<Card> PlayerHand { get; private set; }
    public List<Card> DealerHand { get; private set; }
    public GameState State { get; private set; }
    public GameResult Result { get; private set; }
    public int CurrentBet { get; private set; }
    public bool DealerHoleCardRevealed { get; private set; }

    public event Action<string>? OnGameEvent;

    public BlackjackGame()
    {
        deck = new Deck();
        PlayerHand = new List<Card>();
        DealerHand = new List<Card>();
        State = GameState.WaitingForBet;
        Result = GameResult.None;
    }

    public void StartNewGame(int bet)
    {
        CurrentBet = bet;
        PlayerHand.Clear();
        DealerHand.Clear();
        DealerHoleCardRevealed = false;
        Result = GameResult.None;

        // Distribuer les cartes initiales
        PlayerHand.Add(deck.DrawCard());
        DealerHand.Add(deck.DrawCard());
        PlayerHand.Add(deck.DrawCard());
        DealerHand.Add(deck.DrawCard());

        OnGameEvent?.Invoke("Cartes distribuées");

        // Vérifier le blackjack
        if (GetHandValue(PlayerHand) == 21)
        {
            DealerHoleCardRevealed = true;
            if (GetHandValue(DealerHand) == 21)
            {
                Result = GameResult.Push;
                State = GameState.GameOver;
                OnGameEvent?.Invoke("Double blackjack - Égalité!");
            }
            else
            {
                Result = GameResult.PlayerBlackjack;
                State = GameState.GameOver;
                OnGameEvent?.Invoke("BLACKJACK!");
            }
        }
        else
        {
            State = GameState.PlayerTurn;
        }
    }

    public void Hit()
    {
        if (State != GameState.PlayerTurn) return;

        var card = deck.DrawCard();
        PlayerHand.Add(card);
        OnGameEvent?.Invoke($"Carte tirée: {card.GetDisplayName()}");
        
        if (GetHandValue(PlayerHand) > 21)
        {
            DealerHoleCardRevealed = true;
            Result = GameResult.PlayerBust;
            State = GameState.GameOver;
            OnGameEvent?.Invoke("BUST! Vous avez dépassé 21");
        }
    }

    public void Stand()
    {
        if (State != GameState.PlayerTurn) return;

        DealerHoleCardRevealed = true;
        State = GameState.DealerTurn;
        OnGameEvent?.Invoke("Le croupier révèle sa carte");
        PlayDealerTurn();
    }

    private void PlayDealerTurn()
    {
        while (GetHandValue(DealerHand) < 17)
        {
            var card = deck.DrawCard();
            DealerHand.Add(card);
            OnGameEvent?.Invoke($"Le croupier tire: {card.GetDisplayName()}");
        }

        int playerValue = GetHandValue(PlayerHand);
        int dealerValue = GetHandValue(DealerHand);

        if (dealerValue > 21)
        {
            Result = GameResult.DealerBust;
            OnGameEvent?.Invoke("Le croupier dépasse 21 - Vous gagnez!");
        }
        else if (playerValue > dealerValue)
        {
            Result = GameResult.PlayerWin;
            OnGameEvent?.Invoke("Vous gagnez!");
        }
        else if (dealerValue > playerValue)
        {
            Result = GameResult.DealerWin;
            OnGameEvent?.Invoke("Le croupier gagne");
        }
        else
        {
            Result = GameResult.Push;
            OnGameEvent?.Invoke("Égalité!");
        }

        State = GameState.GameOver;
    }

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

        // Ajuster les As si nécessaire
        while (value > 21 && aces > 0)
        {
            value -= 10;
            aces--;
        }

        return value;
    }

    public int GetWinnings()
    {
        return Result switch
        {
            GameResult.PlayerBlackjack => (int)(CurrentBet * 1.5f),
            GameResult.PlayerWin or GameResult.DealerBust => CurrentBet,
            GameResult.Push => 0,
            GameResult.DealerWin or GameResult.PlayerBust => -CurrentBet,
            _ => 0
        };
    }

    public bool CanDoubleDown()
    {
        return State == GameState.PlayerTurn && PlayerHand.Count == 2;
    }

    public void DoubleDown()
    {
        if (!CanDoubleDown()) return;

        CurrentBet *= 2;
        Hit();
        
        if (State == GameState.PlayerTurn) // Si pas bust
        {
            Stand();
        }
    }
}
