using UnityEngine;
using UnityEngine.UI;

public class MapLinker : MonoBehaviour
{

    [System.Serializable]
    public class Map
    {
        public Button buttonPrefab;
        public string mapSceneName;
        public bool locked = false;
    }
    
    public Map[] maps;

    private void Awake()
    {
        
        for (int i = 0; i < maps.Length; i++)
        {
            if (maps[i].locked == false)
            {
                maps[i].buttonPrefab.transform.GetChild(0).gameObject.SetActive(false);
                maps[i].buttonPrefab.onClick.AddListener(delegate { SingletonGameManager.Instance.LoadNextLevel(maps[i].mapSceneName); });
            }
            else
            {
                maps[i].buttonPrefab.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
