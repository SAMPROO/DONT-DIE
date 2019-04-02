using UnityEngine;
using UnityEngine.UI;

public class ButtonLinker : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    void Start()
    {
        button1.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(1); });
        button2.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(2); });
        button3.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(3); });
        button4.onClick.AddListener(delegate { SingletonGameManager.Instance.SetPlayerCount(4); });
    }
}
