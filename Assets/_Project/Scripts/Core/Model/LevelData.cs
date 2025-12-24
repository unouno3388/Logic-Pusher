using System;
using UnityEngine;

namespace Core.Models
{
    [System.Serializable]
    public class LevelData
    {
        public int levelId;
        public Vector2Int gridSize;
        public string[] layout;
        public int[] orientations;
        public LevelMapping mapping;
    }

    [System.Serializable]
    public class LevelMapping
    {
        // --- 定義 JSON 欄位對應 ---
        public string W = "Wire";
        public string A = "AndGate";
        public string O = "OrGate";
        public string N = "NotGate";
        public string S = "Source";
        public string T = "Target";
        public string B = "Obstacle";

        // [新增] 玩家的對應欄位
        public string P = "Player";

        public BlockType GetBlockType(char c)
        {
            string typeName = c switch
            {
                'W' => W,
                'A' => A,
                'O' => O,
                'N' => N,
                'S' => S,
                'T' => T,
                'B' => B,

                // [新增] 遇到 'P' 就回傳 "Player"
                'P' => P,

                '.' => "Empty",
                _ => "Empty"
            };

            if (Enum.TryParse(typeName, out BlockType result))
            {
                return result;
            }
            return BlockType.Empty;
        }
    }
}