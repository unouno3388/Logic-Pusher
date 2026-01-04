using UnityEngine;
using UI.Base;
using UnityEngine.UI;

using DG.Tweening;
public class GamePanel : BasePanel
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    Button SettingButton;
    void Awake()
    {
        //transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        //UIManager.Instance.ClosePanel(PanelType.StartPanel);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        Debug.Log("GamePanel Awake");
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

    void OnSettingButtonClick()
    {
        Debug.Log("Setting Button Clicked");
        UIManager.Instance.OpenPanel(PanelType.SettingPanel);
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
