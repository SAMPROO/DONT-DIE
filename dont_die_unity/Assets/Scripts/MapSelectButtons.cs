using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapSelectButtons : MonoBehaviour
{
    [System.Serializable]
    private class MapButtonInfo
    {
        public Button   button;
        public string   mapSceneName;
        public bool     locked = false;
    }

    [SerializeField] private MapButtonInfo [] infos;

    [System.Serializable]
    public class StringUnityEvent : UnityEvent<string> {}
    public StringUnityEvent SetMapSceneName;

    private void Awake ()
    {
        foreach (var info in infos)
        {
            if (info.locked == false)
            {
                info.button.onClick.AddListener(() => SetMapSceneName?.Invoke(info.mapSceneName));
                info.button.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                info.button.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}