# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based farming game (農場ゲーム) targeting WeChat Mini Games. The project uses Tuanjie Engine (Unity 2019.4.29f1) with WebGL/WASM export for the WeChat platform.

## Development Environment

- **Engine:** Tuanjie Engine (Unity fork) 2019.4.29f1
- **Language:** C# (143 scripts)
- **Build target:** WebGL → WeChat Mini Game via `WX-WASM-SDK-V2`
- **Main scene:** `Assets/game.unity`; loading scene: `Assets/load.unity`

### Building

Use Unity Editor directly — there are no CLI build scripts:
1. Open the project in Tuanjie Engine / Unity 2019.4.29f1
2. Load `Assets/game.unity`
3. Build → WebGL, then use the WeChat mini-game export tools bundled in `Assets/WX-WASM-SDK-V2/`

## Architecture

### Manager Singleton Pattern

All major systems are Singleton `MonoBehaviour`s. The root `Manager` class (`Assets/Scripts/Manager.cs`) is the central orchestrator — it owns ad display (`PlayVideo`, `ShowInterstitial`), sharing (`TouchShare`), and bootstraps the rest of the game.

Common base classes:
- `SingletonMonoBehaviour<T>` — generic singleton
- `MonoBehaviourWithInit` — base class with an explicit `Init()` lifecycle step

### Core Systems

| File | Responsibility |
|------|---------------|
| `Data.cs` | All persisted game state. Uses enum `eSAVE` (75+ keys) as typed save-data indices. Also defines `eFarmType` (NORMAL/RESORT) and `eMainType` (MAIN_1/MAIN_2). |
| `Event.cs` (84 KB) | Game event dispatch — largest file; touch with care |
| `Facility.cs` (68 KB) | Buildings/structures lifecycle |
| `Common.cs` | Shared condition helpers: seasons (Halloween, Christmas, Summer), facility/animal/level gating, hotel star ratings |
| `MainCharacter.cs` | Player movement, actions, grass cutting, construction |
| `Map.cs` | World/map management |
| `StoreManager.cs` | Shop operations |
| `HotelManager.cs` | Hotel system |
| `WorkerManager.cs` / `Worker.cs` | NPC worker AI |
| `WildAnimalManager.cs` / `FarmAnimal.cs` | Animal systems |
| `SugorokuManager.cs` | Board-game minigame |
| `Casher.cs` (33 KB) | Currency and payment logic |
| `Purchaser.cs` / `PurchaserManager.cs` | In-app purchases |
| `WebMediator.cs` | WeChat API bridge |
| `SoundManager.cs` | Audio |
| `TouchManager.cs` | Input |

### Key Patterns

- **Save system:** all game state flows through the `eSAVE` enum in `Data.cs`; never store persistent state outside this enum.
- **Event system:** `Event.cs` is the hub for cross-system communication; check it before adding new callbacks.
- **Monetization:** rewarded video and interstitials are coordinated in `Manager.cs`; IAP flows through `Purchaser`/`PurchaserManager`.
- **Localization:** `Language` enum supports `JP` and `EN`.

### Third-party Libraries

- **DOTween** (`Assets/Common/Common/Demigiant/DOTween/`) — tweening/animation
- **TextMesh Pro** (`Assets/TextMesh Pro/`) — text rendering
- **NGTools** (`Assets/NGTools/`) — asset finder
- **UTJ recorders** (`Assets/Scripts/UTJ/`) — GIF/MP4/PNG capture utilities
- **WX-WASM-SDK-V2** (`Assets/WX-WASM-SDK-V2/`) — official WeChat Mini Game SDK
