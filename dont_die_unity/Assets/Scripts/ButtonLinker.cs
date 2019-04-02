using UnityEngine;
using UnityEngine.UI;

public class ButtonLinker : MonoBehaviour
{
    public Button onePlayer;
    public Button twoPlayers;
    public Button threePlayers;
    public Button fourPlayers;

    void Start()
    {
        onePlayer.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(1); });
        twoPlayers.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(2); });
        threePlayers.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(3); });
        fourPlayers.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(4); });
    }
}
