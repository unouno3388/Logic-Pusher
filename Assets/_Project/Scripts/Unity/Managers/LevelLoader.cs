using Core.Interfaces;
using Core.Logic;
using Core.Logic.Gate; // 引用策略
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using Unity.Controllers; // 引用 BlockController
using UnityEngine;

namespace Unity.Managers
{
    public class LevelLoader : MonoBehaviour
    {
        // 預置體路徑前綴
        private const string PREFAB_PATH = "Prefabs/Blocks/Block_";
        private int levelCount; // 假設有 3 個關卡

        public int LevelCount
        {
            get 
            { 
                 return levelDatabase.LevelCount;
            }
        }

        //private Transform _playerTransform;

        //public Transform GetPlayerTransform() => _playerTransform;

        private void Awake()
        {
            LoadAllLevelsFromResources();
        }
        #region SO Version

        [Header("Data Source")]
        public LevelDatabaseSO levelDatabase; // 拖入你的 LevelDatabase 檔案
        private void LoadAllLevelsFromResources()
        {
            // 1. 如果 Inspector 沒拖入 Database，則嘗試從 Resources 載入現有的或建立新的
            if (levelDatabase == null)
            {
                // 嘗試從 Resources 載入名為 "LevelDatabase" 的資源
                levelDatabase = Resources.Load<LevelDatabaseSO>("LevelDatabase");

                // 如果真的都沒有，就動態建立一個實例（僅存在於記憶體）
                if (levelDatabase == null)
                {
                    levelDatabase = ScriptableObject.CreateInstance<LevelDatabaseSO>();
                    Debug.Log("LevelLoader: 已動態建立臨時 LevelDatabase。");
                }
            }

            // 2. 自動讀取 Resources/Levels/SO 下所有的 LevelDataSO 檔案
            // 注意：路徑是相對於 Resources 的，不需要加 Assets/Resources/
            LevelDataSO[] loadedLevels = Resources.LoadAll<LevelDataSO>("Levels/SO");

            if (loadedLevels.Length > 0)
            {
                // 3. 將讀取到的關卡放入清單，並根據 levelId 進行排序
                levelDatabase.allLevels = loadedLevels.OrderBy(l => l.levelId).ToList();
                Debug.Log($"LevelLoader: 自動讀取了 {loadedLevels.Length} 個關卡 SO 並完成排序。");
            }
            else
            {
                Debug.LogWarning("LevelLoader: 在 Resources/Levels/SO 找不到任何 LevelDataSO 檔案！");
            }
        }
        /// <summary>
        ///New Version: 從 SO 資料庫載入關卡
        /// </summary>
        /// <param name="levelId">關卡ID</param>
        /// <param name="gridSystem">網格系統</param>
        /// <returns>是否有此關卡</returns>
        public bool LoadLevelFromSO(int levelId, GridSystem gridSystem)
        {
            if (levelDatabase == null)
            {
                Debug.LogError("LevelLoader: LevelDatabaseSO 未指定！");
                return false;
            }

            // 根據 ID 尋找對應的 SO 檔案
            LevelDataSO data = levelDatabase.allLevels.Find(l => l.levelId == levelId);

            if (data == null)
            {
                Debug.LogError($"LevelLoader: 在資料庫中找不到 ID 為 {levelId} 的關卡。");
                return false;
            }

            // 開始執行生成邏輯
            ExecuteLoad(data, gridSystem);
            return true;
        }

        private void ExecuteLoad(LevelDataSO levelData, GridSystem gridSystem)
        {
            int height = levelData.gridSize.y;
            int width = levelData.gridSize.x;

            for (int z = 0; z < height; z++)
            {
                // 沿用你原本的座標邏輯 (從上往下讀取字串)
                string row = levelData.layout[height - 1 - z];

                for (int x = 0; x < width; x++)
                {
                    char symbol = row[x];
                    if (symbol == '.') continue;

                    // 從 SO 的 Mapping List 查找對應的 BlockType
                    BlockType type = GetBlockTypeFromMapping(levelData, symbol);
                    if (type == BlockType.Empty) continue;

                    // 取得方向
                    int index = (height - 1 - z) * width + x;
                    int orientationInt = levelData.orientations[index];
                    GridDirection orientation = (GridDirection)orientationInt;

                    // 建立邏輯與視覺物件
                    GridPoint position = new GridPoint(x, z);
                    ILogicStrategy strategy = CreateStrategy(type);
                    BlockModel model = new BlockModel(type, position, orientation, strategy);

                    gridSystem.AddBlock(model);
                    CreateVisualBlock(type, model);
                }
            }
            Debug.Log($"關卡 {levelData.levelId} 加載成功 (來源: ScriptableObject)");
        }

        // 輔助方法：處理 SO 內部的 List<MappingEntry>
        private BlockType GetBlockTypeFromMapping(LevelDataSO data, char symbol)
        {
            string keyStr = symbol.ToString();
            // 在 List 中尋找 Key 匹配的項目
            var entry = data.mapping.FirstOrDefault(m => m.key == keyStr);

            if (entry != null && System.Enum.TryParse(entry.value, out BlockType result))
            {
                return result;
            }
            return BlockType.Empty;
        }
        #endregion

        /// <summary>
        /// Old Version: 從 JSON 檔案載入關卡
        /// </summary>
        /// <param name="levelId"></param>
        /// <param name="gridSystem"></param>
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

            //// 如果是玩家，記錄它的 Transform
            //if (type == BlockType.Player)
            //{
            //    _playerTransform = instance.transform;
            //}

            instance.name = $"{type}_{model.Position.X}_{model.Position.Z}";

            var controller = instance.GetComponent<BlockController>();
            if (controller != null)
            {
                controller.Initialize(model);
            }
        }
    }
}