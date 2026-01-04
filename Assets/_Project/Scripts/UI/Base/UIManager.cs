using System.Collections.Generic;
using System.Linq;
using Unity.Managers;
using UnityEngine;
namespace UI.Base
{
    public class UIManager 
    {
        private static UIManager _instance;
        private Transform _uiRoot;
        private Dictionary<PanelType, string> pathDict;

        private Dictionary<PanelType, GameObject> prefabDict;

        public Dictionary<PanelType, BasePanel> panelDict;

        //GameManager _gameManager;
        //[Header("References")]
        //[SerializeField] private GameManager _gameManager; // 需要知道去監聽誰
        //[SerializeField] private GameObject _nextLevelPanel; // 下一關的 UI 面板
        public static UIManager Instance 
        { 
          get 
          {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
           }
        }
        //public Transform UIRoot
        //{
        //    get
        //    {

               
        //        if (_uiRoot == null)
        //        {

        //            if (GameObject.Find("Canvas")) 
        //            {
        //                _uiRoot = GameObject.Find("Canvas").transform;
        //            }
        //            else
        //            {
        //                _uiRoot = new GameObject("Canvas").transform;
        //            }
        //            return _uiRoot;
        //        }
        //        return _uiRoot;
        //    }
        //}

        private UIManager()
        {
          InitDicts();
        }
        private void InitDicts()
        {
            prefabDict = new Dictionary<PanelType, GameObject>();
            panelDict = new Dictionary<PanelType, BasePanel>();
            pathDict = new Dictionary<PanelType, string>()
            {
                //{ PanelType.StartPanel, "StartPanel" },
                { PanelType.GamePanel, "GamePanel" },
                { PanelType.PausePanel, "PausePanel" },
                { PanelType.GameOverPanel, "GameOverPanel" },
                { PanelType.MainMenuPanel, "MainMenuPanel" },              
                { PanelType.SettingPanel, "SettingPanel" },
                { PanelType.WinLevelPanel, "WinLevelPanel"},
                { PanelType.SelectLevelPanel, "SelectLevelPanel"}
            };
        }

        public BasePanel OpenPanel(PanelType panelType)
        {
            // 檢查介面是否已打開
            BasePanel panel = null;
            if (panelDict.TryGetValue(panelType, out panel))
            {
                // Panel already opened, return it
                Debug.Log($"介面：{panelType}已打開");
                return null;
            }

            //路徑配置檢查
            string path;
            if (!pathDict.TryGetValue(panelType, out path))
            {
                Debug.LogError($"介面：{panelType}的路徑未配置或配置錯誤");
                return null;
            }

            // 使用緩存Prefab
            GameObject panelPrefab;
            if (!prefabDict.TryGetValue(panelType, out panelPrefab))
            {
                // 如果Prefab未緩存，則從Resources加載
                panelPrefab = Resources.Load<GameObject>("Prefabs/UI/" + path);
                prefabDict.Add(panelType, panelPrefab);
            }
            // 加載Prefab(UI)
            //Debug.Log(panelPrefab.name);
            GameObject panelObject = GameObject.Instantiate(panelPrefab, /*UIRoot*/null, false);
            panel = panelObject.GetComponent<BasePanel>();

            if (panel != null)
            {
                panel.OpenPanel(panelType);
            }

            Debug.Log($"介面：{panelType}打開成功");
            panelDict.Add(panelType, panel);
            return panel;
        }
        public bool ClosePanel(PanelType panelType)
        {
            BasePanel basePanel = null;
            if (!panelDict.TryGetValue(panelType,out basePanel))
            {
                Debug.LogWarning($"介面：{panelType}未打開或已關閉");
                return false;
            }
            basePanel.ClosePanel();
            return true;
        }
    }
    
    public enum PanelType
    {
        //StartPanel,
        None,
        GamePanel,
        PausePanel,
        GameOverPanel,
        MainMenuPanel,
        SettingPanel,
        WinLevelPanel,
        SelectLevelPanel,
    }

}

