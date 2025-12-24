using Core.Logic.Gate;
using UnityEngine;

public class NotStrategy : ILogicStrategy
{
    public bool Calculate(SignalState[] inputs, GridDirection myOrientation)
    {
        // 根據 SDD：後方輸入為 Low 則輸出 High
        // 計算後方索引：(目前方向 + 2) % 4
        int backIndex = ((int)myOrientation + 2) % 4;
        return inputs[backIndex] == SignalState.Low;
    }
}
