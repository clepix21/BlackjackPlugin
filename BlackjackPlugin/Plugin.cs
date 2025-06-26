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

        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        // Commandes principales
        commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the blackjack game. Use '/blackjack help' for more options."
        });
        
        // Alias pour la commande
        commandManager.AddHandler(CommandAlias, new CommandInfo(OnCommand)
        {
            HelpMessage = "Alias for /blackjack"
        });

        pluginInterface.UiBuilder.Draw += DrawUI;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        pluginLog.Information(Get("plugin_loaded", Configuration.CurrentLanguage));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        
        ConfigWindow.Dispose();
        MainWindow.Dispose();
        
        commandManager.RemoveHandler(CommandName);
        commandManager.RemoveHandler(CommandAlias);
        
        pluginInterface.UiBuilder.Draw -= DrawUI;
        pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUI;
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        pluginLog.Information(Get("plugin_unloaded", Configuration.CurrentLanguage));
    }

    private void OnCommand(string command, string args)
    {
        var arguments = args.Trim().ToLowerInvariant();
        var lang = Configuration.CurrentLanguage;
        
        switch (arguments)
        {
            case "":
                ToggleMainUI();
                break;
            case "config":
            case "settings":
                ToggleConfigUI();
                break;
            case "help":
                pluginLog.Information(Get("available_commands", lang));
                pluginLog.Information(Get("cmd_open_game", lang));
                pluginLog.Information(Get("cmd_open_config", lang));
                pluginLog.Information(Get("cmd_help", lang));
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
            default:
                // Vérifier si c'est une commande de création avec nom
                if (arguments.StartsWith("create "))
                {
                    var saveName = args.Substring(7).Trim(); // Enlever "create "
                    CreateQuickSave(saveName);
                }
                else
                {
                    pluginLog.Warning(Get("unknown_command", lang, arguments));
                }
                break;
        }
    }

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
                pluginLog.Information($"Save '{saveName}' created in slot {i + 1}!");
                return;
            }
        }
        
        pluginLog.Warning("All save slots are full. Please delete a save first.");
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}