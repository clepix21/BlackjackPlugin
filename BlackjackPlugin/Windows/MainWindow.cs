using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using BlackjackPlugin.GameLogic;
using System.Collections.Generic;

namespace BlackjackPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;
    private BlackjackGame game;
    private int betAmount = 50;
    private List<string> gameLog;
    private const int MaxLogEntries = 10;

    public MainWindow(Plugin plugin) : base("🎰 Blackjack Casino")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 425),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        this.game = new BlackjackGame();
        this.betAmount = plugin.Configuration.DefaultBet;
        this.gameLog = new List<string>();

        // S'abonner aux événements du jeu
        game.OnGameEvent += AddToLog;
    }

    public void Dispose()
    {
        game.OnGameEvent -= AddToLog;
    }

    private void AddToLog(string message)
    {
        gameLog.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (gameLog.Count > MaxLogEntries)
        {
            gameLog.RemoveAt(0);
        }
    }

    public override void Draw()
    {
        DrawHeader();
        ImGui.Separator();
        
        DrawGameArea();
        ImGui.Separator();
        
        DrawControls();
        ImGui.Separator();
        
        DrawGameLog();
    }

    private void DrawHeader()
    {
        // Style du header
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f)); // Or
        ImGui.Text($"💰 Argent: {plugin.Configuration.PlayerMoney} Gil");
        ImGui.PopStyleColor();
        
        ImGui.SameLine();
        if (game.CurrentBet > 0)
        {
            ImGui.Text($"🎯 Mise: {game.CurrentBet} Gil");
        }
    }

    private void DrawGameArea()
    {
        // Zone du croupier
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.2f, 0.2f, 1.0f)); // Rouge
        ImGui.Text("🎩 Croupier:");
        ImGui.PopStyleColor();
        
        ImGui.Indent();
        DrawHand(game.DealerHand, !game.DealerHoleCardRevealed);
        
        if (game.DealerHoleCardRevealed)
        {
            int dealerValue = game.GetHandValue(game.DealerHand);
            ImGui.Text($"Total: {dealerValue}");
            if (dealerValue > 21)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "BUST!");
            }
        }
        else if (game.DealerHand.Count > 0)
        {
            ImGui.Text($"Total: {game.DealerHand[0].GetBlackjackValue()} + ?");
        }
        ImGui.Unindent();
        
        ImGui.Spacing();
        ImGui.Spacing();

        // Zone du joueur
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 0.8f, 0.2f, 1.0f)); // Vert
        ImGui.Text("👤 Votre main:");
        ImGui.PopStyleColor();
        
        ImGui.Indent();
        DrawHand(game.PlayerHand, false);
        
        if (game.PlayerHand.Count > 0)
        {
            int playerValue = game.GetHandValue(game.PlayerHand);
            ImGui.Text($"Total: {playerValue}");
            
            if (playerValue > 21)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "BUST!");
            }
            else if (playerValue == 21 && game.PlayerHand.Count == 2)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "BLACKJACK!");
            }
        }
        ImGui.Unindent();
    }

    private void DrawHand(List<Card> hand, bool hideSecondCard)
    {
        if (hand.Count == 0) return;

        for (int i = 0; i < hand.Count; i++)
        {
            if (i > 0) ImGui.SameLine();
            
            if (i == 1 && hideSecondCard)
            {
                // Carte cachée
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
                ImGui.Button("🂠");
                ImGui.PopStyleColor(2);
            }
            else
            {
                // Carte visible avec couleur
                var card = hand[i];
                var color = card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds 
                    ? new Vector4(0.8f, 0.2f, 0.2f, 1.0f) 
                    : new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.Text, color);
                ImGui.Button($" {card.GetDisplayName()} ");
                ImGui.PopStyleColor(2);
            }
        }
    }

    private void DrawControls()
    {
        switch (game.State)
        {
            case GameState.WaitingForBet:
                DrawBettingControls();
                break;
                
            case GameState.PlayerTurn:
                DrawPlayerTurnControls();
                break;
                
            case GameState.DealerTurn:
                ImGui.Text("🎲 Le croupier joue...");
                break;
                
            case GameState.GameOver:
                DrawGameOverControls();
                break;
        }
    }

    private void DrawBettingControls()
    {
        ImGui.Text("💸 Placez votre mise:");
        
        // Boutons de mise rapide
        if (ImGui.Button("10 Gil")) betAmount = 10;
        ImGui.SameLine();
        if (ImGui.Button("50 Gil")) betAmount = 50;
        ImGui.SameLine();
        if (ImGui.Button("100 Gil")) betAmount = 100;
        ImGui.SameLine();
        if (ImGui.Button("Max")) betAmount = plugin.Configuration.PlayerMoney;
        
        ImGui.SetNextItemWidth(150);
        ImGui.SliderInt("##bet", ref betAmount, 10, plugin.Configuration.PlayerMoney);
        
        if (betAmount < 10) betAmount = 10;
        if (betAmount > plugin.Configuration.PlayerMoney) betAmount = plugin.Configuration.PlayerMoney;
        
        ImGui.Spacing();
        
        if (plugin.Configuration.PlayerMoney <= 0)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), "Vous n'avez plus d'argent!");
            if (ImGui.Button("Réinitialiser l'argent"))
            {
                plugin.Configuration.PlayerMoney = 1000;
                plugin.Configuration.Save();
            }
        }
        else if (ImGui.Button("🎯 Distribuer les cartes") && betAmount <= plugin.Configuration.PlayerMoney)
        {
            plugin.Configuration.PlayerMoney -= betAmount;
            plugin.Configuration.Save();
            game.StartNewGame(betAmount);
        }
    }

    private void DrawPlayerTurnControls()
    {
        // Boutons principaux
        if (ImGui.Button("🃏 Tirer une carte"))
        {
            game.Hit();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button("✋ Rester"))
        {
            game.Stand();
        }
        
        // Double Down si possible
        if (game.CanDoubleDown() && plugin.Configuration.PlayerMoney >= game.CurrentBet)
        {
            ImGui.SameLine();
            if (ImGui.Button("⬆️ Double"))
            {
                plugin.Configuration.PlayerMoney -= game.CurrentBet;
                plugin.Configuration.Save();
                game.DoubleDown();
            }
        }
    }

    private void DrawGameOverControls()
    {
        string resultText = game.Result switch
        {
            GameResult.PlayerBlackjack => "🎉 BLACKJACK! Vous gagnez!",
            GameResult.PlayerWin or GameResult.DealerBust => "🎊 Vous gagnez!",
            GameResult.DealerWin or GameResult.PlayerBust => "😞 Le croupier gagne",
            GameResult.Push => "🤝 Égalité!",
            _ => ""
        };
        
        Vector4 color = game.Result switch
        {
            GameResult.PlayerBlackjack or GameResult.PlayerWin or GameResult.DealerBust => new Vector4(0, 1, 0, 1),
            GameResult.DealerWin or GameResult.PlayerBust => new Vector4(1, 0, 0, 1),
            GameResult.Push => new Vector4(1, 1, 0, 1),
            _ => new Vector4(1, 1, 1, 1)
        };
        
        ImGui.TextColored(color, resultText);
        
        int winnings = game.GetWinnings();
        if (winnings > 0)
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), $"💰 Gains: +{winnings} Gil");
        }
        else if (winnings < 0)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), $"💸 Pertes: {winnings} Gil");
        }
        else
        {
            ImGui.Text("💰 Pas de gains/pertes");
        }
        
        ImGui.Spacing();
        
        if (ImGui.Button("🔄 Nouvelle partie"))
        {
            plugin.Configuration.PlayerMoney += winnings;
            plugin.Configuration.Save();
            game = new BlackjackGame();
            game.OnGameEvent += AddToLog;
            gameLog.Clear();
        }
    }

    private void DrawGameLog()
    {
        if (ImGui.CollapsingHeader("📜 Historique du jeu"))
        {
            ImGui.BeginChild("GameLog", new Vector2(0, 100), true);
            
            foreach (var entry in gameLog)
            {
                ImGui.TextWrapped(entry);
            }
            
            // Auto-scroll vers le bas
            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                ImGui.SetScrollHereY(1.0f);
                
            ImGui.EndChild();
        }
    }
}
