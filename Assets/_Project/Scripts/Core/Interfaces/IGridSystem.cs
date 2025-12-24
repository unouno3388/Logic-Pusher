using System.Collections.Generic;
using UnityEngine;

namespace Core.Interfaces
{

    public interface IGridSystem
    {
        IBlockEntity GetBlockAt(GridPoint point);

        IEnumerable<IBlockEntity> GetAllBlocks();

        bool IsWalkable(GridPoint point); // 需包含邊界檢查 (Bounds Check)

        // 移動介面：回傳 true 代表移動成功 (包含推箱子)
        // 成功後應觸發 Model 的 OnPositionChanged 事件
        bool MovePlayer(GridDirection direction);

        void ResetLevel();
    }
}