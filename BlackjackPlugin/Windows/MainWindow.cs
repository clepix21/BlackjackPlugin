using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using BlackjackPlugin.GameLogic;
using BlackjackPlugin.Localization;
using System.Collections.Generic;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;
    private BlackjackGame game;
    private int betAmount = 50;
    private List<string> gameLog;
    private const int MaxLogEntries = 10;

    public MainWindow(Plugin plugin) : base(
        Get("window_title", plugin.Configuration.CurrentLanguage))
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
        
        // Synchroniser la langue du jeu
        game.GameLanguage = plugin.Configuration.CurrentLanguage;
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
        var lang = plugin.Configuration.CurrentLanguage;
        
        // Mise à jour du titre de la fenêtre et de la langue du jeu
        WindowName = Get("window_title", lang);
        game.GameLanguage = lang;
        
        // Vérifier si une sauvegarde est sélectionnée
        if (plugin.Configuration.CurrentSave == null)
        {
            DrawNoSaveSelected();
            return;
        }
        
        DrawHeader();
        ImGui.Separator();
        
        DrawGameArea();
        ImGui.Separator();
        
        DrawControls();
        ImGui.Separator();
        
        DrawGameLog();
    }

    private void DrawNoSaveSelected()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        ImGui.SetCursorPosY(ImGui.GetWindowHeight() / 2 - 50);
        
        var text = Get("select_save", lang);
        var textSize = ImGui.CalcTextSize(text);
        ImGui.SetCursorPosX((ImGui.GetWindowWidth() - textSize.X) / 2);
        ImGui.TextColored(new Vector4(1.0f, 0.8f, 0.2f, 1.0f), text);
        
        ImGui.SetCursorPosX((ImGui.GetWindowWidth() - 200) / 2);
        if (ImGui.Button(Get("save_management", lang), new Vector2(200, 30)))
        {
            plugin.ToggleConfigUI();
        }
    }

    private void DrawHeader()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        var currentSave = plugin.Configuration.CurrentSave!;
        
        // Style du header
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.84f, 0.0f, 1.0f)); // Or
        ImGui.Text($"{Get("money", lang)}: {currentSave.PlayerMoney} Gil");
        ImGui.PopStyleColor();
        
        ImGui.SameLine();
        ImGui.Text($"{Get("current_save", lang)}: {currentSave.Name}");
        
        ImGui.SameLine();
        if (game.CurrentBet > 0)
        {
            ImGui.Text($"{Get("bet", lang)}: {game.CurrentBet} Gil");
        }
    }

    private void DrawGameArea()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        // Zone du croupier
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.8f, 0.2f, 0.2f, 1.0f)); // Rouge
        ImGui.Text(Get("dealer", lang));
        ImGui.PopStyleColor();
        
        ImGui.Indent();
        DrawHand(game.DealerHand, !game.DealerHoleCardRevealed);
        
        if (game.DealerHoleCardRevealed)
        {
            int dealerValue = game.GetHandValue(game.DealerHand);
            ImGui.Text($"{Get("total", lang)}: {dealerValue}");
            if (dealerValue > 21)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), Get("bust", lang));
            }
        }
        else if (game.DealerHand.Count > 0)
        {
            ImGui.Text($"{Get("total", lang)}: {game.DealerHand[0].GetBlackjackValue()} + ?");
        }
        ImGui.Unindent();
        
        ImGui.Spacing();
        ImGui.Spacing();

        // Zone du joueur
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 0.8f, 0.2f, 1.0f)); // Vert
        ImGui.Text(Get("your_hand", lang));
        ImGui.PopStyleColor();
        
        ImGui.Indent();
        DrawHand(game.PlayerHand, false);
        
        if (game.PlayerHand.Count > 0)
        {
            int playerValue = game.GetHandValue(game.PlayerHand);
            ImGui.Text($"{Get("total", lang)}: {playerValue}");
            
            if (playerValue > 21)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1, 0, 0, 1), Get("bust", lang));
            }
            else if (playerValue == 21 && game.PlayerHand.Count == 2)
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0, 1, 0, 1), Get("blackjack", lang));
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
                var lang = plugin.Configuration.CurrentLanguage;
                ImGui.Text(Get("dealer_playing", lang));
                break;
                
            case GameState.GameOver:
                DrawGameOverControls();
                break;
        }
    }

    private void DrawBettingControls()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        var currentSave = plugin.Configuration.CurrentSave!;
        
        ImGui.Text(Get("place_bet", lang));
        
        // Boutons de mise rapide
        if (ImGui.Button("10 Gil")) betAmount = 10;
        ImGui.SameLine();
        if (ImGui.Button("50 Gil")) betAmount = 50;
        ImGui.SameLine();
        if (ImGui.Button("100 Gil")) betAmount = 100;
        ImGui.SameLine();
        if (ImGui.Button("Max")) betAmount = currentSave.PlayerMoney;
        
        ImGui.SetNextItemWidth(150);
        ImGui.SliderInt("##bet", ref betAmount, 10, currentSave.PlayerMoney);
        
        if (betAmount < 10) betAmount = 10;
        if (betAmount > currentSave.PlayerMoney) betAmount = currentSave.PlayerMoney;
        
        ImGui.Spacing();
        
        if (currentSave.PlayerMoney <= 0)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), Get("no_money", lang));
            if (ImGui.Button(Get("reset_money", lang)))
            {
                plugin.Configuration.UpdatePlayerMoney(1000);
            }
        }
        else if (ImGui.Button(Get("deal_cards", lang)) && betAmount <= currentSave.PlayerMoney)
        {
            game.StartNewGame(betAmount);
        }
    }

    private void DrawPlayerTurnControls()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        var currentSave = plugin.Configuration.CurrentSave!;
        
        // Boutons principaux
        if (ImGui.Button(Get("hit", lang)))
        {
            game.Hit();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button(Get("stand", lang)))
        {
            game.Stand();
        }
        
        // Double Down si possible
        if (game.CanDoubleDown() && currentSave.PlayerMoney >= game.CurrentBet)
        {
            ImGui.SameLine();
            if (ImGui.Button(Get("double", lang)))
            {
                game.DoubleDown();
            }
        }
    }

    private void DrawGameOverControls()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        string resultText = game.Result switch
        {
            GameResult.PlayerBlackjack => Get("blackjack_win", lang),
            GameResult.PlayerWin or GameResult.DealerBust => Get("player_win", lang),
            GameResult.DealerWin or GameResult.PlayerBust => Get("dealer_win", lang),
            GameResult.Push => Get("push", lang),
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
        
        int netResult = game.GetNetResult();
        
        if (netResult > 0)
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), Get("net_winnings", lang, netResult));
        }
        else if (netResult < 0)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), Get("losses", lang, netResult));
        }
        else
        {
            ImGui.Text(Get("bet_recovered", lang));
        }
        
        ImGui.Spacing();
        
        if (ImGui.Button(Get("new_game", lang)))
        {
            // Appliquer le résultat financier de la partie
            ApplyGameResult();
            
            // Réinitialiser le jeu
            game = new BlackjackGame();
            game.OnGameEvent += AddToLog;
            game.GameLanguage = lang;
            gameLog.Clear();
        }
    }

    private void DrawGameLog()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        if (ImGui.CollapsingHeader(Get("game_history", lang)))
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

    private void ApplyGameResult()
    {
        var currentSave = plugin.Configuration.CurrentSave!;
        
        // Déduire la mise au début de la partie
        int newMoney = currentSave.PlayerMoney - game.CurrentBet;
        
        // Ajouter les gains totaux
        int winnings = game.GetWinnings();
        newMoney += winnings;
        
        // S'assurer que l'argent ne devient pas négatif
        if (newMoney < 0) newMoney = 0;
        
        // Mettre à jour l'argent
        plugin.Configuration.UpdatePlayerMoney(newMoney);
        
        // Calculer le résultat net pour les statistiques
        int netResult = winnings - game.CurrentBet;
        
        // Mettre à jour les statistiques
        bool won = game.Result == GameResult.PlayerWin || 
                   game.Result == GameResult.PlayerBlackjack || 
                   game.Result == GameResult.DealerBust;
        bool blackjack = game.Result == GameResult.PlayerBlackjack;
        
        plugin.Configuration.UpdateStats(won, netResult, blackjack);
    }
}
