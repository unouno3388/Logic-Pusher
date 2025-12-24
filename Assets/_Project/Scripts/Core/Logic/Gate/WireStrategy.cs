using Core.Logic.Gate;

namespace Core.Logic.Gate
{
    public class WireStrategy : ILogicStrategy
    {
        public bool Calculate(SignalState[] inputs, GridDirection myOrientation)
        {
            // 檢查所有輸入端，只要有一個是 High 就導通
            foreach (var signal in inputs)
            {
                if (signal == SignalState.High) return true;
            }
            return false;
        }
    }
}