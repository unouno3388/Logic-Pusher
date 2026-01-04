using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SettingPanel : BasePanel
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    Button Return;
    [SerializeField]
    Button ExitGame;
    [SerializeField]
    TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private void Awake()
    {
        Return.onClick.AddListener(OnReturnClick);   
        panel = PanelType.SettingPanel;
    }
    void Start()
    {
        LoadResolution();
        InGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadResolution()
    {
        // 1. 取得並初始化解析度清單
        resolutions = Screen.resolutions.Select(res => new Resolution { width = res.width, height = res.height }).Distinct().ToArray();
        //resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
        }
        resolutionDropdown.AddOptions(options);

        // 2. 決定要顯示哪一個索引 (Index)
        int indexToSet = 0;

        // 檢查硬碟裡有沒有存過設定
        if (PlayerPrefs.HasKey("SelectedResIndex"))
        {
            // 如果有存檔，直接用存檔的值
            indexToSet = PlayerPrefs.GetInt("SelectedResIndex");
        }
        else
        {
            // 如果是第一次執行（沒存檔），才去偵測目前的解析度
            indexToSet = GetCurrentSystemResIndex();
        }

        // 3. 【關鍵步驟】先移除監聽器，設定數值，再加回監聽器
        // 這樣可以避免「設定 UI 數值」的行為誤觸發「儲存」邏輯
        resolutionDropdown.onValueChanged.RemoveAllListeners();

        resolutionDropdown.value = indexToSet;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void InGame() 
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Game")
        {
            // 在遊戲場景中執行的邏輯
            ExitGame.gameObject.SetActive(true);
            ExitGame.onClick.AddListener(() =>
            {
                // 返回主選單場景
                ClosePanel();
                SceneManager.LoadScene("Menu");
                
            });
        }
    }

    public void SetResolution(int index)
    {
        // 套用解析度
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // 【核心：儲存設定】
        // 將選擇的索引值存入硬碟
        PlayerPrefs.SetInt("SelectedResIndex", index);
        PlayerPrefs.Save(); // 強制寫入硬碟

        Debug.Log("已儲存解析度索引：" + index);
    }

    // 輔助函式：尋找目前系統解析度在清單中的位置
    private int GetCurrentSystemResIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                return i;
            }
        }
        return 0;
    }
    private void OnReturnClick()
    {
        //UIManager.Instance.ClosePanel(PanelType.SettingPanel);
        ClosePanel();
    }
}
