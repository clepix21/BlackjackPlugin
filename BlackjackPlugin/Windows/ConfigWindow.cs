using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using BlackjackPlugin.Localization;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin.Windows;

// Fenêtre de configuration principale du plugin Blackjack
public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration; // Référence à la configuration du plugin
    private string[] newSaveNames = new string[3]; // Un nom par slot de sauvegarde
    private Language lastLanguage; // Pour détecter les changements de langue

    // Constructeur : initialise la fenêtre avec le titre localisé
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
        
        // Met à jour le titre si la langue a changé et que la fenêtre n'est pas en focus
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

    // Affiche et gère les slots de sauvegarde
    private void DrawSaveManagement()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("save_management", lang));
        ImGui.Separator();
        
        // Affiche les 3 emplacements de sauvegarde
        for (int i = 0; i < configuration.SaveSlots.Length; i++)
        {
            var slot = configuration.SaveSlots[i];
            var isCurrentSlot = configuration.CurrentSaveSlot == i;
            
            ImGui.PushID(i);
            
            // Met en surbrillance le slot actuel
            if (isCurrentSlot)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.4f, 0.2f, 0.3f));
            }
            
            ImGui.BeginChild($"slot_{i}", new Vector2(0, 80), true);
            
            ImGui.Text($"{Get("slot", lang)} {i + 1}:");
            ImGui.SameLine();
            
            if (slot.IsEmpty)
            {
                // Slot vide : permet de créer une nouvelle sauvegarde
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), Get("empty", lang));
                
                ImGui.SetNextItemWidth(200);
                ImGui.InputText($"##name_{i}", ref newSaveNames[i], 50);
                ImGui.SameLine();
                
                if (ImGui.Button(Get("create_save", lang)))
                {
                    // Si aucun nom n'est fourni, utiliser un nom par défaut
                    string saveName = string.IsNullOrWhiteSpace(newSaveNames[i]) 
                        ? $"Slot {i + 1}" 
                        : newSaveNames[i].Trim();
                    
                    configuration.CreateSave(i, saveName);
                    newSaveNames[i] = ""; // Vider le champ après création
                }
            }
            else
            {
                // Slot existant : affiche les infos et les boutons d'action
                ImGui.Text($"{slot.Name}");
                
                // Affichage des statistiques
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
                
                // Boutons pour charger, réinitialiser ou supprimer la sauvegarde
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
            
            ImGui.EndChild();
            
            if (isCurrentSlot)
            {
                ImGui.PopStyleColor();
            }
            
            ImGui.PopID();
        }
    }

    // Affiche et gère les options de jeu (ex : mise par défaut)
    private void DrawGameOptions()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("game_options", lang));
        ImGui.Separator();

        var defaultBet = configuration.DefaultBet;
        if (ImGui.InputInt(Get("default_bet", lang), ref defaultBet))
        {
            configuration.DefaultBet = Math.Max(10, defaultBet); // Mise minimale de 10
            configuration.Save(); // Sauvegarde automatique
        }
    }

    // Affiche et gère le choix de la langue
    private void DrawLanguageSettings()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("language_settings", lang));
        ImGui.Separator();
        
        var useFrench = configuration.CurrentLanguage == Language.French;
        if (ImGui.Checkbox(Get("use_french", lang), ref useFrench))
        {
            configuration.CurrentLanguage = useFrench ? Language.French : Language.English;
            configuration.Save(); // Sauvegarde automatique
            // Ne pas mettre à jour lastLanguage ici pour éviter le changement de titre immédiat
        }
    }
}
