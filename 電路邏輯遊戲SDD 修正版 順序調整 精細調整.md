好的，這是整合了 **方向性定義 (Orientation)**、**遞迴防呆 (Loop Protection)** 以及 **移動事件 (Movement Events)** 的完整修正版 SDD。

我將版本號更新為 **v1.4**。您可以直接複製這份文件，它現在具備了實作邏輯閘所需的完整細節。

-----

# Logic Pusher 3D - 技術規格書 (SDD)

**Technical Design Document**

  * **專案名稱**：Logic Pusher 3D
  * **指定引擎版本**：**Unity 6.2 (6000.2.9f1)**
  * **開發模式**：Contract-First (契約優先) & SDD
  * **文件版本**：**v1.4 (完整邏輯定義版)**

-----

## 1\. 專案資訊與技術選型 (Project Info & Tech Stack)

### 1.1 基礎資訊

  * **Engine Core**: **Unity 6.2 (6000.2.9f1)**
      * *Constraint*: 團隊協作必須鎖定此版本。
  * **Render Pipeline**: **URP (Universal Render Pipeline)**
      * 利用 Unity 6 Volume Framework 處理 **Global Bloom** (發光特效)。
      * 開啟 **SRP Batcher** 優化渲染效能。
  * **Input**: **Input System Package (v1.11+)**
      * Action Map 設定：`Move` (Vector2), `Reset` (Button), `RotateCamera` (Button - Stretch Goal).

-----

## 2\. 檔案與目錄結構 (Directory Structure)

*統一檔案擺放位置，確保團隊（或未來的你）找得到東西。*

```text
Assets/
├── _Project/               <-- 專案核心資料夾 (與第三方插件區隔)
│   ├── Scripts/
│   │   ├── Core/           <-- 邏輯層 (無 Unity 依賴)
│   │   │   ├── Models/     (GridPoint, BlockType, GridDirection)
│   │   │   ├── Interfaces/ (IGridSystem, ILogicStrategy)
│   │   │   └── Logic/      (AndStrategy, CircuitSystem)
│   │   ├── Unity/          <-- 視覺與控制層 (MonoBehaviour)
│   │   │   ├── Controllers/(BlockController, PlayerInput)
│   │   │   ├── Managers/   (GameManager, GridManager)
│   │   │   └── View/       (VisualEffects, UI, Audio)
│   │   └── Utils/          (GameConstants, Helpers)
│   ├── Prefabs/
│   │   ├── Blocks/
│   │   └── UI/
│   ├── Materials/
│   ├── Levels/             (JSON files)
│   └── Tests/              <-- 測試代碼
```

-----

## 3\. Unity 專案設定規範 (Project Settings Specs)

### 3.1 Layers (圖層)

  * **Layer 6**: `Block` (所有可互動方塊：箱子、邏輯閘、牆)
  * **Layer 7**: `Ground` (地面，用於滑鼠點擊或射線檢測基準)
  * **Layer 8**: `PostProcessing` (用於 Volume)

### 3.2 Tags (標籤)

  * `Block`: 所有動態生成的邏輯元件。
  * `Socket`: 邏輯閘的輸入/輸出接口點 (若需要對位)。

### 3.3 Physics Matrix (物理矩陣)

  * 為了效能與邏輯純淨，規定 **Disable All Collisions** (取消所有物理碰撞)，除非您打算用物理引擎來做推箱子。
  * 僅保留 `Raycast` 需要的檢測。

-----

## 4\. 全域常數定義 (Game Constants)

*SDD 禁止在代碼中出現「魔術數字」。建立靜態類別 `GameConstants.cs`。*

```csharp
public static class GameConstants {
    // 視覺與動畫參數
    public const float MOVE_DURATION = 0.3f; // 移動一格所需秒數
    public const float PUSH_DELAY = 0.1f;    // 推箱子的延遲感
    public const float BLOOM_INTENSITY_ON = 2.5f;
    public const float BLOOM_INTENSITY_OFF = 0.0f;
    
    // 網格參數
    public const float CELL_SIZE = 1.0f;     // 每一格的 Unity 單位寬度
    
    // 音效參數 (Audio Keys)
    public const string SFX_PUSH = "Push";
    public const string SFX_POWER_UP = "PowerUp";
    public const string SFX_WIN = "LevelClear";

    // 層級與標籤
    public const string LAYER_BLOCK = "Block";
    public const string LAYER_GROUND = "Ground";
}
```

-----

## 5\. 系統架構總覽 (System Architecture)

### 5.1 核心設計理念

採用 **MVC (Model-View-Controller)** 變體架構，嚴格區分數據與表現。

  * **Model (邏輯層)**：純 C\# 類別。負責 Grid 運算、電路遞迴、方向性判斷。**不繼承 MonoBehaviour**。
  * **View (視覺層)**：Unity `MonoBehaviour`。負責材質切換、Bloom 發光、DOTween 動畫、音效播放。
  * **Controller (控制層)**：使用 **Input System** 處理輸入，管理 Game Loop。

-----

## 6\. 設計模式規範 (Design Patterns)

### 6.1 策略模式 (Strategy Pattern) - 邏輯閘運算

  * **實作**：針對不同 `BlockType` 實作 `ILogicStrategy` 介面。
  * **重點**：運算需考慮元件的「朝向 (Orientation)」。

### 6.2 觀察者模式 (Observer Pattern) - 邏輯與視覺解耦

  * **Model 層**：`BlockModel` 發送 C\# event。
      * `event Action<bool> OnStateChanged;` (開關燈)
      * `event Action<GridPoint, float> OnPositionChanged;` (移動，參數為新位置與時間)
  * **View 層**：`BlockView` 訂閱事件以更新 Transform 或 Material。

### 6.3 命令模式 (Command Pattern) - 移動系統

  * **實作**：將 `MoveCommand(GridPoint dir)` 封裝為物件，支援未來擴充 Undo 功能。

-----

## 7\. 領域模型定義 (Domain Models)

*資料結構定義，所有子系統必須遵守此標準。*

```csharp
// 邏輯座標
public readonly record struct GridPoint(int X, int Z) 
{
    public static GridPoint Zero => new GridPoint(0, 0);
    // 實作與 Vector3 的轉換擴充方法
}

// 方向定義 (新增) - 用於定義邏輯閘的輸入/輸出方向
public enum GridDirection {
    North = 0, // Z+
    East = 1,  // X+
    South = 2, // Z-
    West = 3   // X-
}

// 訊號狀態
public enum SignalState {
    Low = 0,    // 斷電
    High = 1    // 通電
}

// 元件類型
public enum BlockType {
    Wire, AndGate, OrGate, NotGate, Source, Target, Obstacle, Empty
}
```

-----

## 8\. 資料儲存與關卡格式 (Data & Level Format)

*核心在於資料驅動。新增 `orientations` 以支援邏輯閘轉向。*

### 8.1 關卡設定檔 (Level Schema)

  * **路徑**：`Assets/Resources/Levels/Level_XX.json`

<!-- end list -->

```json
{
  "levelId": 1,
  "gridSize": {"x": 8, "y": 8},
  "layout": [
    "WWWA....",
    "S..W....",
    "....T...",
    "........",
    "OBS.....",
    "........",
    "........",
    "........"
  ],
  "orientations": [
    1,1,1,0,0,0,0,0,
    2,0,0,3,0,0,0,0,
    0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0,
    0,0,0,0,0,0,0,0
  ],
  "mapping": {
    "W": "Wire",
    "A": "AndGate",
    "O": "OrGate",
    "N": "NotGate",
    "S": "Source",
    "T": "Target",
    "B": "Obstacle",
    ".": "Empty"
  }
}
```

*註：`orientations` 陣列長度應等於 grid width \* height，數值對應 `GridDirection` Enum (0=North, 1=East...)。*

-----

## 9\. 邏輯規格表 (Logic Specifications)

*定義所有元件行為。**方向性說明**：預設元件「前方」為輸出端，「後方/側面」為輸入端。*

| 元件類型 | 輸入條件 (Input Logic) | 輸出行為 (Output Logic) | 視覺回饋 | 備註 |
| :--- | :--- | :--- | :--- | :--- |
| **Source** | 無 (忽略輸入) | 恆定 `High` | 永久強光 | 電源 |
| **Wire** | 任一相鄰方塊為 `High` | 輸出 `High` | 凹槽發光 | 無方向性導體 |
| **AND Gate** | **後方與側面**輸入皆為 `High` | **前方**輸出 `High` | 核心亮起 | 具方向性 |
| **OR Gate** | **任一**非前方輸入為 `High` | **前方**輸出 `High` | 核心亮起 | 具方向性 |
| **NOT Gate** | **後方**輸入為 `Low` | **前方**輸出 `High` | 反向器亮起 | 具方向性 |
| **Target** | 任一輸入為 `High` | 觸發 `LevelWin` | 勝利動畫 | |
| **Obstacle**| 無 | 無 | 不發光 | 阻擋移動 |

-----

## 10\. 介面契約 (Interface Contracts)

### 10.1 網格與移動系統 (`IGridSystem`)

```csharp
public interface IGridSystem {
    IBlockEntity GetBlockAt(GridPoint point);
    bool IsWalkable(GridPoint point); // 需包含邊界檢查 (Bounds Check)
    
    // 移動介面：回傳 true 代表移動成功 (包含推箱子)
    // 成功後應觸發 Model 的 OnPositionChanged 事件
    bool MovePlayer(GridDirection direction); 

    void ResetLevel();
}
```

### 10.2 電路運算系統 (`ICircuitSystem`)

```csharp
public interface ICircuitSystem {
    // 觸發全域重算 (傳播邏輯)
    // 實作須知：必須包含 Visited Set 防止無限迴圈 (Loop Protection)
    void Recalculate();

    SignalState[] GetInputsFor(GridPoint point, GridDirection orientation);
}
```

### 10.3 視覺回饋系統 (`IVisualController`)

```csharp
public interface IVisualController {
    void SetPowerVisuals(bool isPowered);
    
    // 結合 DOTween 播放移動
    void PlayMoveAnim(Vector3 targetPos, float duration);
    
    void PlayWinEffect();
}
```

### 10.4 邏輯策略介面 (`ILogicStrategy`)

*策略模式核心，加入方向參數以支援複雜邏輯閘。*

```csharp
public interface ILogicStrategy {
    /// <summary>
    /// 計算輸出結果
    /// </summary>
    /// <param name="inputs">周圍訊號狀態</param>
    /// <param name="myOrientation">元件朝向 (決定 Input/Output 面)</param>
    bool Calculate(SignalState[] inputs, GridDirection myOrientation);
}
```

-----

## 11\. Unity 6 實作細節 (Implementation Detail)

### 11.1 場景與渲染 (URP)

  * **Volume Profile**: Bloom Threshold 0.9, Intensity 1.5\~2.0。
  * **Tonemapping**: ACES。
  * **Materials**: 使用 `MaterialPropertyBlock` 控制 Emission Intensity，避免產生大量 Material Instance。

### 11.2 輸入系統 (Input System)

  * Action Map: `Player` -\> `Move` (Composite Vector2: WASD / Arrows)。
  * 在 `GameManager` 統一監聽輸入，呼叫 `IGridSystem.MovePlayer`。

-----

## 12\. 測試規格與計畫 (Testing Specification)

### 12.1 測試環境設定

  * 建立 `Logic.Tests.asmdef` (僅引用 Core) 與 `Integration.Tests.asmdef`。

### 12.2 單元測試案例 (EditMode Tests)

**A. 邏輯閘策略測試 (含方向性)**

```csharp
[Test]
public void AndGate_CorrectInputs_ReturnsTrue() {
    // 假設 AND Gate 朝北 (North)，輸入應來自 South/West/East
    var strategy = new AndStrategy();
    var inputs = new SignalState[] { SignalState.High, SignalState.High }; // 簡化範例
    
    // 傳入方向參數進行測試
    bool result = strategy.Calculate(inputs, GridDirection.North);
    Assert.IsTrue(result);
}
```

### 12.3 整合行為測試

**A. 移動與事件觸發**

```csharp
[Test]
public void Player_PushBox_TriggersPositionEvent() {
    var player = new BlockModel(BlockType.Player);
    bool eventTriggered = false;
    player.OnPositionChanged += (pos, time) => eventTriggered = true;
    
    // Act: 模擬移動
    player.MoveTo(new GridPoint(0, 1), 0.3f);
    
    // Assert
    Assert.IsTrue(eventTriggered);
}
```

-----

## 13\. 驗收測試清單 (Acceptance Criteria)

1.  **邏輯正確性**：AND/OR/NOT 閘在不同朝向 (N/E/S/W) 下，輸入輸出判定正確。
2.  **電路穩定性**：當電路形成迴圈 (Loop) 時，遊戲不會崩潰 (StackOverflow)，應視為斷路或保持上一狀態。
3.  **視覺同步**：推動箱子時，箱子平滑移動，且移動結束後電路狀態立即更新 (亮/暗)。
4.  **發光效果**：通電物件具有明顯的 Bloom 效果。