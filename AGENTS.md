# MFarm — Agent 工作指引

本檔供 AI 編碼代理與協作者在 **Unity 專案 MFarm** 內變更程式與資產時參考。目標是與既有程式風格一致，並避免破壞序列化、IAP、場景與 Prefab 引用。

---

## 專案概要

| 項目 | 說明 |
|------|------|
| 類型 | 2D 農場／度假村類模擬（含雙陸棋、商店、飯店、工人等子系統） |
| 引擎版本 | 以 **`ProjectSettings/ProjectVersion.txt`** 為準（目前為 **2019.2.17f1**）；請勿假設其他主版本或 LTS 行為 |
| 腳本 | C#，核心邏輯在 **`Assets/Scripts/`** |
| 渲染 | 本 repo 的 **`Packages/manifest.json`** 未含 URP/HDRP；預設為 **Built-in** 管線，查 Shader／後處理時請對應實際專案設定 |

---

## 技術棧（以 `Packages/manifest.json` 為準）

下列為目前鎖定的主要套件；升級或新增套件前請比對 manifest 與 Unity 2019 相容性。

- **UI**：`com.unity.ugui`、`com.unity.textmeshpro`
- **內購**：`com.unity.purchasing`（例如 `Purchaser` / `PurchaserManager`、**`Assets/Resources/BillingMode.json`**）
- **分析／廣告**：`com.unity.analytics`、`com.unity.ads`
- **2D**：`com.unity.2d.sprite`、`com.unity.2d.tilemap`
- **其他**：`com.unity.timeline`、`com.unity.test-framework`、Legacy **Multiplayer HLAPI**（`com.unity.multiplayer-hlapi`）等

**補間動畫**：**DOTween Pro** 位於 `Assets/Common/Common/Demigiant/DOTweenPro`（執行期使用 `DG.Tweening`）。

變更購買流程、產品 ID 或商店設定時，須與實際商店後台及程式內常數（例如 `kProductIDNonConsumable` 等）一致。

---

## 目錄與職責

| 路徑 | 說明 |
|------|------|
| `Assets/Scripts/` | 遊戲主邏輯；**`Manager`** 為中央協調者（單例式存取、多子系統引用） |
| `Assets/Scripts/SWorker/` | 社群／工人相關腳本 |
| `Assets/Scripts/UTJ/` | 錄影／序列圖等工具向程式（UTJ 風格）；變更前確認是否影響編輯器或建置 |
| `Assets/Common/` | 共用外掛與資源（含 DOTween）；避免無必要的大範圍重構 |
| `Assets/game.unity` | 主要場景之一；場景／Prefab 變更會影響序列化引用，需謹慎 |

---

## 架構與慣例

- **中央 `Manager`**：聚合 `Data`、`Map`、`StoreManager`、`HotelManager`、`Purchaser` 等。新系統若需全域入口，優先沿用既有引用與初始化順序，避免再平行造一套 Singleton。
- **命名**：既有程式常見短前綴列舉（如 `eTYPE`）、`m_` 成員等；**新程式請延續同一檔案／模組的風格**，勿在同一專案內混用另一套 C# 命名規範。
- **縮排**：多數腳本使用 **Tab**；請與正在編輯的檔案一致。
- **序列化**：大量 `public` 欄位供 Inspector 為常態；重新命名或刪除欄位可能造成 **已存檔資料或 Prefab 遺失**，需評估 `[FormerlySerializedAs]` 或資料遷移。
- **日誌**：專案內有 `Utils.Log` 等用法；除錯訊息避免刷屏或洩漏敏感資訊。

---

## Git 與忽略項目

若使用版本控制，常見不提交目錄包含 `Library/`、`Temp/`、`Logs/`、`UserSettings/` 等；實際以專案內 **`.gitignore`**（若有）為準。`*.csproj`／`*.sln` 可能被忽略；IDE 方案檔以本機 Unity 產生為準。

---

## 代理實作建議流程

1. **先讀再改**：從 `Manager`、UI 事件、`Purchaser` 等呼叫鏈讀起，再改行為。
2. **Unity API**：以 **2019.2** 與專案實際套件為準；勿仅凭記憶套用他版或新文件 API。
3. **若已連接 Unity MCP**：變更腳本後應確認編譯與主場景狀態；大型階層查詢請**分頁**，避免一次載入過量 JSON；可依 `read_console`／`editor_state` 確認編譯完成。
4. **範圍控制**：只改與需求直接相關的檔案；不順手重構第三方或整個 `UTJ` 目錄。
5. **溝通語言**：使用者偏好**繁體中文**說明；程式註解若需新增，與專案既有語言一致即可。

---

## 相關設定檔

- `Packages/manifest.json` — 套件版本鎖定
- `ProjectSettings/ProjectVersion.txt` — Unity 版本單一真相來源

---

*若之後新增 `.cursor/rules/`、測試規範或 CI 說明，可在此補一行連結或摘要，避免本檔過長。*
