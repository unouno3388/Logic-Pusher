using UnityEngine;

namespace Core.Logic.Gate
{
    public interface ILogicStrategy
    {
        /// <summary>
        /// 計算輸出結果
        /// </summary>
        /// <param name="inputs">周圍訊號狀態</param>
        /// <param name="myOrientation">元件朝向 (決定 Input/Output 面)</param>
        bool Calculate(SignalState[] inputs, GridDirection myOrientation);
    }
}
