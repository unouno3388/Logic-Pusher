using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level System/Database")]
public class LevelDatabaseSO : ScriptableObject
{
    // 這裡存放所有轉換出來的關卡
    public List<LevelDataSO> allLevels = new List<LevelDataSO>();

    // 獲取關卡總數
    public int LevelCount => allLevels.Count;

    // 根據 ID 獲取關卡 (假設 ID 是從 1 開始)
    public LevelDataSO GetLevelById(int id)
    {
        return allLevels.Find(l => l.levelId == id);
    }
}