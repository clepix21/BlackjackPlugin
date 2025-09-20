using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using DImGui = Dalamud.Bindings.ImGui;
using BlackjackPlugin.GameLogic;
using System.Collections.Generic;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin.Windows;

// Fenêtre principale du plugin Blackjack
public class MainWindow : Window, IDisposable
{
    private Plugin plugin; // Référence au plugin principal
    private BlackjackGame game; // Instance du jeu de blackjack
    private int betAmount = 50; // Montant de la mise actuelle
    private List<string> gameLog; // Historique des événements du jeu
    private const int MaxLogEntries = 10; // Nombre max d'entrées dans le log
    private int moneyBeforeGame = 0; // Argent avant le début de la partie

    // Constructeur de la fenêtre principale
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

        // S'abonner aux événements du jeu pour le log
        game.OnGameEvent += AddToLog;
        
        // Synchroniser la langue du jeu
        game.GameLanguage = plugin.Configuration.CurrentLanguage;
    }

    // Désabonnement lors de la destruction
    public void Dispose()
    {
        game.OnGameEvent -= AddToLog;
    }

    // Ajoute un message à l'historique du jeu
    private void AddToLog(string message)
    {
        gameLog.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (gameLog.Count > MaxLogEntries)
        {
            gameLog.RemoveAt(0);
        }
    }

    // Méthode principale d'affichage de la fenêtre
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

    // Affiche un message si aucune sauvegarde n'est sélectionnée
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

    // Affiche l'en-tête avec l'argent, la sauvegarde et la mise
    private void DrawHeader()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        var currentSave = plugin.Configuration.CurrentSave!;
        
        // Calculer l'argent disponible pendant la partie
        int availableMoney = game.State == GameState.WaitingForBet 
            ? currentSave.PlayerMoney 
            : moneyBeforeGame - game.CurrentBet;
        
    // Style du header
    var colOr = DImGui.ImGui.GetColorU32(new Vector4(1.0f, 0.84f, 0.0f, 1.0f));
    using var textColor = ImRaii.PushColor(DImGui.ImGuiCol.Text, colOr); // Or
    ImGui.Text($"{Get("money", lang)}: {availableMoney} Gil");
    textColor.Pop();
        
        ImGui.SameLine();
        ImGui.Text($"{Get("current_save", lang)}: {currentSave.Name}");
        
        ImGui.SameLine();
        if (game.CurrentBet > 0)
        {
            ImGui.Text($"{Get("bet", lang)}: {game.CurrentBet} Gil");
        }
    }

    // Affiche la zone de jeu (main du croupier et du joueur)
    private void DrawGameArea()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        // Zone du croupier
        var colDealer = DImGui.ImGui.GetColorU32(new Vector4(0.8f, 0.2f, 0.2f, 1.0f));
        using (var dealerColor = ImRaii.PushColor(DImGui.ImGuiCol.Text, colDealer)) // Rouge
        {
            ImGui.Text(Get("dealer", lang));
        }
        
        using (var indent = ImRaii.PushIndent())
        {
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
        }
        
        ImGui.Spacing();
        ImGui.Spacing();

        // Zone du joueur
        var colPlayer = DImGui.ImGui.GetColorU32(new Vector4(0.2f, 0.8f, 0.2f, 1.0f));
        using (var playerColor = ImRaii.PushColor(DImGui.ImGuiCol.Text, colPlayer)) // Vert
        {
            ImGui.Text(Get("your_hand", lang));
        }
        
        using (var indent = ImRaii.PushIndent())
        {
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
        }
    }

    // Affiche une main de cartes (joueur ou croupier)
    private void DrawHand(List<Card> hand, bool hideSecondCard)
    {
        if (hand.Count == 0) return;

        for (int i = 0; i < hand.Count; i++)
        {
            if (i > 0) ImGui.SameLine();
            
            if (i == 1 && hideSecondCard)
            {
                // Carte cachée
                var colHidden = DImGui.ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                var colHiddenHover = DImGui.ImGui.GetColorU32(new Vector4(0.4f, 0.4f, 0.4f, 1.0f));
                using var hiddenCardStyle = ImRaii.PushColor(DImGui.ImGuiCol.Button, colHidden)
                    .Push(DImGui.ImGuiCol.ButtonHovered, colHiddenHover);
                ImGui.Button("🂠");
            }
            else
            {
                // Carte visible avec couleur
                var card = hand[i];
                var color = card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds 
                    ? new Vector4(0.8f, 0.2f, 0.2f, 1.0f) 
                    : new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
                
                var colButton = DImGui.ImGui.GetColorU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                var colText = DImGui.ImGui.GetColorU32(color);
                using var cardStyle = ImRaii.PushColor(DImGui.ImGuiCol.Button, colButton)
                    .Push(DImGui.ImGuiCol.Text, colText);
                ImGui.Button($" {card.GetDisplayName()} ");
            }
        }
    }

    // Affiche les contrôles selon l'état du jeu
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

    // Contrôles pour placer une mise
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
            // Message si plus d'argent
            ImGui.TextColored(new Vector4(1, 0, 0, 1), Get("no_money", lang));
            if (ImGui.Button(Get("reset_money", lang)))
            {
                plugin.Configuration.UpdatePlayerMoney(1000);
            }
        }
        else if (ImGui.Button(Get("deal_cards", lang)) && betAmount <= currentSave.PlayerMoney)
        {
            // Sauvegarder l'argent avant la partie
            moneyBeforeGame = currentSave.PlayerMoney;
            game.StartNewGame(betAmount);
        }
    }

    // Contrôles pendant le tour du joueur
    private void DrawPlayerTurnControls()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        // Calculer l'argent disponible (argent avant la partie - mise actuelle)
        int availableMoney = moneyBeforeGame - game.CurrentBet;
        
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
        
        // Double Down si possible ET si le joueur a assez d'argent pour doubler la mise
        if (game.CanDoubleDown() && availableMoney >= game.CurrentBet)
        {
            ImGui.SameLine();
            if (ImGui.Button(Get("double", lang)))
            {
                game.DoubleDown();
            }
        }
    }

    // Contrôles et affichage de fin de partie
    private void DrawGameOverControls()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        // Texte du résultat
        string resultText = game.Result switch
        {
            GameResult.PlayerBlackjack => Get("blackjack_win", lang),
            GameResult.PlayerWin or GameResult.DealerBust => Get("player_win", lang),
            GameResult.DealerWin or GameResult.PlayerBust => Get("dealer_win", lang),
            GameResult.Push => Get("push", lang),
            _ => ""
        };
        
        // Couleur selon le résultat
        Vector4 color = game.Result switch
        {
            GameResult.PlayerBlackjack or GameResult.PlayerWin or GameResult.DealerBust => new Vector4(0, 1, 0, 1),
            GameResult.DealerWin or GameResult.PlayerBust => new Vector4(1, 0, 0, 1),
            GameResult.Push => new Vector4(1, 1, 0, 1),
            _ => new Vector4(1, 1, 1, 1)
        };
        
        ImGui.TextColored(color, resultText);
        
        int netResult = game.GetNetResult();
        
        // Affichage du gain ou de la perte
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
        
        // Bouton pour recommencer une partie
        if (ImGui.Button(Get("new_game", lang)))
        {
            // Appliquer le résultat financier de la partie
            ApplyGameResult();
            
            // Réinitialiser le jeu
            game = new BlackjackGame();
            game.OnGameEvent += AddToLog;
            game.GameLanguage = lang;
            gameLog.Clear();
            moneyBeforeGame = 0; // Reset
        }
    }

    // Affiche l'historique des événements du jeu
    private void DrawGameLog()
    {
        var lang = plugin.Configuration.CurrentLanguage;
        
        if (ImGui.CollapsingHeader(Get("game_history", lang)))
        {
            using var child = ImRaii.Child("GameLog", new Vector2(0, 100), true);
            if (child)
            {
                foreach (var entry in gameLog)
                {
                    ImGui.TextWrapped(entry);
                }
                
                // Auto-scroll vers le bas
                if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                    ImGui.SetScrollHereY(1.0f);
            }
        }
    }

    // Applique le résultat financier et met à jour les stats
    private void ApplyGameResult()
    {
        var currentSave = plugin.Configuration.CurrentSave!;
        
        // Utiliser l'argent sauvegardé avant la partie
        int newMoney = moneyBeforeGame;
        
        // Déduire la mise totale (qui peut avoir été doublée)
        newMoney -= game.CurrentBet;
        
        // Ajouter les gains
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
