using System;

namespace BlackjackPlugin.SaveSystem;

/// <summary>
/// Représente un slot de sauvegarde pour un joueur dans le plugin Blackjack.
/// </summary>
[Serializable]
public class SaveSlot
{
    /// <summary>
    /// Nom du joueur ou du slot.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Argent actuel du joueur.
    /// </summary>
    public int PlayerMoney { get; set; } = 1000;

    /// <summary>
    /// Date de création du slot.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Date de la dernière partie jouée.
    /// </summary>
    public DateTime LastPlayed { get; set; } = DateTime.Now;

    // Statistiques

    /// <summary>
    /// Nombre total de parties jouées.
    /// </summary>
    public int GamesPlayed { get; set; } = 0;

    /// <summary>
    /// Nombre de parties gagnées.
    /// </summary>
    public int GamesWon { get; set; } = 0;

    /// <summary>
    /// Gains totaux du joueur.
    /// </summary>
    public int TotalWinnings { get; set; } = 0;

    /// <summary>
    /// Nombre de blackjacks réalisés.
    /// </summary>
    public int BlackjacksHit { get; set; } = 0;

    /// <summary>
    /// Indique si le slot est vide (pas de nom).
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(Name);

    /// <summary>
    /// Pourcentage de victoires.
    /// </summary>
    public double WinPercentage => GamesPlayed > 0 ? (double)GamesWon / GamesPlayed * 100 : 0;

    /// <summary>
    /// Constructeur par défaut.
    /// </summary>
    public SaveSlot() { }

    /// <summary>
    /// Constructeur avec nom du slot.
    /// </summary>
    /// <param name="name">Nom du joueur ou du slot.</param>
    public SaveSlot(string name)
    {
        Name = name;
        PlayerMoney = 1000;
        CreatedDate = DateTime.Now;
        LastPlayed = DateTime.Now;
    }

    /// <summary>
    /// Met à jour les statistiques après une partie.
    /// </summary>
    /// <param name="won">Indique si la partie a été gagnée.</param>
    /// <param name="winnings">Gains de la partie.</param>
    /// <param name="blackjack">Indique si un blackjack a été réalisé.</param>
    public void UpdateStats(bool won, int winnings, bool blackjack = false)
    {
        GamesPlayed++;
        if (won) GamesWon++;
        TotalWinnings += winnings;
        if (blackjack) BlackjacksHit++;
        LastPlayed = DateTime.Now;
    }

    /// <summary>
    /// Réinitialise les statistiques et l'argent du joueur.
    /// </summary>
    public void Reset()
    {
        PlayerMoney = 1000;
        GamesPlayed = 0;
        GamesWon = 0;
        TotalWinnings = 0;
        BlackjacksHit = 0;
        CreatedDate = DateTime.Now;
        LastPlayed = DateTime.Now;
    }

    /// <summary>
    /// Vide complètement le slot.
    /// </summary>
    public void Clear()
    {
        Name = "";
        PlayerMoney = 1000;
        CreatedDate = DateTime.Now;
        LastPlayed = DateTime.Now;
        GamesPlayed = 0;
        GamesWon = 0;
        TotalWinnings = 0;
        BlackjacksHit = 0;
    }
}
