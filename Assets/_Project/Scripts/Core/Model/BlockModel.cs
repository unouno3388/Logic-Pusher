using Core.Logic.Gate;
using Core.Interfaces;
using Core.Logic;
using System;

namespace Core.Models
{
    public class BlockModel : IBlockEntity
    {
        public GridPoint Position { get; private set; }
        public BlockType Type { get; }
        public GridDirection Orientation { get; }
        public SignalState CurrentState { get; private set; }
        public ILogicStrategy Strategy { get; }

        // 事件：用於通知 Unity View 層播放動畫
        public event Action<GridPoint, float> OnPositionChanged;
        public event Action<SignalState> OnStateChanged;

        public BlockModel(BlockType type, GridPoint pos, GridDirection orientation, ILogicStrategy strategy = null)
        {
            Type = type;
            Position = pos;
            Orientation = orientation;
            Strategy = strategy;
            CurrentState = SignalState.Low;
        }

        public void SetPosition(GridPoint newPos)
        {
            Position = newPos;
            // 觸發事件，GameConstants.MOVE_DURATION 為 0.3f
            OnPositionChanged?.Invoke(newPos, 0.3f);
        }

        public void SetSignalState(SignalState state)
        {
            if (CurrentState != state)
            {
                CurrentState = state;
                OnStateChanged?.Invoke(state);
            }
        }
    }
}