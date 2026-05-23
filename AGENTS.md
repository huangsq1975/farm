# MFarm — Agent 工作指引

本檔供 AI 編碼代理與協作者在 **Unity 專案 MFarm** 內變更程式與資產時參考。目標是與既有程式風格一致，並避免破壞序列化、IAP、場景與 Prefab 引用。

---

## 專案概要

| 項目 | 說明 |
|------|------|
| 類型 | 2D 農場／度假村類模擬（含飛行棋、商店、飯店、工人等子系統） |
| 引擎版本 | **Tuanjie Engine 2019.4.29f1**（騰訊微信定製 Unity fork）；`ProjectSettings/` 未提交至 repo，以 `Assets/WX-WASM-SDK-V2/package.json` 所記 `"unity":"2019.4"` 為準 |
| 腳本 | C#（143 個），核心邏輯在 **`Assets/Scripts/`** |
| 目標平台 | **微信小遊戲**（WebGL / WASM）；微信 SDK 位於 `Assets/WX-WASM-SDK-V2/` |
| 渲染管線 | **Built-in**（內建渲染管線），無 URP/HDRP |
| 詳細系統文件 | 見 **`SYSTEMS.md`**（各系統玩法說明、數值表） |
| Claude 開發指引 | 見 **`CLAUDE.md`**（建置流程、架構速覽） |

---

## 技術棧

下列套件根據 `Assets/WX-WASM-SDK-V2/package.json` 及腳本 import 確認；升級或新增套件前須比對 Unity 2019.4 相容性。

- **UI**：`com.unity.ugui`、`com.unity.textmeshpro`（`Assets/TextMesh Pro/`）
- **內購**：`com.unity.purchasing`（`Purchaser` / `PurchaserManager`、`Assets/Resources/BillingMode.json`）
- **補間動畫**：**DOTween Pro**（`Assets/Common/Common/Demigiant/DOTween/`，執行期 `DG.Tweening`）
- **資產工具**：NGTools（`Assets/NGTools/`）
- **錄影工具**：UTJ 系列（`Assets/Scripts/UTJ/`，GIF／MP4／PNG 錄製）
- **微信 SDK**：`Assets/WX-WASM-SDK-V2/`（含 Node.js 建置文件，`com.qq.weixin.minigame` v0.1.1）

變更購買流程、產品 ID 或商店設定時，須與 `BillingMode.json` 及 `Purchaser.eTYPE` 內的常數保持一致。

---

## 目錄與職責

| 路徑 | 說明 |
|------|------|
| `Assets/Scripts/` | 遊戲主邏輯；**`Manager.cs`** 為中央協調者（單例），依序初始化所有子系統 |
| `Assets/Scripts/SWorker/` | 社群分享相關腳本（`SocialWorker`） |
| `Assets/Scripts/UTJ/` | 錄影工具向程式；變更前確認是否影響編輯器或建置 |
| `Assets/Common/` | 共用外掛與資源（含 DOTween）；避免大範圍重構 |
| `Assets/Resources/prefab/` | 動態載入 Prefab，路徑以 `Resources.Load()` 字串為準 |
| `Assets/game.unity` | 主遊戲場景；`Assets/load.unity` 為載入場景 |
| `Assets/WX-WASM-SDK-V2/` | 微信小遊戲 SDK，建置時由 Tuanjie Engine 的匯出工具處理 |

---

## 架構與慣例

### 核心架構

- **中央 `Manager`（單例）**：持有並初始化 `Data`、`Map`、`StoreManager`、`HotelManager`、`WorkerManager`、`WildAnimalManager`、`SugorokuManager`、`Sailo`、`Purchaser` 等所有子系統。新系統若需全域入口，沿用既有引用鏈，勿再平行建立 Singleton。
- **`SingletonMonoBehaviour<T>`**：通用單例基底類，子系統繼承使用。
- **`MonoBehaviourWithInit`**：帶 `Init()` 生命週期步驟的基底類，初始化順序由 `Manager.Awake()` 明確控制。

### 存檔系統（關鍵）

所有持久狀態通過 **`PlayerPrefs`** 儲存，鍵名格式為：

```
eSAVE 枚舉名稱 + 農場後綴（普通農場="" / 度假農場="_1"）
```

- **絕不**在 `eSAVE` 之外另開存檔鍵；新增存檔欄位須先在 `eSAVE` 枚舉中聲明。
- 重命名或刪除 `public` 可序列化欄位前，評估 `[FormerlySerializedAs]` 或資料遷移，否則既有存檔會靜默丟失。
- 兩套農場（`NORMAL` / `RESORT`）的存檔互相獨立，切換時通過 `Data.suf` 後綴隔離。

### 解鎖條件系統

動物、設施、魚類等的解鎖條件統一用 `Data.Condition` 表示，類別（`eCATEGORY`）包括：`LEVEL`、`FACILITY`、`FARMANIMAL`、`WILDANIMAL`、`FISH`、`CUSTOMER`、`SEASON_EVENT`、`HOTEL`、`FARM_TYPE`。條件比對邏輯集中在 `Common.GetCompareData()` 與 `Common.ConditionCompare`，修改解鎖規則請在此處操作。

### 命名慣例

- 枚舉使用短前綴（如 `eType`、`eState`、`eSAVE`）
- 成員變數不統一使用 `m_`，**與正在編輯的檔案風格一致**
- 縮排：多數腳本使用 **Tab**
- 程式碼中的字符串日誌使用 `Utils.Log()`，避免直接 `Debug.Log` 刷屏

### 語言設定

- 遊戲語言由 `Data.eLang`（`JP`、`EN`）控制，根據 `Application.systemLanguage` 自動選擇
- UI 顯示字串（錯誤訊息、標籤等）現已統一使用**簡體中文**（原日語已替換）
- 新增字符串請使用簡體中文

---

## 價格與數值修改

所有解鎖費用、產品售價、經驗獲取公式集中在 **`Price.cs`**（靜態類）。修改數值時：

1. 只改 `Price.cs` 內的常數或數組，不散落到各子系統
2. 注意動態公式（如升星費用根據全體工人等級總和計算）與靜態表格的區別
3. 修改後需同步更新 `SYSTEMS.md` 中的對應數值表

---

## 廣告與 IAP

- **激励視頻**：狀態機 `NONE → LOADING → LOAD_COMPLETED → PLAYING → NONE`，入口為 `Manager.PlayVideo()`，回調在 `videoAdPlayed()` / `videoAdFailedToPlay()`
- **插屏廣告**：每 8 次操作觸發一次，且需距上次 ≥ 180 秒，入口 `Manager.LoadInterstitial()` / `ShowInterstitial()`
- **IAP**：`Purchaser.eTYPE` 定義 6 個商品（去廣告、3 檔捐贈、農場工人包、度假工人包）；購買狀態存於 `Data.purchase[]`
- **微信 SDK 橋接**：`WebMediator.cs` 封裝所有原生 API 調用；目前部分廣告調用已被注釋（`//WebMediator.xxx()`），啟用前確認 SDK 版本

---

## 代理實作建議流程

1. **先讀再改**：從 `Manager.Awake()` 呼叫鏈讀起，確認子系統初始化順序後再動手
2. **查閱 `SYSTEMS.md`**：瞭解各系統的遊戲邏輯與數值，避免破壞平衡
3. **存檔安全**：新增持久欄位 → 先加 `eSAVE` 枚舉 → 再加 `Data.Init()` 讀取 → 再加對應 `Set` 方法
4. **Prefab 路徑**：`Resources.Load()` 的字符串路徑須與 `Assets/Resources/` 下的實際目錄結構一致
5. **Unity API**：以 **2019.4** 為準；不使用 2020+ 新增的 API
6. **範圍控制**：只改與需求直接相關的檔案；不順手重構 `UTJ/`、`WX-WASM-SDK-V2/` 等第三方目錄
7. **溝通語言**：使用者偏好**繁體中文**說明；程式注釋若需新增，與專案既有語言風格一致

---

## 相關文件

| 文件 | 用途 |
|------|------|
| `CLAUDE.md` | Claude Code 開發指引（建置、架構速覽） |
| `SYSTEMS.md` | 各遊戲系統詳細技術說明與數值表 |
| `Assets/WX-WASM-SDK-V2/package.json` | SDK 版本與引擎版本的單一真相來源 |
| `Assets/Resources/BillingMode.json` | 計費模式設定 |

---

*若之後新增測試規範、CI 說明或 `.cursor/rules/`，可在此補一行連結，避免本檔過長。*
