using UnityEngine;
using UI.Base;
using Unity.Managers;
using UnityEngine.SceneManagement;
public class InitUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        { 
           Debug.Log("Menu Scene Detected");
        }
        //UIManager.Instance.OpenPanel(PanelType.MainMenuPanel);
        if (GameManager.Instance != null) 
        {
           GameManager.Instance.OnLevelCompleted += HandleLevelCleared;
        }

    }

    private void OnDestroy()
    {
        // 養成好習慣，物件銷毀時取消訂閱，防止 Memory Leak
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelCompleted -= HandleLevelCleared;
        }
    }
    private void HandleLevelCleared()
    {
        // 假設你的 PanelType 有定義一個 NextLevel 或 WinPanel
        UIManager.Instance.OpenPanel(PanelType.WinLevelPanel);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
