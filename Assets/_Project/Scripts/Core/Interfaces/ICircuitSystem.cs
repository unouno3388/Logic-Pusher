using UnityEngine;

namespace Core.Interfaces
{
    public interface ICircuitSystem
    {
        // 觸發全域重算 (傳播邏輯)
        // 實作須知：必須包含 Visited Set 防止無限迴圈 (Loop Protection)
        void Recalculate();

        SignalState[] GetInputsFor(GridPoint point, GridDirection orientation);
    }
}
