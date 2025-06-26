using System;

namespace BlackjackPlugin.SaveSystem;

[Serializable]
public class SaveSlot
{
    public string Name { get; set; } = "";
    public int PlayerMoney { get; set; } = 1000;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime LastPlayed { get; set; } = DateTime.Now;
    
    // Statistiques
    public int GamesPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public int TotalWinnings { get; set; } = 0;
    public int BlackjacksHit { get; set; } = 0;
    
    public bool IsEmpty => string.IsNullOrEmpty(Name);
    
    public double WinPercentage => GamesPlayed > 0 ? (double)GamesWon / GamesPlayed * 100 : 0;
    
    public SaveSlot() { }
    
    public SaveSlot(string name)
    {
        Name = name;
        PlayerMoney = 1000;
        CreatedDate = DateTime.Now;
        LastPlayed = DateTime.Now;
    }
    
    public void UpdateStats(bool won, int winnings, bool blackjack = false)
    {
        GamesPlayed++;
        if (won) GamesWon++;
        TotalWinnings += winnings;
        if (blackjack) BlackjacksHit++;
        LastPlayed = DateTime.Now;
    }
    
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
