using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCountButtons : MonoBehaviour
{
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> {}
    public IntUnityEvent SetPlayerCount;

    [SerializeField] private Button onePlayer;
    [SerializeField] private Button twoPlayers;
    [SerializeField] private Button threePlayers;
    [SerializeField] private Button fourPlayers;

    void Start()
    {
        onePlayer   .onClick.AddListener(() => SetPlayerCount?.Invoke(1));
        twoPlayers  .onClick.AddListener(() => SetPlayerCount?.Invoke(2));
        threePlayers.onClick.AddListener(() => SetPlayerCount?.Invoke(3));
        fourPlayers .onClick.AddListener(() => SetPlayerCount?.Invoke(4));
    }
}