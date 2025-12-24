using UnityEngine;
using UI.Base;
using UnityEngine.UI;
public class SettingPanel : BasePanel
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    Button Return;

    private void Awake()
    {
        Return.onClick.AddListener(OnReturnClick);   
        panel = PanelType.SettingPanel;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnReturnClick()
    {
        //UIManager.Instance.ClosePanel(PanelType.SettingPanel);
        ClosePanel();
    }
}
