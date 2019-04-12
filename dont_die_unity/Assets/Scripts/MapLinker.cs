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
        foreach (Map map in maps)
        {
            if (map.locked == false)
            {
                map.buttonPrefab.transform.GetChild(0).gameObject.SetActive(false);
                map.buttonPrefab.onClick.AddListener(delegate { SingletonGameManager.Instance.LoadNextLevel(map.mapSceneName); });
            }else
            {
                map.buttonPrefab.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
