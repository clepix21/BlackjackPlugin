using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using BlackjackPlugin.Windows;
using BlackjackPlugin.Localization;
using System;
using static BlackjackPlugin.Localization.Localization;

namespace BlackjackPlugin;

/// <summary>
/// Plugin principal pour le jeu de Blackjack dans Final Fantasy XIV
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    /// <inheritdoc/>
    public string Name => "Blackjack Casino";
    
    private const string CommandName = "/blackjack";
    private const string CommandAlias = "/bj";

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly IPluginLog pluginLog;

    public Configuration Configuration { get; }
    public readonly WindowSystem WindowSystem = new("BlackjackPlugin");

    private ConfigWindow ConfigWindow { get; }
    private MainWindow MainWindow { get; }

    /// <summary>
    /// Initialise une nouvelle instance du plugin Blackjack
    /// </summary>
    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IPluginLog pluginLog)
    {
        this.pluginInterface = pluginInterface;
        this.commandManager = commandManager;
        this.pluginLog = pluginLog;

        // Charger et initialiser la configuration
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);

        // S'assurer que les slots de sauvegarde sont correctement initialisés
        if (Configuration.SaveSlots == null || Configuration.SaveSlots.Length != 3)
        {
            Configuration.SaveSlots = new SaveSystem.SaveSlot[3];
            for (int i = 0; i < Configuration.SaveSlots.Length; i++)
            {
                Configuration.SaveSlots[i] = new SaveSystem.SaveSlot();
            }
            Configuration.Save();
        }

        // Créer les fenêtres de configuration et principale
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        // Enregistrer les commandes du plugin
        commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = Get("cmd_open_game", Configuration.CurrentLanguage)
        });
        
        commandManager.AddHandler(CommandAlias, new CommandInfo(OnCommand)
        {
            HelpMessage = "Alias for /blackjack"
        });

        // Enregistrer les événements UI
        pluginInterface.UiBuilder.Draw += DrawUI;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Message de chargement dans le log
        pluginLog.Information(Get("plugin_loaded", Configuration.CurrentLanguage));
        
        // Si c'est la première utilisation, ouvrir automatiquement la configuration
        if (Configuration.FirstTimeUser)
        {
            Configuration.FirstTimeUser = false;
            Configuration.Save();
            
            // Ouvrir la config après un petit délai pour laisser le temps au plugin de se charger
            System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ => 
            {
                ToggleConfigUI();
            });
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Nettoyer toutes les fenêtres
        WindowSystem.RemoveAllWindows();
        
        ConfigWindow.Dispose();
        MainWindow.Dispose();
        
        // Retirer les handlers de commande
        commandManager.RemoveHandler(CommandName);
        commandManager.RemoveHandler(CommandAlias);
        
        // Désabonner les événements UI
        pluginInterface.UiBuilder.Draw -= DrawUI;
        pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        // Message de déchargement dans le log
        pluginLog.Information(Get("plugin_unloaded", Configuration.CurrentLanguage));
    }

    /// <summary>
    /// Handler principal pour les commandes du plugin
    /// </summary>
    private void OnCommand(string command, string args)
    {
        var arguments = args.Trim().ToLowerInvariant();
        var lang = Configuration.CurrentLanguage;
        
        switch (arguments)
        {
            case "":
                // Ouvre la fenêtre principale du jeu
                ToggleMainUI();
                break;
            case "config":
            case "settings":
                // Ouvre la fenêtre de configuration
                ToggleConfigUI();
                break;
            case "help":
                // Affiche l'aide des commandes
                pluginLog.Information(Get("available_commands", lang));
                pluginLog.Information(Get("cmd_open_game", lang));
                pluginLog.Information(Get("cmd_open_config", lang));
                pluginLog.Information(Get("cmd_help", lang));
                pluginLog.Information("/blackjack create - Creates a quick save");
                pluginLog.Information("/blackjack create [name] - Creates a save with custom name");
                pluginLog.Information("/blackjack reset - Resets current save money to 1000");
                break;
            case "reset":
                // Réinitialiser la sauvegarde actuelle si elle existe
                if (Configuration.CurrentSave != null)
                {
                    Configuration.UpdatePlayerMoney(1000);
                    pluginLog.Information(Get("money_reset", lang));
                }
                else
                {
                    pluginLog.Warning("No save selected. Please create or select a save first.");
                }
                break;
            case "create":
                // Créer une sauvegarde rapide dans le premier slot libre
                CreateQuickSave();
                break;
            case "saves":
            case "list":
                // Lister toutes les sauvegardes
                ListSaves();
                break;
            default:
                // Vérifier si c'est une commande de création avec nom
                if (arguments.StartsWith("create "))
                {
                    var saveName = args.Substring(7).Trim(); // Enlever "create "
                    CreateQuickSave(saveName);
                }
                else if (arguments.StartsWith("load "))
                {
                    var slotStr = args.Substring(5).Trim(); // Enlever "load "
                    if (int.TryParse(slotStr, out int slot) && slot >= 1 && slot <= 3)
                    {
                        LoadSave(slot - 1); // Convertir en index 0-based
                    }
                    else
                    {
                        pluginLog.Warning("Invalid slot number. Use 1, 2, or 3.");
                    }
                }
                else
                {
                    pluginLog.Warning(Get("unknown_command", lang, arguments));
                }
                break;
        }
    }

    /// <summary>
    /// Crée une sauvegarde rapide dans le premier slot libre, ou avec un nom personnalisé
    /// </summary>
    private void CreateQuickSave(string customName = "")
    {
        var lang = Configuration.CurrentLanguage;
        
        // Trouver le premier slot libre
        for (int i = 0; i < Configuration.SaveSlots.Length; i++)
        {
            if (Configuration.SaveSlots[i].IsEmpty)
            {
                string saveName = string.IsNullOrWhiteSpace(customName) 
                    ? $"Slot {i + 1}" 
                    : customName;
                
                Configuration.CreateSave(i, saveName);
                pluginLog.Information($"Save '{saveName}' created in slot {i + 1} and loaded!");
                return;
            }
        }
        
        pluginLog.Warning("All save slots are full. Please delete a save first or use '/blackjack config' to manage saves.");
    }

    /// <summary>
    /// Charge une sauvegarde à partir de l'index de slot donné
    /// </summary>
    private void LoadSave(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < Configuration.SaveSlots.Length)
        {
            var slot = Configuration.SaveSlots[slotIndex];
            if (!slot.IsEmpty)
            {
                Configuration.SelectSave(slotIndex);
                pluginLog.Information($"Loaded save '{slot.Name}' from slot {slotIndex + 1}!");
            }
            else
            {
                pluginLog.Warning($"Slot {slotIndex + 1} is empty. Create a save first.");
            }
        }
        else
        {
            pluginLog.Warning("Invalid slot number. Use 1, 2, or 3.");
        }
    }

    /// <summary>
    /// Liste toutes les sauvegardes existantes dans le log
    /// </summary>
    private void ListSaves()
    {
        var lang = Configuration.CurrentLanguage;
        pluginLog.Information("=== Save Slots ===");
        
        for (int i = 0; i < Configuration.SaveSlots.Length; i++)
        {
            var slot = Configuration.SaveSlots[i];
            var isActive = Configuration.CurrentSaveSlot == i;
            var activeMarker = isActive ? " [ACTIVE]" : "";
            
            if (slot.IsEmpty)
            {
                pluginLog.Information($"Slot {i + 1}: Empty{activeMarker}");
            }
            else
            {
                pluginLog.Information($"Slot {i + 1}: {slot.Name} - {slot.PlayerMoney} Gil - {slot.GamesPlayed} games - {slot.WinPercentage:F1}% win rate{activeMarker}");
            }
        }
    }

    /// <summary>
    /// Dessine l'UI du plugin
    /// </summary>
    private void DrawUI() => WindowSystem.Draw();

    /// <summary>
    /// Ouvre ou ferme la fenêtre de configuration
    /// </summary>
    public void ToggleConfigUI() => ConfigWindow.Toggle();

    /// <summary>
    /// Ouvre ou ferme la fenêtre principale du jeu
    /// </summary>
    public void ToggleMainUI() => MainWindow.Toggle();
}
