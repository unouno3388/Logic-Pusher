using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Level System/Level Data")]
public class LevelDataSO : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int levelId;
    public Vector2Int gridSize;

    [Header("Layout (Strings)")]
    public string[] layout; // 直接存儲每一行的字串

    [Header("Orientations")]
    public int[] orientations;

    // 因為 Unity Inspector 不支援 Dictionary，我們可以用一個簡單的類別來代替
    [System.Serializable]
    public class MappingEntry
    {
        public string key;
        public string value;
    }

    public List<MappingEntry> mapping;
}
