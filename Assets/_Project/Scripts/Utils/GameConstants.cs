using UnityEngine;

public static class GameConstants
{
    // 視覺與動畫參數
    public const float MOVE_DURATION = 0.3f; // 移動一格所需秒數
    public const float PUSH_DELAY = 0.1f;    // 推箱子的延遲感
    public const float BLOOM_INTENSITY_ON = 2.5f;
    public const float BLOOM_INTENSITY_OFF = 0.0f;

    // 網格參數
    public const float CELL_SIZE = 1.0f;     // 每一格的 Unity 單位寬度

    // 音效參數 (Audio Keys)
    public const string SFX_PUSH = "Push";
    public const string SFX_POWER_UP = "PowerUp";
    public const string SFX_WIN = "LevelClear";

    // 層級與標籤
    public const string LAYER_BLOCK = "Block";
    public const string LAYER_GROUND = "Ground";
}
