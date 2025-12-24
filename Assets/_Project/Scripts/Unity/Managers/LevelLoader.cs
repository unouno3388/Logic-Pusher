using UnityEngine;
using Core.Logic;
using Core.Models;
using Core.Interfaces;
using Core.Logic.Gate; // 引用策略
using Unity.Controllers; // 引用 BlockController
using System.Collections.Generic;

namespace Unity.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        // 預置體路徑前綴
        private const string PREFAB_PATH = "Prefabs/Blocks/Block_";

        public void LoadLevel(int levelId, GridSystem gridSystem)
        {
            // 1. 讀取 JSON 檔案
            string path = $"Levels/Level_{levelId}";
            TextAsset jsonFile = Resources.Load<TextAsset>(path);

            if (jsonFile == null)
            {
                Debug.LogError($"找不到關卡檔：{path}");
                return;
            }

            // 2. 解析 JSON
            LevelData levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);

            // 3. 遍歷地圖資料 (注意：JSON 的 layout 通常是從上到下，對應 Z 軸從大到小)
            // 為了方便，我們假設 layout[0] 是地圖的最上面 (Max Z)，layout[Last] 是最下面 (Z=0)
            int height = levelData.gridSize.y;
            int width = levelData.gridSize.x;

            for (int z = 0; z < height; z++)
            {
                // JSON layout 陣列的索引 (從上往下讀)
                // 如果您的 JSON layout[0] 是最底層，則不需要倒過來。
                // 根據您的 Level 1 JSON 範例，layout 畫起來像地圖，所以 layout[0] 應該是 Z=Max。
                // 但為了配合常見習慣，這裡假設 layout 索引 0 對應 Z = height - 1
                string row = levelData.layout[height - 1 - z];

                for (int x = 0; x < width; x++)
                {
                    char symbol = row[x];

                    // 忽略空地 '.'
                    if (symbol == '.') continue;

                    // 4. 根據符號取得 BlockType
                    BlockType type = levelData.mapping.GetBlockType(symbol);
                    if (type == BlockType.Empty) continue;

                    // 5. 取得方向 (從一維陣列轉換)
                    // JSON index: row * width + col
                    int index = (height - 1 - z) * width + x;
                    int orientationInt = levelData.orientations[index];
                    GridDirection orientation = (GridDirection)orientationInt;

                    // 6. 建立邏輯實體 (Core Model)
                    GridPoint position = new GridPoint(x, z);
                    ILogicStrategy strategy = CreateStrategy(type);

                    BlockModel model = new BlockModel(type, position, orientation, strategy);

                    // 加入核心系統
                    gridSystem.AddBlock(model);

                    // 7. 生成視覺物件 (View Prefab)
                    CreateVisualBlock(type, model);
                }
            }

            Debug.Log($"關卡 {levelId} 載入完成！");
        }

        // 在 CreateStrategy 方法中修改
        private ILogicStrategy CreateStrategy(BlockType type)
        {
            return type switch
            {
                BlockType.AndGate => new AndStrategy(),
                BlockType.OrGate => new OrStrategy(),
                BlockType.NotGate => new NotStrategy(),

                // --- 修改以下部分 ---
                BlockType.Source => new SourceStrategy(),
                BlockType.Wire => new WireStrategy(),
                BlockType.Target => new WireStrategy(), // Target 邏輯跟 Wire 一樣 (有電就亮)

                // 其他 (如 Player, Obstacle) 確實不需要電路邏輯，維持 null 沒關係
                // 但要在 CircuitSystem 做防呆 (詳見第三步)
                _ => null
            };
        }

        private void CreateVisualBlock(BlockType type, BlockModel model)
        {
            // 拼湊 Prefab 路徑，例如 "Prefabs/Blocks/Block_AndGate"
            string prefabName = PREFAB_PATH + type.ToString();
            GameObject prefab = Resources.Load<GameObject>(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning($"找不到 Prefab: {prefabName}，請確認 Resources 資料夾與命名是否正確。");
                return;
            }

            // 生成並初始化
            GameObject instance = Instantiate(prefab, transform); // 生成在 LevelLoader 底下保持整潔
            instance.name = $"{type}_{model.Position.X}_{model.Position.Z}";

            var controller = instance.GetComponent<BlockController>();
            if (controller != null)
            {
                controller.Initialize(model);
            }
        }
    }
}