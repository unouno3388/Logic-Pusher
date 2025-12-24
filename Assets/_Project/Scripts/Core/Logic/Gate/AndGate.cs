using UnityEngine;

namespace Core.Logic.Gate
{
    public class AndStrategy : ILogicStrategy
    {
        public bool Calculate(SignalState[] inputs, GridDirection myOrientation)
        {
            if (inputs.Length == 0) return false;

            int outputIndex = (int)myOrientation;

            for (int i = 0; i < 4; i++)
            {
                if (i == outputIndex) continue;
                if (inputs[i] == SignalState.Low) return false;
            }
            return true;
        }
    }
}
