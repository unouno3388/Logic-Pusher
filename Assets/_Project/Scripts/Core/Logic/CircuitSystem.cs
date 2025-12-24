using Core.Logic.Gate;
using System;
using System.Collections.Generic;
using System.Linq;

using Core.Interfaces;

namespace Core.Logic
{


    public class CircuitSystem : ICircuitSystem
    {
        private readonly IGridSystem _gridSystem;
        private readonly HashSet<GridPoint> _visited = new HashSet<GridPoint>();

        // 用於記錄每個格子的當前訊號狀態，避免重複運算
        private readonly Dictionary<GridPoint, SignalState> _stateCache = new Dictionary<GridPoint, SignalState>();

        public CircuitSystem(IGridSystem gridSystem)
        {
            _gridSystem = gridSystem;
        }

        /// <summary>
        /// 觸發全域電路重算
        /// </summary>
        public void Recalculate()
        {
            _visited.Clear();
            _stateCache.Clear();

            // 1. 找到所有電源 (Source) 作為起點
            var sources = _gridSystem.GetAllBlocks()
                .Where(b => b.Type == BlockType.Source);

            // 2. 從每個電源開始廣度優先或深度優先傳播
            foreach (var source in sources)
            {
                Propagate(source.Position, SignalState.High);
            }

            // 3. 檢查目標 (Target) 是否被激活
            CheckWinCondition();
        }

        /// <summary>
        /// 遞迴傳播訊號，包含防呆機制
        /// </summary>
        private void Propagate(GridPoint point, SignalState inputSignal)
        {
            // --- 遞迴防呆 (Loop Protection) ---
            if (_visited.Contains(point)) return;
            _visited.Add(point);

            var block = _gridSystem.GetBlockAt(point);
            if (block == null || block.Type == BlockType.Empty || block.Type == BlockType.Obstacle) return;

            // 獲取該元件的邏輯策略與目前方向
            var strategy = block.Strategy;
            var orientation = block.Orientation;

            // 獲取四周鄰居的訊號作為輸入
            SignalState[] neighbors = GetInputsFor(point, orientation);

            // 計算輸出結果
            bool isPowered = strategy.Calculate(neighbors, orientation);
            SignalState outputSignal = isPowered ? SignalState.High : SignalState.Low;

            // 更新 Model 狀態並觸發視覺事件
            block.SetSignalState(outputSignal);

            // 如果有輸出電力，則向「輸出方向」繼續傳播
            if (outputSignal == SignalState.High)
            {
                GridPoint nextPoint = GetNextPoint(point, orientation, block.Type);
                Propagate(nextPoint, SignalState.High);
            }
        }

        /// <summary>
        /// 根據元件類型與朝向決定下一個傳播點
        /// </summary>
        private GridPoint GetNextPoint(GridPoint current, GridDirection orientation, BlockType type)
        {
            // Wire 沒有方向性，會向四周傳播 (這部分可根據需求優化)
            // Logic Gates 僅向前方 (Orientation 方向) 傳播
            return type == BlockType.Wire ? current : MoveInDirection(current, orientation);
        }

        public SignalState[] GetInputsFor(GridPoint point, GridDirection orientation)
        {
            // 按照 [North, East, South, West] 順序回傳周圍狀態
            SignalState[] states = new SignalState[4];
            states[0] = GetSignalFrom(MoveInDirection(point, GridDirection.North));
            states[1] = GetSignalFrom(MoveInDirection(point, GridDirection.East));
            states[2] = GetSignalFrom(MoveInDirection(point, GridDirection.South));
            states[3] = GetSignalFrom(MoveInDirection(point, GridDirection.West));
            return states;
        }

        private SignalState GetSignalFrom(GridPoint targetPoint)
        {
            var neighbor = _gridSystem.GetBlockAt(targetPoint);
            return neighbor?.CurrentState ?? SignalState.Low;
        }

        private GridPoint MoveInDirection(GridPoint p, GridDirection dir)
        {
            return dir switch
            {
                GridDirection.North => p + new GridPoint(0, 1),
                GridDirection.East => p + new GridPoint(1, 0),
                GridDirection.South => p + new GridPoint(0, -1),
                GridDirection.West => p + new GridPoint(-1, 0),
                _ => p
            };
        }

        private void CheckWinCondition()
        {
            // 遍歷所有 Target，若任一輸入為 High 則觸發勝利
            var targets = _gridSystem.GetAllBlocks().Where(b => b.Type == BlockType.Target);
            foreach (var t in targets)
            {
                if (t.CurrentState == SignalState.High)
                {
                    // 呼叫 GameManager 觸發 LevelWin
                }
            }
        }
    }
}