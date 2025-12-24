using UnityEngine;
using UI.Base;
public class InitUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIManager.Instance.OpenPanel(PanelType.MainMenuPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
