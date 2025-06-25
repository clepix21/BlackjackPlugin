using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using BlackjackPlugin.Windows;
using System;

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
            HelpMessage = "Ouvre le jeu de blackjack. Utilisez '/blackjack help' pour plus d'options."
        });
        
        // Alias pour la commande
        commandManager.AddHandler(CommandAlias, new CommandInfo(OnCommand)
        {
            HelpMessage = "Alias pour /blackjack"
        });

        pluginInterface.UiBuilder.Draw += DrawUI;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        pluginLog.Information("Plugin Blackjack Casino chargé avec succès!");
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

        pluginLog.Information("Plugin Blackjack Casino déchargé proprement.");
    }

    private void OnCommand(string command, string args)
    {
        var arguments = args.Trim().ToLowerInvariant();
        
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
                pluginLog.Information("Commandes disponibles:");
                pluginLog.Information("/blackjack - Ouvre le jeu");
                pluginLog.Information("/blackjack config - Ouvre la configuration");
                pluginLog.Information("/blackjack help - Affiche cette aide");
                break;
            case "reset":
                Configuration.PlayerMoney = 1000;
                Configuration.Save();
                pluginLog.Information("Argent réinitialisé à 1000 Gil!");
                break;
            default:
                pluginLog.Warning($"Commande inconnue: {arguments}. Utilisez '/blackjack help' pour l'aide.");
                break;
        }
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
