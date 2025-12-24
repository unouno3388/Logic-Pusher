using System;

// 改用標準 struct，手動實作構造函數與運算子
public readonly struct GridPoint : IEquatable<GridPoint>
{
    public readonly int X;
    public readonly int Z;

    public GridPoint(int x, int z)
    {
        X = x;
        Z = z;
    }

    public static GridPoint Zero => new GridPoint(0, 0);

    // 實作 SDD 要求的分量加法
    public static GridPoint operator +(GridPoint a, GridPoint b)
        => new GridPoint(a.X + b.X, a.Z + b.Z);

    // 實作相等性判斷 (struct 建議實作以優化效能)
    public bool Equals(GridPoint other) => X == other.X && Z == other.Z;
    public override bool Equals(object obj) => obj is GridPoint other && Equals(other);
    public override int GetHashCode() => (X, Z).GetHashCode();
}
public enum GridDirection { North = 0, East = 1, South = 2, West = 3 }

public enum SignalState { Low = 0, High = 1 }

public enum BlockType { Wire, AndGate, OrGate, NotGate, Source, Target, Obstacle, Empty, Player }

//using System;

//public readonly record struct GridPoint(int X, int Z)
//{
//    public static GridPoint operator +(GridPoint a, GridPoint b) => new GridPoint(a.X + b.X, a.Z + b.Z);
//}

//public enum GridDirection { North = 0, East = 1, South = 2, West = 3 }

//public enum SignalState { Low = 0, High = 1 }

//public enum BlockType { Wire, AndGate, OrGate, NotGate, Source, Target, Obstacle, Empty, Player }