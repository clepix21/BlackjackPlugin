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

    public ConfigWindow(Plugin plugin) : base(
        Get("config_title", plugin.Configuration.CurrentLanguage))
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(350, 300);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var lang = configuration.CurrentLanguage;
        
        // Mise à jour du titre de la fenêtre si la langue a changé
        WindowName = Get("config_title", lang);
        
        ImGui.Text(Get("money_management", lang));
        ImGui.Separator();
        
        var playerMoney = configuration.PlayerMoney;
        if (ImGui.InputInt(Get("player_money", lang), ref playerMoney))
        {
            configuration.PlayerMoney = Math.Max(0, playerMoney);
        }

        var defaultBet = configuration.DefaultBet;
        if (ImGui.InputInt(Get("default_bet", lang), ref defaultBet))
        {
            configuration.DefaultBet = Math.Max(10, defaultBet);
        }
        
        ImGui.Spacing();
        ImGui.Text(Get("game_options", lang));
        ImGui.Separator();

        var soundEnabled = configuration.SoundEnabled;
        if (ImGui.Checkbox(Get("sounds_enabled", lang), ref soundEnabled))
        {
            configuration.SoundEnabled = soundEnabled;
        }

        var showAnimations = configuration.ShowAnimations;
        if (ImGui.Checkbox(Get("animations_enabled", lang), ref showAnimations))
        {
            configuration.ShowAnimations = showAnimations;
        }
        
        ImGui.Spacing();
        ImGui.Text(Get("language_settings", lang));
        ImGui.Separator();
        
        var useFrench = configuration.CurrentLanguage == Language.French;
        if (ImGui.Checkbox(Get("use_french", lang), ref useFrench))
        {
            configuration.CurrentLanguage = useFrench ? Language.French : Language.English;
        }
        
        ImGui.Spacing();
        
        if (ImGui.Button(Get("save", lang)))
        {
            configuration.Save();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button(Get("reset", lang)))
        {
            configuration.PlayerMoney = 1000;
            configuration.DefaultBet = 50;
            configuration.SoundEnabled = true;
            configuration.ShowAnimations = true;
            // Ne pas réinitialiser la langue
            configuration.Save();
        }
    }
}
