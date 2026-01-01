using DG.Tweening;
using UI.Base;
using Unity.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WinLevelPanel : BasePanel
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private Button NextLevelButton;

    [SerializeField]
    private Button ReturnMenuButton;
    private void Awake()
    {

        panel = PanelType.WinLevelPanel;
        
        if(NextLevelButton != null) NextLevelButton.onClick.AddListener(OnNextLevelClick);
        if(ReturnMenuButton != null) ReturnMenuButton.onClick.AddListener(OnReturnMenuClick);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNextLevelClick()
    {
        Debug.Log("Next Level Clicked");
        // Add logic to load the next level
        GameManager.Instance.NextLevel();

        UIManager.Instance.ClosePanel(PanelType.WinLevelPanel);
    }
    void OnReturnMenuClick()
    {
        Debug.Log("Return to Menu Clicked");
        // Add logic to return to the main menu
        //GameManager.Instance.;
        UIManager.Instance.ClosePanel(PanelType.WinLevelPanel);
        SceneManager.LoadScene("Menu");
        //UI
    }
    public override void OpenPanel(PanelType panelType)
    {
        base.OpenPanel(panelType);
        //transform.localPosition = new Vector3(0, -800, 0);
        //transform.DOLocalMoveY(0, 0.5f);
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 1f, 1);

        //Time.timeScale = 0f; // ¼È°±¹CÀ¸
        //Debug.Log("WinLevelPanel:Time.timeScale " + Time.timeScale);
    }
    public override void ClosePanel()
    {
        Time.timeScale = 1f; // «ì´_¹CÀ¸
        //Debug.Log("WinLevelPanel:Time.timeScale " + Time.timeScale);
        base.ClosePanel();     
        //GameManager.Instance.NextLevel();
    }
}
