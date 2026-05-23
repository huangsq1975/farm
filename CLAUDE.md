# CLAUDE.md

本文件为 Claude Code（claude.ai/code）提供该代码仓库的开发指引。

## 项目概述

这是一款基于 Unity 的农场游戏，目标平台为微信小游戏。使用团结引擎（Unity 2019.4.29f1）构建，通过 WebGL/WASM 导出至微信平台。

## 开发环境

- **引擎：** 团结引擎（Unity 分支）2019.4.29f1
- **语言：** C#（共约 143 个脚本）
- **构建目标：** WebGL → 微信小游戏，导出工具为 `WX-WASM-SDK-V2`
- **主场景：** `Assets/game.unity`；加载场景：`Assets/load.unity`

### 构建方法

直接使用 Unity 编辑器，无 CLI 构建脚本：
1. 用团结引擎 / Unity 2019.4.29f1 打开项目
2. 加载 `Assets/game.unity`
3. 构建 → WebGL，然后使用 `Assets/WX-WASM-SDK-V2/` 中的微信小游戏导出工具

## 架构总览

### 单例 Manager 模式

所有主要系统均为单例 `MonoBehaviour`。根类 `Manager`（`Assets/Scripts/Manager.cs`）是全局协调者，负责广告播放（`PlayVideo`、`ShowInterstitial`）、分享（`TouchShare`）以及整个游戏的启动初始化。

公共基类：
- `SingletonMonoBehaviour<T>` — 泛型单例基类
- `MonoBehaviourWithInit` — 带有显式 `Init()` 生命周期步骤的基类

### 核心系统一览

| 文件 | 职责 |
|------|------|
| `Data.cs` | 全部持久化游戏状态。使用枚举 `eSAVE`（75+ 个键）作为类型安全的存档索引。定义 `eFarmType`（NORMAL/RESORT）和 `eMainType`（MAIN_1/MAIN_2）。 |
| `Event.cs`（84 KB） | 游戏事件调度，含新手引导、开场动画、结局、度假模式等全部流程；是项目最大的文件，修改需谨慎。 |
| `Facility.cs`（68 KB） | 建筑/设施的完整生命周期管理。 |
| `Common.cs` | 共用条件判断工具：季节事件（万圣节、圣诞、夏天）、设施/动物/等级门槛、酒店星级评定。 |
| `MainCharacter.cs` | 玩家移动、动作、割草、建造。 |
| `Map.cs` | 世界地图管理，含网格（SquareInfo）和访问区域（VisitAreaInfo）。 |
| `StoreManager.cs` | 商店操作，含篮子区域（BasketArea）UI 管理。 |
| `HotelManager.cs` | 酒店系统，含房间区域（RoomArea）UI 管理。 |
| `WorkerManager.cs` / `Worker.cs` | NPC 工人 AI，含招募、分配、星级管理（13 名工人 × 4 星级）。 |
| `WildAnimalManager.cs` / `FarmAnimal.cs` | 动物系统。 |
| `SugorokuManager.cs` | 棋盘小游戏。 |
| `Casher.cs`（33 KB） | 货币收费逻辑，处理建筑/动物/树木的添加与删除操作。 |
| `Purchaser.cs` / `PurchaserManager.cs` | 内购（IAP）流程。 |
| `WebMediator.cs` | 微信 API 桥接层。 |
| `SoundManager.cs` | 音频管理。 |
| `TouchManager.cs` | 输入管理。 |

### 关键开发规则

#### 存档系统
- **所有持久化状态必须通过 `Data.cs` 中的 `eSAVE` 枚举存取**，禁止在此枚举之外存储持久数据。
- 读取：`PlayerPrefs.GetInt(eSAVE.XXX.ToString(), 默认值)`
- 写入：通过 `Data` 类对应的 setter 方法

#### 事件系统
- `Event.cs` 是跨系统通信的核心；添加任何新回调前，务必先查阅该文件。
- 事件状态由 `eState` 枚举管理（CHARACTER_SELECT、OPENING、TUTORIAL_* 等约 40 个状态）。
- 修改教程或开场流程时，注意 `#region old` / `#region new` 区块——旧代码已注释保留，新代码为当前生效逻辑。

#### 摄像机图层切换
- 开场及教程期间会频繁切换 `Camera.main.cullingMask` 以只渲染特定图层（Select / Event）。
- `SetLayer(int layer)` 会将背景、设施、酒店、商店等世界对象批量移至目标图层。
- **重要：** 切换摄像机遮罩前，必须确认白色遮罩（`front_white`）已在目标图层，否则会出现黑屏。

#### 微信小游戏性能注意事项
- **避免在关键帧同步调用多次 `Resources.Load()`**，每次均可能触发 WebGL/WASM 层的 I/O 读取，造成多帧卡顿。
- 解决方案：在场景加载完毕、用户等待期间（如选角界面）提前预加载所需 Prefab，利用 `Resources.Load()` 的缓存机制，后续调用直接命中缓存。
- DOTween 序列中的多个 `AppendCallback` 会在同一帧内同步执行；若需跨帧等待（如等待纹理上传 GPU），需插入 `AppendInterval(0.1f)` 以上的间隔。

#### 货币与内购
- 激励视频和插屏广告在 `Manager.cs` 中统一协调。
- IAP 流程走 `Purchaser` / `PurchaserManager`。

#### 本地化
- `Language` 枚举支持 `JP`（日语）和 `EN`（英语）。

### 新手开场流程（Event.cs）

```
Manager.Awake()
  └─ events.Opening()
       └─ SelectCharacter()        // 角色选择界面（cullingMask = "Select"）
            └─ [玩家点击确认]
                 └─ DoOpening()    // 开场动画（cullingMask = "Event"）
                      └─ [动画播放完毕]
                           └─ OnOpeningFinish()  // 恢复正常遮罩，进入教程
                                └─ SetKeepOutGrassCut()  // 割草教程
```

进入 `DoOpening()` 时会同步加载以下 Prefab（已在 `SelectCharacter()` 入口处预加载以避免卡顿）：
- `Prefab/granpa` — 爷爷 NPC
- `Prefab/event_human` — 开场用主角模型
- `Prefab/op_balloon` — 对话气泡
- `Prefab/HollowedCamera` — 圆形聚焦摄像机特效
- `Prefab/skip` — 跳过按钮

### 第三方库

- **DOTween**（`Assets/Common/Common/Demigiant/DOTween/`）— 动画缓动
- **TextMesh Pro**（`Assets/TextMesh Pro/`）— 文字渲染
- **NGTools**（`Assets/NGTools/`）— 资源查找工具
- **UTJ Recorders**（`Assets/Scripts/UTJ/`）— GIF/MP4/PNG 截图录制工具
- **WX-WASM-SDK-V2**（`Assets/WX-WASM-SDK-V2/`）— 微信小游戏官方 SDK
