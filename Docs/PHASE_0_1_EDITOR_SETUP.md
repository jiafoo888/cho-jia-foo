# Phase 0 & 1 — Unity Editor setup

This repo ships **importable `Assets/` code and docs only**. There is no full Unity project (`ProjectSettings/`, `Packages/`) yet — create one in Unity Hub first, then copy or open this repo’s `Assets/` into it.

## 0. Create a Unity 6 2D project (required once)

1. Open **Unity Hub**.
2. Install a **Unity 6** LTS/stable editor (any current Unity 6.x release is fine).
3. **New project** → template **2D (Built-in Render Pipeline)** or **2D (URP)** — either works for Phase 1 placeholders.
4. Project name suggestion: `ProjectArtillery`.
5. Create the project in a temporary folder, then either:
   - **Preferred:** Replace that project’s empty `Assets/` with this repository’s `Assets/`, and keep this repo as the Git root (copy `ProjectSettings/` / `Packages/` from the Hub project into the repo root and commit them on a follow-up), **or**
   - Open the Hub project and copy this repo’s `Assets/Scripts`, `Assets/ScriptableObjects`, etc. into it.
6. Open the project in the Editor and wait for script compile (Console should be clean).

### Input system choice (Phase 0 decision)

**Choice: Legacy Input Manager (`UnityEngine.Input`).**

| Setting | Value |
|---|---|
| **Edit → Project Settings → Player → Active Input Handling** | **Both** (recommended) or **Input Manager (Old)** |

Reasons for Phase 1:

- No Input Actions asset required.
- Default axes `Horizontal` and `Jump` already map to A/D / arrows and Space.
- Easy to migrate to the new Input System in a later phase.

Do **not** set Active Input Handling to **Input System Package (New)** only, or `Input.GetAxisRaw` / `Input.mousePosition` will fail at runtime.

---

## 1. Create GameConfig asset

1. In the Project window: **Assets → Create → Project Artillery → Game Config**.
2. Save as `Assets/ScriptableObjects/GameConfig.asset`.
3. Leave defaults (move 5, jump 8, aim 5°–175°, aim line 2.5) unless you are tuning.

---

## 2. Build the test scene (manual — no auto scene file)

Create a new scene and save it as:

`Assets/Scenes/Test_MoveAim.unity`

### 2.1 Camera

1. Use the default **Main Camera**.
2. Set **Projection** to **Orthographic**.
3. **Size** ≈ `5`.
4. Position `(0, 0, -10)`.
5. Tag must remain **MainCamera** (required by `PlayerAim` fallback).

### 2.2 Ground

1. **GameObject → 2D Object → Sprites → Square** (or create empty + `SpriteRenderer`).
2. Name: `Ground`.
3. Scale: `(20, 1, 1)`, Position: `(0, -3, 0)`.
4. Add **Box Collider 2D** (should exist on Sprite Square; keep it).
5. Set Layer to **Default** (or create a `Ground` layer and select it on the object).

Optional: tint the sprite a dark green so it reads as floor.

### 2.3 Player

1. **GameObject → 2D Object → Sprites → Square**.
2. Name: `Player`.
3. Scale: `(1, 1, 1)`, Position: `(0, -2, 0)` (resting on the ground).
4. Color: distinct from the ground (e.g. blue).
5. Add components:
   - **Rigidbody 2D**
     - Body Type: **Dynamic**
     - Gravity Scale: `1`
     - Freeze Rotation **Z**: checked
     - Collision Detection: **Continuous**
     - Interpolate: **Interpolate**
   - **Box Collider 2D** (default is fine)
   - **Player Controller** (`ProjectArtillery.Player.PlayerController`)
   - **Player Aim** (`ProjectArtillery.Player.PlayerAim`)
   - **Line Renderer** (optional — `PlayerAim` adds one at runtime if missing)

6. Create empty child **GroundCheck**:
   - Position local: `(0, -0.5, 0)` (bottom of the sprite)
   - Assign to **Player Controller → Ground Check**
7. Assign **Game Config** on both **Player Controller** and **Player Aim**.
8. **Ground Layers**: include the layer used by `Ground` (Default is fine if both use Default).
9. Optional child **Barrel** (empty or thin square) assigned to **Player Aim → Aim Visual** so something rotates with aim.

### 2.4 Walls (optional but useful)

Two tall squares with BoxCollider2D at the sides of the ground so the player cannot walk off forever.

---

## 3. Play-mode test steps

1. Open `Test_MoveAim` and press **Play**.
2. Hold **A** / **D** (or ← / →): player moves left/right.
3. Press **Space** on the ground: one jump; hold Space in air: **no** multi-jump.
4. Move the mouse: aim line updates; on-screen label shows angle.
5. Confirm angles stay within about **5°–175°** (cannot aim straight down).
6. Walk off a ledge: fall with gravity; land and jump again.
7. Console: no spam errors or missing-reference exceptions.

---

## 4. Acceptance checklist (Phase 0–1)

- [ ] Unity 6 project opens and scripts compile.
- [ ] Active Input Handling allows legacy `Input` API.
- [ ] Test scene has ground + player with colliders.
- [ ] Horizontal move works.
- [ ] Jump only when grounded.
- [ ] Mouse aim updates visual line (and optional barrel).
- [ ] Angle display updates and stays in ~5°–175°.
- [ ] Tunables live on `GameConfig` (not hardcoded in multiple places).
- [ ] No projectiles, explosions, networking, or Steam code in this phase.

---

## 5. Known limits (Phase 1)

- No firing, projectiles, damage, or cooldown.
- No facing/mirror rules beyond world-space aim clamp (left-facing remapping comes later).
- Aim uses `Camera.main` if no camera is assigned.
- Ground check is a simple circle overlap — thin platforms / moving platforms not handled.
- On-screen angle uses IMGUI (`OnGUI`), not production UI.
- Full Unity `ProjectSettings/` / `Packages/` are created by Hub, not by this PR, until you commit them.
