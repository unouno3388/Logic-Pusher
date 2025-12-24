using Core.Logic.Gate;
using UnityEngine;

namespace Core.Interfaces
{
    public interface IBlockEntity
    {
        GridPoint Position { get; }
        BlockType Type { get; }
        GridDirection Orientation { get; }
        SignalState CurrentState { get; }
        ILogicStrategy Strategy { get; }

        // 用於更新狀態，會觸發視覺層的事件
        void SetSignalState(SignalState state);
    }
}
