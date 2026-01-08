## 1. 簡介 (Introduction)
**Logic Pusher** 是一款融合了經典「推箱子 (Sokoban)」機制與「數位電路邏輯」的益智遊戲。玩家在網格化地圖中推動各種邏輯閘（Logic Gates）方塊與導線（Wires），目標是將電力從電源（Source）正確引導至目標點（Target）。

本專案旨在透過直觀的遊戲操作，引導玩家理解基礎數位邏輯運算（AND, OR, NOT）與電路連接概念。

## 2. 安裝 / 編譯指南 (Installation & Build)
### 開發環境
* **Unity Version**: Unity6.2 或更高版本
* **Render Pipeline**: Universal Render Pipeline (URP)
* **平台支援**: PC (Windows/Mac), WebGL

### 依賴插件 (Dependencies)
本專案使用了以下插件，請確保在編譯前已正確導入：
1. **DOTween (HOTween v2)**: 負責方塊位移與 UI 的平滑過渡動畫。
2. **Input System Package**: 處理玩家的鍵盤輸入映射。
3. **TextMeshPro**: 用於高品質文字顯示。
4. **Cinemachine** 

### 編譯步驟
1. 複製本倉庫至本地路徑。
2. 使用 Unity Hub 開啟專案。
3. 在 `File > Build Settings` 中，確保 `Assets/Scenes/Menu.unity` 為第一個場景（Index 0），`Game.unity` 為第二個場景。
4. 點擊 `Build` 並選擇目標路徑即可生成執行檔。

## 3. 操作說明 (Instructions)
* **移動**: 使用鍵盤 `W`, `A`, `S`, `D` 或方向鍵控制角色移動。
* **推動**: 面對方塊並朝其方向移動即可推動。一次只能推動一個方塊。
* **過關條件**: 透過排列方塊，使「Source」輸出的高電位訊號經過邏輯閘運算後，最終傳遞至「Target」。
* **方塊圖例**:
    * **Source (S)**: 持續輸出高電位（1）。
    * **Target (T)**: 接收到高電位時觸發過關。
    * **AndGate**: 兩端輸入皆為 1 時才輸出 1。
    * **OrGate**: 任一端輸入為 1 即可輸出 1。
    * **NotGate**: 反轉輸入訊號。
    * **Wire**: 單純傳遞電力訊號。

