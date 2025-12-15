using UnityEngine;
using System.Collections.Generic;

public static class GameSession
{
    // Seznam ID poražených nepřátel
    public static List<string> deadEnemies = new List<string>();

    // ID nepřítele, se kterým právě bojujeme
    public static string currentEnemyID;

    // Uložená pozice hráče před bojem (aby se nevrátil na start mapy)
    public static Vector3 lastPlayerPosition;

    // Flag, abychom věděli, že se vracíme z boje
    public static bool returningFromBattle = false;

    // --- NOVÉ: Inventář ---
    public static bool hasDungeonKey = false; // Klíč z prvních bojů
    public static bool hasRoyalCrown = false; // Koruna z Bosse (konec hry)
}