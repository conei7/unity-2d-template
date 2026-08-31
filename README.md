# Unity 2D Template

Unity 2Dゲームをすばやく始めるための個人用スターターテンプレートです。

## Environment

- Unity `6000.5.5f1`
- Universal Render Pipeline (2D Renderer)
- Input System
- [MCP for Unity](https://github.com/CoplayDev/unity-mcp)

## Project structure

```text
Assets/
└─ _Project/
   ├─ Art/             # Sprites, animations, fonts
   ├─ Audio/           # BGM and sound effects
   ├─ Data/            # ScriptableObjects and game data
   ├─ Prefabs/         # Project prefabs
   ├─ Scenes/          # Game scenes and sandbox
   ├─ Scripts/
   │  ├─ Runtime/      # Runtime code
   │  └─ Editor/       # Editor-only tools
   ├─ Settings/        # URP and Input System settings
   └─ Tests/
      ├─ EditMode/
      └─ PlayMode/
```

## Getting started

1. Clone this repository.
2. Open the repository root from Unity Hub using Unity `6000.5.5f1`.
3. Open `Assets/_Project/Scenes/Sandbox.unity`.
4. Update the Company Name, Product Name, package name, and build target for the new game.

Generated directories such as `Library`, `Temp`, `Logs`, and `UserSettings` are intentionally not tracked by Git.
