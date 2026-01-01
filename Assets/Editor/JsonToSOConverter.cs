using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class JsonToSOConverter
{
    [MenuItem("Tools/Convert All JSON Levels to SO")]
    public static void ConvertAllLevels()
    {

        Debug.Log("開始轉換所有關卡 JSON 為 ScriptableObject...");
        // 1. 定義路徑
        string jsonFolderPath = "Assets/Resources/Levels";
        string soFolderPath = "Assets/Resources/Levels/SO"; // 建議分開存放

        // 確保目標資料夾存在
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            Directory.CreateDirectory(soFolderPath);
            AssetDatabase.Refresh();
        }

        // 2. 取得所有 JSON 檔案
        string[] fileEntries = Directory.GetFiles(jsonFolderPath, "*.json");

        foreach (string filePath in fileEntries)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string jsonContent = File.ReadAllText(filePath);

            // 3. 解析基礎資料
            // 我們先用一個臨時類別來接住 JsonUtility 能解析的部分
            TempLevelData temp = JsonUtility.FromJson<TempLevelData>(jsonContent);

            // 4. 建立新的 SO 實例
            LevelDataSO newSO = ScriptableObject.CreateInstance<LevelDataSO>();
            newSO.levelId = temp.levelId;
            newSO.gridSize = temp.gridSize;
            newSO.layout = temp.layout;
            newSO.orientations = temp.orientations;

            // 5. 手動處理 Mapping (因為 JsonUtility 不支援字典)
            // 這裡使用簡單的字串截取或正則，或者如果你有裝 Newtonsoft.Json 會更簡單
            // 下面示範一種不需要外部套件的解析法：
            newSO.mapping = ParseMappingFromJson(jsonContent);

            // 6. 儲存檔案
            string savePath = $"{soFolderPath}/{fileName}_SO.asset";
            AssetDatabase.CreateAsset(newSO, savePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("所有關卡已成功轉換為 ScriptableObject！");
    }

    // 簡單的解析邏輯：從 JSON 字串中提取 mapping 內容
    private static List<LevelDataSO.MappingEntry> ParseMappingFromJson(string json)
    {
        var result = new List<LevelDataSO.MappingEntry>();

        // 尋找 "mapping": { ... } 之間的內容
        int start = json.IndexOf("\"mapping\":") + 10;
        int end = json.LastIndexOf("}");
        string mappingSection = json.Substring(start, end - start).Trim();
        mappingSection = mappingSection.Replace("{", "").Replace("}", "").Replace("\"", "");

        string[] pairs = mappingSection.Split(',');
        foreach (var pair in pairs)
        {
            string[] kv = pair.Split(':');
            if (kv.Length == 2)
            {
                result.Add(new LevelDataSO.MappingEntry
                {
                    key = kv[0].Trim(),
                    value = kv[1].Trim()
                });
            }
        }
        return result;
    }

    // 內部輔助類別，結構必須與 JSON 欄位完全一致
    [System.Serializable]
    private class TempLevelData
    {
        public int levelId;
        public Vector2Int gridSize;
        public string[] layout;
        public int[] orientations;
    }
}