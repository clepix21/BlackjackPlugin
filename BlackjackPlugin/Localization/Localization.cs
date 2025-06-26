using System.Collections.Generic;

namespace BlackjackPlugin.Localization;

public enum Language
{
    English,
    French
}

public static class Localization
{
    private static readonly Dictionary<string, Dictionary<Language, string>> Translations = new()
    {
        // Interface principale
        ["window_title"] = new() { [Language.English] = "🎰 Blackjack Casino", [Language.French] = "🎰 Blackjack Casino" },
        ["config_title"] = new() { [Language.English] = "⚙️ Blackjack Configuration", [Language.French] = "⚙️ Configuration Blackjack" },
        
        // Header
        ["money"] = new() { [Language.English] = "💰 Money", [Language.French] = "💰 Argent" },
        ["bet"] = new() { [Language.English] = "🎯 Bet", [Language.French] = "🎯 Mise" },
        ["current_save"] = new() { [Language.English] = "💾 Save", [Language.French] = "💾 Sauvegarde" },
        
        // Système de sauvegarde
        ["save_management"] = new() { [Language.English] = "💾 Save Management", [Language.French] = "💾 Gestion des sauvegardes" },
        ["no_save_selected"] = new() { [Language.English] = "No save selected", [Language.French] = "Aucune sauvegarde sélectionnée" },
        ["select_save"] = new() { [Language.English] = "Please select or create a save to play", [Language.French] = "Veuillez sélectionner ou créer une sauvegarde pour jouer" },
        ["slot"] = new() { [Language.English] = "Slot", [Language.French] = "Emplacement" },
        ["empty"] = new() { [Language.English] = "Empty", [Language.French] = "Vide" },
        ["create_save"] = new() { [Language.English] = "Create", [Language.French] = "Créer" },
        ["load_save"] = new() { [Language.English] = "Load", [Language.French] = "Charger" },
        ["delete_save"] = new() { [Language.English] = "Delete", [Language.French] = "Supprimer" },
        ["reset_save"] = new() { [Language.English] = "Reset", [Language.French] = "Réinitialiser" },
        ["save_name"] = new() { [Language.English] = "Save name", [Language.French] = "Nom de la sauvegarde" },
        ["created"] = new() { [Language.English] = "Created", [Language.French] = "Créée" },
        ["last_played"] = new() { [Language.English] = "Last played", [Language.French] = "Dernière partie" },
        ["games_played"] = new() { [Language.English] = "Games", [Language.French] = "Parties" },
        ["win_rate"] = new() { [Language.English] = "Win rate", [Language.French] = "Taux de victoire" },
        ["total_winnings"] = new() { [Language.English] = "Total winnings", [Language.French] = "Gains totaux" },
        ["blackjacks"] = new() { [Language.English] = "Blackjacks", [Language.French] = "Blackjacks" },
        
        // Zones de jeu
        ["dealer"] = new() { [Language.English] = "🎩 Dealer:", [Language.French] = "🎩 Croupier:" },
        ["your_hand"] = new() { [Language.English] = "👤 Your hand:", [Language.French] = "👤 Votre main:" },
        ["total"] = new() { [Language.English] = "Total", [Language.French] = "Total" },
        ["bust"] = new() { [Language.English] = "BUST!", [Language.French] = "BUST!" },
        ["blackjack"] = new() { [Language.English] = "BLACKJACK!", [Language.French] = "BLACKJACK!" },
        
        // Contrôles de mise
        ["place_bet"] = new() { [Language.English] = "💸 Place your bet:", [Language.French] = "💸 Placez votre mise:" },
        ["deal_cards"] = new() { [Language.English] = "🎯 Deal cards", [Language.French] = "🎯 Distribuer les cartes" },
        ["no_money"] = new() { [Language.English] = "You have no money left!", [Language.French] = "Vous n'avez plus d'argent!" },
        ["reset_money"] = new() { [Language.English] = "Reset money", [Language.French] = "Réinitialiser l'argent" },
        
        // Contrôles de jeu
        ["hit"] = new() { [Language.English] = "🃏 Hit", [Language.French] = "🃏 Tirer une carte" },
        ["stand"] = new() { [Language.English] = "✋ Stand", [Language.French] = "✋ Rester" },
        ["double"] = new() { [Language.English] = "⬆️ Double", [Language.French] = "⬆️ Double" },
        ["dealer_playing"] = new() { [Language.English] = "🎲 Dealer is playing...", [Language.French] = "🎲 Le croupier joue..." },
        
        // Résultats
        ["blackjack_win"] = new() { [Language.English] = "🎉 BLACKJACK! You win!", [Language.French] = "🎉 BLACKJACK! Vous gagnez!" },
        ["player_win"] = new() { [Language.English] = "🎊 You win!", [Language.French] = "🎊 Vous gagnez!" },
        ["dealer_win"] = new() { [Language.English] = "😞 Dealer wins", [Language.French] = "😞 Le croupier gagne" },
        ["push"] = new() { [Language.English] = "🤝 Push!", [Language.French] = "🤝 Égalité!" },
        ["net_winnings"] = new() { [Language.English] = "💰 Net winnings: +{0} Gil", [Language.French] = "💰 Gains nets: +{0} Gil" },
        ["losses"] = new() { [Language.English] = "💸 Losses: {0} Gil", [Language.French] = "💸 Pertes: {0} Gil" },
        ["bet_recovered"] = new() { [Language.English] = "💰 Push - Bet recovered", [Language.French] = "💰 Égalité - Mise récupérée" },
        ["new_game"] = new() { [Language.English] = "🔄 New game", [Language.French] = "🔄 Nouvelle partie" },
        
        // Log de jeu
        ["game_history"] = new() { [Language.English] = "📜 Game history", [Language.French] = "📜 Historique du jeu" },
        
        // Messages de jeu
        ["cards_dealt"] = new() { [Language.English] = "Cards dealt", [Language.French] = "Cartes distribuées" },
        ["card_drawn"] = new() { [Language.English] = "Card drawn: {0}", [Language.French] = "Carte tirée: {0}" },
        ["bust_message"] = new() { [Language.English] = "BUST! You exceeded 21", [Language.French] = "BUST! Vous avez dépassé 21" },
        ["dealer_reveals"] = new() { [Language.English] = "Dealer reveals hole card", [Language.French] = "Le croupier révèle sa carte" },
        ["dealer_draws"] = new() { [Language.English] = "Dealer draws: {0}", [Language.French] = "Le croupier tire: {0}" },
        ["dealer_bust"] = new() { [Language.English] = "Dealer busts - You win!", [Language.French] = "Le croupier dépasse 21 - Vous gagnez!" },
        ["you_win"] = new() { [Language.English] = "You win!", [Language.French] = "Vous gagnez!" },
        ["dealer_wins"] = new() { [Language.English] = "Dealer wins", [Language.French] = "Le croupier gagne" },
        ["tie"] = new() { [Language.English] = "Tie!", [Language.French] = "Égalité!" },
        ["double_blackjack"] = new() { [Language.English] = "Double blackjack - Push!", [Language.French] = "Double blackjack - Égalité!" },
        
        // Configuration
        ["money_management"] = new() { [Language.English] = "💰 Money Management", [Language.French] = "💰 Gestion de l'argent" },
        ["default_bet"] = new() { [Language.English] = "Default bet", [Language.French] = "Mise par défaut" },
        ["game_options"] = new() { [Language.English] = "🎮 Game Options", [Language.French] = "🎮 Options de jeu" },
        ["sounds_enabled"] = new() { [Language.English] = "Sounds enabled", [Language.French] = "Sons activés" },
        ["animations_enabled"] = new() { [Language.English] = "Animations enabled", [Language.French] = "Animations activées" },
        ["language_settings"] = new() { [Language.English] = "🌐 Language Settings", [Language.French] = "🌐 Paramètres de langue" },
        ["use_french"] = new() { [Language.English] = "Use French", [Language.French] = "Utiliser le français" },
        ["save"] = new() { [Language.English] = "💾 Save", [Language.French] = "💾 Sauvegarder" },
        ["reset"] = new() { [Language.English] = "🔄 Reset", [Language.French] = "🔄 Réinitialiser" },
        
        // Commandes
        ["plugin_loaded"] = new() { [Language.English] = "Blackjack Casino plugin loaded successfully!", [Language.French] = "Plugin Blackjack Casino chargé avec succès!" },
        ["plugin_unloaded"] = new() { [Language.English] = "Blackjack Casino plugin unloaded cleanly.", [Language.French] = "Plugin Blackjack Casino déchargé proprement." },
        ["money_reset"] = new() { [Language.English] = "Money reset to 1000 Gil!", [Language.French] = "Argent réinitialisé à 1000 Gil!" },
        ["unknown_command"] = new() { [Language.English] = "Unknown command: {0}. Use '/blackjack help' for help.", [Language.French] = "Commande inconnue: {0}. Utilisez '/blackjack help' pour l'aide." },
        ["available_commands"] = new() { [Language.English] = "Available commands:", [Language.French] = "Commandes disponibles:" },
        ["cmd_open_game"] = new() { [Language.English] = "/blackjack - Opens the game", [Language.French] = "/blackjack - Ouvre le jeu" },
        ["cmd_open_config"] = new() { [Language.English] = "/blackjack config - Opens configuration", [Language.French] = "/blackjack config - Ouvre la configuration" },
        ["cmd_help"] = new() { [Language.English] = "/blackjack help - Shows this help", [Language.French] = "/blackjack help - Affiche cette aide" }
    };

    public static string Get(string key, Language language = Language.English, params object[] args)
    {
        if (Translations.TryGetValue(key, out var translations) && 
            translations.TryGetValue(language, out var text))
        {
            return args.Length > 0 ? string.Format(text, args) : text;
        }
        
        // Fallback vers l'anglais si la traduction française n'existe pas
        if (language == Language.French && 
            Translations.TryGetValue(key, out var fallbackTranslations) && 
            fallbackTranslations.TryGetValue(Language.English, out var fallbackText))
        {
            return args.Length > 0 ? string.Format(fallbackText, args) : fallbackText;
        }
        
        return $"[MISSING: {key}]";
    }
}
