using UnityEngine;
using UI.Base;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuPanel : BasePanel
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public Button StartGame;

        [SerializeField]
        Button SettingButton;
        private void Awake()
        {
            StartGame.onClick.AddListener(OnStartStratGameClick);
            SettingButton.onClick.AddListener(OnSettingButtonClick);
            panel = PanelType.MainMenuPanel;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnStartStratGameClick()
        {
            Debug.Log("Start Game Clicked");
            // Add logic to start the game
            UIManager.Instance.OpenPanel(PanelType.SelectLevelPanel);

            //SceneManager.LoadScene("SampleScene");

            //ClosePanel();
            //UIManager.Instance.ClosePanel(PanelType.MainMenuPanel);
        }
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

