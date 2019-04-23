using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public Button restartButton;

    void Start()
    {
        if (restartButton == null)
            restartButton.GetComponent<Button>();

        restartButton.onClick.AddListener(delegate { GameManager.Instance.LoadNextLevel(); });
    }

}
