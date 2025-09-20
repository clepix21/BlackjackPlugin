using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using BlackjackPlugin.Localization;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin.Windows;

// Fenêtre de configuration principale du plugin
public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration; // Référence à la configuration du plugin
    private string[] newSaveNames = new string[3]; // Champs pour les noms des nouvelles sauvegardes
    private Language lastLanguage; // Pour détecter les changements de langue

    // Constructeur : initialise la fenêtre et les champs
    public ConfigWindow(Plugin plugin) : base(
        Get("config_title", plugin.Configuration.CurrentLanguage))
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(450, 400);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
        lastLanguage = configuration.CurrentLanguage;
        
        // Initialiser les noms de sauvegarde à vide
        for (int i = 0; i < newSaveNames.Length; i++)
        {
            newSaveNames[i] = "";
        }
    }

    public void Dispose() { }

    // Méthode principale d'affichage de la fenêtre
    public override void Draw()
    {
        var lang = configuration.CurrentLanguage;
        
        // Met à jour le titre si la langue a changé et que la fenêtre n'est pas active
        if (lastLanguage != lang && !ImGui.IsWindowFocused())
        {
            WindowName = Get("config_title", lang);
            lastLanguage = lang;
        }
        
        DrawSaveManagement(); // Gestion des sauvegardes
        ImGui.Separator();
        
        DrawGameOptions(); // Options de jeu
        ImGui.Separator();
        
        DrawLanguageSettings(); // Paramètres de langue
    }

    // Affiche et gère les emplacements de sauvegarde
    private void DrawSaveManagement()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("save_management", lang));
        ImGui.Separator();
        
        // Affiche les 3 slots de sauvegarde
        for (int i = 0; i < configuration.SaveSlots.Length; i++)
        {
            var slot = configuration.SaveSlots[i];
            var isCurrentSlot = configuration.CurrentSaveSlot == i;
            
            using var id = ImRaii.PushId(i);
            
            // Met en surbrillance le slot sélectionné
            using var bgColor = ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.4f, 0.2f, 0.3f), isCurrentSlot);
            using var child = ImRaii.Child($"slot_{i}", new Vector2(0, 80), true);
            
            if (!child) continue;
            
            ImGui.Text($"{Get("slot", lang)} {i + 1}:");
            ImGui.SameLine();
            
            if (slot.IsEmpty)
            {
                // Si le slot est vide, propose la création d'une sauvegarde
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), Get("empty", lang));
                
                ImGui.SetNextItemWidth(200);
                ImGui.InputText($"##name_{i}", ref newSaveNames[i], 50);
                ImGui.SameLine();
                
                if (ImGui.Button(Get("create_save", lang)))
                {
                    // Utilise un nom par défaut si aucun n'est fourni
                    string saveName = string.IsNullOrWhiteSpace(newSaveNames[i]) 
                        ? $"Slot {i + 1}" 
                        : newSaveNames[i].Trim();
                    
                    configuration.CreateSave(i, saveName);
                    newSaveNames[i] = ""; // Réinitialise le champ après création
                }
            }
            else
            {
                // Affiche les infos de la sauvegarde existante
                ImGui.Text($"{slot.Name}");
                
                // Statistiques du joueur
                string statsLine1 = $"{Get("money", lang)}: {slot.PlayerMoney} Gil";
                string statsLine2 = "";
                
                if (slot.GamesPlayed > 0)
                {
                    double winRate = (double)slot.GamesWon / slot.GamesPlayed * 100.0;
                    statsLine2 = $"{Get("games_played", lang)}: {slot.GamesPlayed} | " +
                                $"{Get("win_rate", lang)}: {winRate:F1}% | " +
                                $"{Get("blackjacks", lang)}: {slot.BlackjacksHit}";
                }
                else
                {
                    statsLine2 = $"{Get("games_played", lang)}: 0 | {Get("win_rate", lang)}: 0.0%";
                }
                
                ImGui.Text(statsLine1);
                ImGui.Text(statsLine2);
                
                ImGui.Text($"{Get("created", lang)}: {slot.CreatedDate:dd/MM/yyyy} | " +
                          $"{Get("last_played", lang)}: {slot.LastPlayed:dd/MM/yyyy}");
                
                // Boutons d'action pour la sauvegarde
                if (!isCurrentSlot && ImGui.Button(Get("load_save", lang)))
                {
                    configuration.SelectSave(i);
                }
                
                if (!isCurrentSlot) ImGui.SameLine();
                
                if (ImGui.Button(Get("reset_save", lang)))
                {
                    slot.Reset();
                    configuration.Save();
                }
                
                ImGui.SameLine();
                
                if (ImGui.Button(Get("delete_save", lang)))
                {
                    configuration.DeleteSave(i);
                }
            }
        }
    }

    // Affiche les options de jeu (ex : mise par défaut)
    private void DrawGameOptions()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("game_options", lang));
        ImGui.Separator();

        var defaultBet = configuration.DefaultBet;
        if (ImGui.InputInt(Get("default_bet", lang), ref defaultBet))
        {
            configuration.DefaultBet = Math.Max(10, defaultBet);
            // Sauvegarde automatique lors du changement
            configuration.Save();
        }
    }

    // Affiche les paramètres de langue
    private void DrawLanguageSettings()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("language_settings", lang));
        ImGui.Separator();
        
        var useFrench = configuration.CurrentLanguage == Language.French;
        if (ImGui.Checkbox(Get("use_french", lang), ref useFrench))
        {
            configuration.CurrentLanguage = useFrench ? Language.French : Language.English;
            // Sauvegarde automatique lors du changement de langue
            configuration.Save();
            // Ne pas mettre à jour lastLanguage ici pour éviter le changement de titre immédiat
        }
    }
}
