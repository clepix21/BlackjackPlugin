using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using BlackjackPlugin.Localization;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;
    private string[] newSaveNames = new string[3]; // Un nom par slot

    public ConfigWindow(Plugin plugin) : base(
        Get("config_title", plugin.Configuration.CurrentLanguage))
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(450, 450); // Réduit la hauteur car moins d'options
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
        
        // Initialiser les noms de sauvegarde
        for (int i = 0; i < newSaveNames.Length; i++)
        {
            newSaveNames[i] = "";
        }
    }

    public void Dispose() { }

    public override void Draw()
    {
        var lang = configuration.CurrentLanguage;
        
        // Mise à jour du titre de la fenêtre si la langue a changé
        WindowName = Get("config_title", lang);
        
        DrawSaveManagement();
        ImGui.Separator();
        
        DrawGameOptions();
        ImGui.Separator();
        
        DrawLanguageSettings();
        ImGui.Separator();
        
        DrawButtons();
    }

    private void DrawSaveManagement()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("save_management", lang));
        ImGui.Separator();
        
        // Afficher les 3 emplacements de sauvegarde
        for (int i = 0; i < configuration.SaveSlots.Length; i++)
        {
            var slot = configuration.SaveSlots[i];
            var isCurrentSlot = configuration.CurrentSaveSlot == i;
            
            ImGui.PushID(i);
            
            // Couleur de fond pour le slot actuel
            if (isCurrentSlot)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.4f, 0.2f, 0.3f));
            }
            
            ImGui.BeginChild($"slot_{i}", new Vector2(0, 80), true);
            
            ImGui.Text($"{Get("slot", lang)} {i + 1}:");
            ImGui.SameLine();
            
            if (slot.IsEmpty)
            {
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
                ImGui.Text($"{slot.Name}");
                
                ImGui.Text($"{Get("money", lang)}: {slot.PlayerMoney} Gil | " +
                          $"{Get("games_played", lang)}: {slot.GamesPlayed} | " +
                          $"{Get("win_rate", lang)}: {slot.WinPercentage:F1}%");
                
                ImGui.Text($"{Get("created", lang)}: {slot.CreatedDate:dd/MM/yyyy} | " +
                          $"{Get("last_played", lang)}: {slot.LastPlayed:dd/MM/yyyy}");
                
                // Boutons d'action
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

    private void DrawGameOptions()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("game_options", lang));
        ImGui.Separator();

        var defaultBet = configuration.DefaultBet;
        if (ImGui.InputInt(Get("default_bet", lang), ref defaultBet))
        {
            configuration.DefaultBet = Math.Max(10, defaultBet);
        }
    }

    private void DrawLanguageSettings()
    {
        var lang = configuration.CurrentLanguage;
        
        ImGui.Text(Get("language_settings", lang));
        ImGui.Separator();
        
        var useFrench = configuration.CurrentLanguage == Language.French;
        if (ImGui.Checkbox(Get("use_french", lang), ref useFrench))
        {
            configuration.CurrentLanguage = useFrench ? Language.French : Language.English;
        }
    }

    private void DrawButtons()
    {
        var lang = configuration.CurrentLanguage;
        
        if (ImGui.Button(Get("save", lang)))
        {
            configuration.Save();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button(Get("reset", lang)))
        {
            configuration.Reset();
        }
    }
}
