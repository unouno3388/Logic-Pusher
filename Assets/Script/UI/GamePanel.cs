using UnityEngine;
using UI.Base;
using UnityEngine.UI;

using DG.Tweening;
public class GamePanel : BasePanel
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button ReturnMainMenu;
    void Awake()
    {
        //transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //UIManager.Instance.ClosePanel(PanelType.StartPanel);
        ReturnMainMenu.onClick.AddListener(OnReturnMainMenuClick);
        panel = PanelType.GamePanel;
    }
    void Start()
    {
        //panel = PanelType.GamePanel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnReturnMainMenuClick()
    {
        Debug.Log("Return to Main Menu Clicked");
        // Add logic to return to main menu
        //UIManager.Instance.OpenPanel(PanelType.MainMenuPanel);
        ClosePanel();
        //UIManager.Instance.ClosePanel(PanelType.GamePanel);
    }
    public override void OpenPanel(PanelType panelType)
    {
        base.OpenPanel(panelType);
        //transform.localPosition = new Vector3(0, -800, 0);
        //transform.DOLocalMoveY(0, 0.5f);
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 1f, 1);
    }
}
