using UnityEditor;
using UnityEngine;

public static class DevCommands
{
    [MenuItem("Dev Commands/Kill Player")]
    public static void KillPlayer()
    {
        GameManager.KillPlayers();
    }
}