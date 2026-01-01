using System.Collections.Generic;
using System.Linq; // 用於 GetAllBlocks
//using LogicPusher.Core.Interfaces; // 假設您的介面 namespace
//using LogicPusher.Core.Models;     // 假設 GridPoint 在此
using Core.Interfaces;
using Core.Models;
namespace Core.Logic
{
    public class GridSystem : IGridSystem
    {
        // 核心資料庫：座標 -> 實體
        private readonly Dictionary<GridPoint, IBlockEntity> _blocks = new Dictionary<GridPoint, IBlockEntity>();

        // 網格邊界 (從 LevelData 讀入)
        private readonly int _width;
        private readonly int _height;

        public GridSystem(int width, int height)
        {
            _width = width;
            _height = height;
        }

        // --- 介面實作 ---

        public IBlockEntity GetBlockAt(GridPoint point)
        {
            return _blocks.TryGetValue(point, out var block) ? block : null;
        }

        public IEnumerable<IBlockEntity> GetAllBlocks()
        {
            return _blocks.Values; // 支援 CircuitSystem 遍歷
        }

        public bool IsWalkable(GridPoint point)
        {
            // 1. 邊界檢查
            if (!IsInsideBounds(point)) return false;

            // 2. 內容檢查
            var block = GetBlockAt(point);

            // 規則：如果是空的(null)或是電線(Wire)則視為可走
            // (具體規則可依照遊戲設計調整，例如 Wire 是否會絆倒人？這裡假設可走)
            return block == null || block.Type == BlockType.Wire || block.Type == BlockType.Empty;
        }

        /// <summary>
        /// 核心移動邏輯：包含推箱子 (Sokoban Logic)
        /// </summary>
        public bool MovePlayer(GridDirection direction)
        {
            // 1. 找到玩家
            var player = _blocks.Values.FirstOrDefault(b => b.Type == BlockType.Player);
            if (player == null) return false; // 場上沒有玩家

            GridPoint currentPos = player.Position;
            GridPoint targetPos = GetNextPosition(currentPos, direction);

            // 2. 檢查目標格狀態
            if (!IsInsideBounds(targetPos)) return false; // 撞牆(邊界)

            var targetBlock = GetBlockAt(targetPos);

            // 情境 A：目標格是空的 (或可走的地板) -> 直接移動
            if (targetBlock == null || targetBlock.Type == BlockType.Empty)
            {
                MoveBlockInternal(player, targetPos);
                return true;
            }

            // 情境 B：目標格有障礙物 (Obstacle/Wall) -> 不可移動
            if (targetBlock.Type == BlockType.Obstacle)
            {
                return false;
            }

            // 情境 C：目標格是可推動的物件 (邏輯閘、Source 等) -> 嘗試推動
            // 檢查「箱子的下一格」是否為空
            GridPoint pushToPos = GetNextPosition(targetPos, direction);

            if (IsInsideBounds(pushToPos) && GetBlockAt(pushToPos) == null)
            {
                // 執行推動序列：
                // 1. 先把箱子移到 pushToPos
                MoveBlockInternal(targetBlock, pushToPos);

                // 2. 再把玩家移到 targetPos
                MoveBlockInternal(player, targetPos);

                return true; // 移動成功
            }

            // 推不動 (箱子後面還有箱子，或箱子後面是牆)
            return false;
        }

        public void ResetLevel()
        {
            _blocks.Clear();

        }

        // --- 輔助方法 ---

        /// <summary>
        /// 用於初始化關卡時填入方塊
        /// </summary>
        public void AddBlock(IBlockEntity block)
        {
            if (_blocks.ContainsKey(block.Position))
                _blocks.Remove(block.Position);

            _blocks[block.Position] = block;
        }

        private void MoveBlockInternal(IBlockEntity block, GridPoint newPos)
        {
            // 1. 從舊位置移除 Key
            _blocks.Remove(block.Position);

            // 2. 更新實體內部的座標 (這會觸發 Model 的 event)
            // 注意：這裡假設 IBlockEntity 有方法可以更新座標，或者我們需要轉型成具體類別
            // 更好的做法是在 IBlockEntity 介面加一個 SetPosition 方法
            if (block is BlockModel model)
            {
                model.SetPosition(newPos);
            }

            // 3. 在新位置加入 Key
            _blocks[newPos] = block;
        }

        private bool IsInsideBounds(GridPoint p)
        {
            return p.X >= 0 && p.X < _width && p.Z >= 0 && p.Z < _height;
        }

        private GridPoint GetNextPosition(GridPoint current, GridDirection dir)
        {
            return dir switch
            {
                GridDirection.North => new GridPoint(current.X, current.Z + 1),
                GridDirection.East => new GridPoint(current.X + 1, current.Z),
                GridDirection.South => new GridPoint(current.X, current.Z - 1),
                GridDirection.West => new GridPoint(current.X - 1, current.Z),
                _ => current
            };
        }
    }
}