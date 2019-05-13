using UnityEngine;

[CreateAssetMenu]
public class RuleSet : ScriptableObject
{
    //Mode Name
    public string modeName;

    //Win Condition
    public int scoreLimit;

    //ScoreMultipliers
    public int deathMultiplier;
    public int damageMultiplier;
    public int areaMultiplier;

    //Others
    public bool immortalPlayers;
    public bool playerRespawn;

}
