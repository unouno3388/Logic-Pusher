using Core.Logic.Gate;
using UnityEngine;

public class OrStrategy : ILogicStrategy
{
    public bool Calculate(SignalState[] inputs, GridDirection myOrientation)
    {
        if (inputs.Length == 0) return false;
        int outputIndex = (int)myOrientation;
        // 根據 SDD：任一「非前方」輸入為 High 則輸出 High
        for (int i = 0; i < 4; i++)
        {
            if (i == outputIndex) continue;
            if (inputs[i] == SignalState.High) return true;
        }
        return false;
    }
}
