using NUnit.Framework;
using Core.Logic.Gate;
public class AndGateTests
{
    [Test]
    public void AndGate_North_AllHighInputs_ReturnsTrue()
    {
        // 1. 安排 (Arrange)
        var strategy = new AndStrategy();
        var orientation = GridDirection.North; // 輸出在北(0)，輸入為東(1), 南(2), 西(3)

        SignalState[] inputs = new SignalState[4];
        inputs[0] = SignalState.Low;  // 北 (輸出端，內容不應影響結果)
        inputs[1] = SignalState.High; // 東
        inputs[2] = SignalState.High; // 南
        inputs[3] = SignalState.High; // 西

        // 2. 執行 (Act)
        bool result = strategy.Calculate(inputs, orientation);

        // 3. 斷言 (Assert)
        Assert.IsTrue(result, "當所有輸入面皆為 High 時，AND Gate 應輸出 True");
    }

    [Test]
    public void AndGate_North_OneLowInput_ReturnsFalse()
    {
        // 安排
        var strategy = new AndStrategy();
        var orientation = GridDirection.North;

        SignalState[] inputs = new SignalState[4];
        inputs[1] = SignalState.High; // 東
        inputs[2] = SignalState.Low;  // 南 (此處為低電位)
        inputs[3] = SignalState.High; // 西

        // 執行
        bool result = strategy.Calculate(inputs, orientation);

        // 斷言
        Assert.IsFalse(result, "只要有一個輸入面為 Low，AND Gate 應輸出 False");
    }
}