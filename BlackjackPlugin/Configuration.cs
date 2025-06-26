using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Text.Json.Serialization;
using BlackjackPlugin.Localization;
using BlackjackPlugin.SaveSystem;

namespace BlackjackPlugin;

/// <summary>
/// Configuration du plugin Blackjack avec système de sauvegarde
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    /// <inheritdoc/>
    public int Version { get; set; } = 1;

    // Système de sauvegarde : 3 emplacements de sauvegarde, index du slot courant
    public SaveSlot[] SaveSlots { get; set; } = new SaveSlot[3];
    public int CurrentSaveSlot { get; set; } = -1; // -1 = aucun slot sélectionné
    
    // Paramètres de jeu : mises par défaut, min et max
    public int DefaultBet { get; set; } = 50;
    public int MinBet { get; set; } = 10;
    public int MaxBet { get; set; } = 500;
    
    // Options d'interface : autosave
    public bool AutoSave { get; set; } = true;
    
    // Paramètres de langue
    public Language CurrentLanguage { get; set; } = Language.English;
    
    // Paramètres visuels : opacité, thème
    public float WindowOpacity { get; set; } = 1.0f;
    public bool UseCustomTheme { get; set; } = false;
    public string ThemeName { get; set; } = "Default";
    
    // Paramètres avancés : probabilités, raccourcis, historique
    public bool ShowProbabilities { get; set; } = false;
    public bool EnableHotkeys { get; set; } = true;
    public bool LogGameHistory { get; set; } = false;
    
    // Paramètres de sécurité : première utilisation
    public bool FirstTimeUser { get; set; } = true;

    // Interface Dalamud (non sérialisée)
    [JsonIgnore]
    private IDalamudPluginInterface? pluginInterface;

    /// <summary>
    /// Constructeur : initialise les slots de sauvegarde
    /// </summary>
    public Configuration()
    {
        // Initialiser les emplacements de sauvegarde
        for (int i = 0; i < SaveSlots.Length; i++)
        {
            SaveSlots[i] = new SaveSlot();
        }
    }

    /// <summary>
    /// Obtient la sauvegarde actuellement active
    /// </summary>
    [JsonIgnore]
    public SaveSlot? CurrentSave => CurrentSaveSlot >= 0 && CurrentSaveSlot < SaveSlots.Length ? SaveSlots[CurrentSaveSlot] : null;
    
    /// <summary>
    /// Obtient l'argent du joueur actuel
    /// </summary>
    [JsonIgnore]
    public int PlayerMoney => CurrentSave?.PlayerMoney ?? 0;

    /// <summary>
    /// Initialise la configuration avec l'interface du plugin
    /// </summary>
    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
        
        // S'assurer que les slots sont initialisés
        if (SaveSlots == null || SaveSlots.Length != 3)
        {
            SaveSlots = new SaveSlot[3];
            for (int i = 0; i < SaveSlots.Length; i++)
            {
                SaveSlots[i] = new SaveSlot();
            }
        }
    }

    /// <summary>
    /// Sauvegarde la configuration sur le disque
    /// </summary>
    public void Save()
    {
        pluginInterface?.SavePluginConfig(this);
    }

    /// <summary>
    /// Crée une nouvelle sauvegarde dans l'emplacement spécifié
    /// </summary>
    /// <param name="slotIndex">Index du slot</param>
    /// <param name="name">Nom de la sauvegarde</param>
    public void CreateSave(int slotIndex, string name)
    {
        if (slotIndex >= 0 && slotIndex < SaveSlots.Length)
        {
            SaveSlots[slotIndex] = new SaveSlot(name);
            CurrentSaveSlot = slotIndex;
            Save();
        }
    }
    
    /// <summary>
    /// Supprime la sauvegarde dans l'emplacement spécifié
    /// </summary>
    /// <param name="slotIndex">Index du slot</param>
    public void DeleteSave(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < SaveSlots.Length)
        {
            SaveSlots[slotIndex].Clear();
            if (CurrentSaveSlot == slotIndex)
            {
                CurrentSaveSlot = -1;
            }
            Save();
        }
    }
    
    /// <summary>
    /// Sélectionne une sauvegarde existante
    /// </summary>
    /// <param name="slotIndex">Index du slot</param>
    public void SelectSave(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < SaveSlots.Length && !SaveSlots[slotIndex].IsEmpty)
        {
            CurrentSaveSlot = slotIndex;
            Save();
        }
    }
    
    /// <summary>
    /// Met à jour l'argent du joueur actuel
    /// </summary>
    /// <param name="newAmount">Nouveau montant</param>
    public void UpdatePlayerMoney(int newAmount)
    {
        if (CurrentSave != null)
        {
            CurrentSave.PlayerMoney = Math.Max(0, newAmount);
            if (AutoSave) Save();
        }
    }
    
    /// <summary>
    /// Met à jour les statistiques de la sauvegarde actuelle
    /// </summary>
    /// <param name="won">Victoire ?</param>
    /// <param name="winnings">Gains</param>
    /// <param name="blackjack">Blackjack ?</param>
    public void UpdateStats(bool won, int winnings, bool blackjack = false)
    {
        if (CurrentSave != null)
        {
            CurrentSave.UpdateStats(won, winnings, blackjack);
            if (AutoSave) Save();
        }
    }

    /// <summary>
    /// Remet la configuration aux valeurs par défaut (hors sauvegardes)
    /// </summary>
    public void Reset()
    {
        DefaultBet = 50;
        MinBet = 10;
        MaxBet = 500;
        AutoSave = true;
        CurrentLanguage = Language.English;
        WindowOpacity = 1.0f;
        UseCustomTheme = false;
        ThemeName = "Default";
        ShowProbabilities = false;
        EnableHotkeys = true;
        LogGameHistory = false;
        FirstTimeUser = true;
        
        Save();
    }
}
