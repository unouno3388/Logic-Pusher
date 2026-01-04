using System.Collections.Generic;
using TMPro;
using UI.Base;
using Unity.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace UI
{
    public class SelectLevelPanel : BasePanel
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        //public Button StartGame;
        [Header("Data Source")]
        public LevelDatabaseSO levelDatabase;
        [SerializeField]
        Button CloseButton;

        [Header("UI References")]
        public GameObject buttonPrefab;        // 關卡按鈕的 Prefab

        private List<GameObject> selectLevelButtons = new List<GameObject>();

        private void Awake()
        {
            //StartGame.onClick.AddListener(OnStartStratGameClick);
            //SettingButton.onClick.AddListener(OnSettingButtonClick);
            CloseButton.onClick.AddListener(ClosePanel);
            panel = PanelType.SelectLevelPanel;
        }
        [SerializeField]
        private List<Button> SelectLevelButtons;
        void Start()
        {
            InitSelectLevelButton();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void InitSelectLevelButton()
        {
            // 1. 尋找父物件 Panel
            Transform buttonParent = transform.Find("SelectPanel");

            if (buttonParent == null)
            {
                Debug.LogError("找不到名為 'Panel' 的子物件，請檢查 Hierarchy 命名");
                return;
            }


            // 清除舊有的按鈕 (防止重複呼叫時按鈕疊加)
            foreach (GameObject btn in selectLevelButtons) Destroy(btn);
            selectLevelButtons.Clear();

            // 2. 根據資料庫中的關卡數量進行循環
            // 順序會依照你在 LevelDatabaseSO 裡 allLevels 清單的排序
            for (int i = 0; i < levelDatabase.allLevels.Count; i++)
            {
                // 取得關卡數據
                LevelDataSO levelData = levelDatabase.allLevels[i];
                int currentLevelId = levelData.levelId;

                // 3. 生成按鈕實例
                GameObject btnInstance = Instantiate(buttonPrefab, buttonParent);
                btnInstance.name = $"Button_Level_{currentLevelId}";

                // 4. 設定按鈕顯示文字
                // 這裡假設按鈕 Prefab 子物件中有 Text 或 TMP 文字
                var btnText = btnInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = currentLevelId.ToString();
                }
                else
                {
                    // 如果是用舊版 Text
                    btnInstance.GetComponentInChildren<Text>().text = currentLevelId.ToString();
                }

                // 5. 綁定按鈕點擊事件
                Button btnComponent = btnInstance.GetComponent<Button>();
                if (btnComponent != null)
                {
                    // 注意：在迴圈中使用 Lambda 需要捕獲一個局部變數，避免 ID 錯誤
                    int idToLoad = currentLevelId;
                    btnComponent.onClick.AddListener(() => OnLevelSelected(idToLoad));
                }

                // 加入清單備查
                selectLevelButtons.Add(btnInstance);
            }
        }
        void OnLevelSelected(int levelId)
        {
            Debug.Log($"選擇了關卡 ID: {levelId}");
            // 在這裡加入載入關卡的邏輯
            UIManager.Instance.ClosePanel(PanelType.SelectLevelPanel);
            SceneManager.LoadScene("Game");
            GameManager.Instance.SelectLevel(levelId);
            // 例如：SceneManager.LoadScene("Level_" + levelId);
        }
        //public void OnStartStratGameClick()
        //{
        //    Debug.Log("Start Game Clicked");
        //    // Add logic to start the game
        //    //UIManager.Instance.OpenPanel(PanelType.GamePanel);

        //    SceneManager.LoadScene("SampleScene");
        //    //ClosePanel();
        //    //UIManager.Instance.ClosePanel(PanelType.MainMenuPanel);
        //}
        public void OnSettingButtonClick()
        {
            Debug.Log("Setting Button Clicked");
            UIManager.Instance.OpenPanel(PanelType.SettingPanel);
        }

        public override void OpenPanel(PanelType panelType)
        {
            base.OpenPanel(panelType);
        }

        public override void ClosePanel()
        {
            base.ClosePanel();
        }
        //public void OnBtnCancel()
        //{
        //    Debug.Log("Cancel Button Clicked");
        //    ClosePanel();
        //}
    }

}

