using System;
using UnityEngine;

public class Judge
{
    public event Action<PlayerHandle> OnPlayerWin;
    private PlayerController[] playerControllers;
    private int[] playerScores;
    public RuleSet rules;

    public Judge(PlayerController[] _players, RuleSet _rules)
    {
        playerControllers = _players;
        playerScores = new int[_players.Length];
        rules = _rules;

        foreach(PlayerController player in playerControllers)
        {
            player.OnDie += OnPlayerDie;
            player.OnChangeHealth += OnHealthChange;
            player.OnAreaScore += OnAreaScore;
            player.SetImmortal(rules.immortalPlayers);
        }
    }

    private void OnPlayerDie(PlayerHandle handle)
    {
        playerScores[handle] += rules.deathMultiplier * 1;
        CheckScore(handle);
    }

    private void OnHealthChange(PlayerHandle handle, int amount)
    {
        playerScores[handle] += rules.damageMultiplier * amount;
        CheckScore(handle);
    }

    private void OnAreaScore(PlayerHandle handle, int amount)
    {
        playerScores[handle] += rules.areaMultiplier * amount;
        CheckScore(handle);
    }

    private void CheckScore(PlayerHandle handle)
    {
            if(playerScores[handle] >= rules.scoreLimit)
                OnPlayerWin?.Invoke(handle);
    }
}
