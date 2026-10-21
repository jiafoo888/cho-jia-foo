# Project Artillery

Realtime 2D artillery prototype (working title). Spec: [`PROJECT_SPEC.md`](PROJECT_SPEC.md).

## Current status

**Phase 0 + Phase 1** — project folder layout, player move/jump, mouse aim with visual feedback.

Not in this phase: projectiles, explosions, destructible terrain, networking, Steam.

## Unity Hub init (required)

This Git repo starts without Unity `ProjectSettings/` / `Packages/`. Create a real project first:

1. Unity Hub → install **Unity 6**.
2. New project → **2D** template → name e.g. `ProjectArtillery`.
3. Merge this repo’s `Assets/` (and docs) into that project, **or** copy the Hub-generated `ProjectSettings/` + `Packages/` into this repo and keep developing here.
4. **Edit → Project Settings → Player → Active Input Handling → Both** (or Input Manager Old).

Phase 1 scripts use the **legacy Input Manager** (`Input.GetAxisRaw`, `Input.mousePosition`, Space jump).

## Scripts added

| Path | Role |
|---|---|
| `Assets/Scripts/Core/GameConfig.cs` | Central tunables (ScriptableObject) |
| `Assets/Scripts/Player/PlayerController.cs` | A/D move, Space jump (grounded only) |
| `Assets/Scripts/Player/PlayerAim.cs` | Mouse aim, 5°–175° clamp, aim line |

## Editor setup & acceptance

See [`Docs/PHASE_0_1_EDITOR_SETUP.md`](Docs/PHASE_0_1_EDITOR_SETUP.md).
