using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Text.Json.Serialization;
using BlackjackPlugin.Localization;

namespace BlackjackPlugin;

/// <summary>
/// Configuration du plugin Blackjack avec toutes les options modernes
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    /// <inheritdoc/>
    public int Version { get; set; } = 1;

    // Paramètres de jeu
    public int PlayerMoney { get; set; } = 1000;
    public int DefaultBet { get; set; } = 50;
    public int MinBet { get; set; } = 10;
    public int MaxBet { get; set; } = 500;
    
    // Options d'interface
    public bool SoundEnabled { get; set; } = true;
    public bool ShowAnimations { get; set; } = true;
    public bool ShowTooltips { get; set; } = true;
    public bool AutoSave { get; set; } = true;
    
    // Paramètres de langue - Anglais par défaut
    public Language CurrentLanguage { get; set; } = Language.English;
    
    // Paramètres visuels
    public float WindowOpacity { get; set; } = 1.0f;
    public bool UseCustomTheme { get; set; } = false;
    public string ThemeName { get; set; } = "Default";
    
    // Statistiques
    public int GamesPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public int TotalWinnings { get; set; } = 0;
    public int BlackjacksHit { get; set; } = 0;
    
    // Paramètres avancés
    public bool ShowProbabilities { get; set; } = false;
    public bool EnableHotkeys { get; set; } = true;
    public bool LogGameHistory { get; set; } = false;
    
    // Paramètres de sécurité
    public DateTime LastPlayed { get; set; } = DateTime.MinValue;
    public bool FirstTimeUser { get; set; } = true;

    [JsonIgnore]
    private IDalamudPluginInterface? pluginInterface;

    /// <summary>
    /// Initialise la configuration avec l'interface du plugin
    /// </summary>
    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    /// <summary>
    /// Sauvegarde la configuration
    /// </summary>
    public void Save()
    {
        pluginInterface?.SavePluginConfig(this);
    }

    /// <summary>
    /// Remet la configuration aux valeurs par défaut
    /// </summary>
    public void Reset()
    {
        PlayerMoney = 1000;
        DefaultBet = 50;
        MinBet = 10;
        MaxBet = 500;
        SoundEnabled = true;
        ShowAnimations = true;
        ShowTooltips = true;
        AutoSave = true;
        CurrentLanguage = Language.English; // Anglais par défaut
        WindowOpacity = 1.0f;
        UseCustomTheme = false;
        ThemeName = "Default";
        ShowProbabilities = false;
        EnableHotkeys = true;
        LogGameHistory = false;
        FirstTimeUser = true;
        
        // Ne pas réinitialiser les statistiques
        Save();
    }

    /// <summary>
    /// Met à jour les statistiques après une partie
    /// </summary>
    public void UpdateStats(bool won, int winnings, bool blackjack = false)
    {
        GamesPlayed++;
        if (won) GamesWon++;
        TotalWinnings += winnings;
        if (blackjack) BlackjacksHit++;
        LastPlayed = DateTime.Now;
        
        if (AutoSave) Save();
    }

    /// <summary>
    /// Calcule le pourcentage de victoires
    /// </summary>
    public double WinPercentage => GamesPlayed > 0 ? (double)GamesWon / GamesPlayed * 100 : 0;
}
