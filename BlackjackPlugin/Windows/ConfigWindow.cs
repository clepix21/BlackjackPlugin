using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BlackjackPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    public ConfigWindow(Plugin plugin) : base("⚙️ Configuration Blackjack")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(350, 250);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("💰 Gestion de l'argent");
        ImGui.Separator();
        
        var playerMoney = configuration.PlayerMoney;
        if (ImGui.InputInt("Argent du joueur", ref playerMoney))
        {
            configuration.PlayerMoney = Math.Max(0, playerMoney);
        }

        var defaultBet = configuration.DefaultBet;
        if (ImGui.InputInt("Mise par défaut", ref defaultBet))
        {
            configuration.DefaultBet = Math.Max(10, defaultBet);
        }
        
        ImGui.Spacing();
        ImGui.Text("🎮 Options de jeu");
        ImGui.Separator();

        var soundEnabled = configuration.SoundEnabled;
        if (ImGui.Checkbox("Sons activés", ref soundEnabled))
        {
            configuration.SoundEnabled = soundEnabled;
        }

        var showAnimations = configuration.ShowAnimations;
        if (ImGui.Checkbox("Animations activées", ref showAnimations))
        {
            configuration.ShowAnimations = showAnimations;
        }
        
        ImGui.Spacing();
        
        if (ImGui.Button("💾 Sauvegarder"))
        {
            configuration.Save();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button("🔄 Réinitialiser"))
        {
            configuration.PlayerMoney = 1000;
            configuration.DefaultBet = 50;
            configuration.SoundEnabled = true;
            configuration.ShowAnimations = true;
            configuration.Save();
        }
    }
}
