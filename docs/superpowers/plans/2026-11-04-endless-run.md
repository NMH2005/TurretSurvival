# Endless Run Implementation Plan

> **For agentic workers:** Use superpowers:executing-plans in the current user-authorized checkout. Execute only this approved request. The user explicitly waived tests, builds, and gameplay runs.

**Goal:** Deliver the approved endless encounter, unique weapon loadout, repeatable upgrades, and reliable manual fire.

**Architecture:** Keep existing input actions, weapon controllers, runtime clones, reward cards, and pooled wave spawning. Extend their existing definitions rather than adding another framework.

**Tech Stack:** Unity 6000.3, C#, Input System, Cinemachine, uGUI/TMP. No dependencies.

**Spec:** ../specs/2026-11-04-endless-run-design.md

## Constraints and review focus

- One instance per type across main and automatic weapons; manual fire requires actual held input.
- New attack modes and upgrade enum values must be appended to preserve existing asset serialization.
- No rank or slot cap; keep positive timing floors and prevent numeric overflow from breaking long runs.
- Continuous auto weapons must stop between bounded pulses; pause must freeze timers and damage.
- Six enemy prefabs must resolve correctly and continue spawning after three minutes; preserve pooling and concurrent-enemy safety limits.
- Preserve unrelated local edits; no tests, builds, gameplay, or commits without a separate request.

## Task 1: Manual input and unique loadout

- [x] Read input, all firing overrides, UI callbacks, and automatic controller ownership.
- [x] Make fireInput read InputAction.IsPressed, clear held UI/focus input, and gate all manual firing entry points.
- [x] Disable automatic controllers on main weapons; remove slot limit and exclude any already-owned type.

Files: PlayerInputHandler.cs, WeaponBase.cs, LaserBase.cs, FlamethrowerBase.cs, WeaponSystem.cs, WeaponSelectUI.cs, UpgradeSystem.cs, Main Scene.unity.

## Task 2: Six automatic types and repeatable rewards

- [x] Add timed nearest-target pulses, configure Sniper/Laser/Flamethrower, and preserve the three existing patterns.
- [x] Add targeted automatic damage/cooldown rewards, interpret maxRank <= 0 as unlimited, and remove existing rank caps in assets.
- [x] Register acquisition and upgrade assets in Main Scene; label automatic upgrade cards correctly.
- [x] Protect damage scaling and XP accumulation from overflowing into negative numbers.

Files: WeaponData.cs, AutomaticAttackController.cs, UpgradeData.cs, UpgradeSystem.cs, UpgradeChoiceCard.cs, LevelData.cs, LevelSystem.cs, weapon/reward assets and metadata, Main Scene.unity.

## Task 3: Full endless encounter

- [x] Configure six staged open-ended waves at 0, 30, 60, 90, 120, and 180 seconds.
- [x] Keep pools economical per enemy type; preserve 500 concurrent enemies and recurring minibosses.
- [x] Inspect code paths, GUID/fileID links, and the incremental diff. Record delivered behavior without claiming runtime verification.

Files: Spawner.cs, WaveData.asset, SpawnerData.asset, progress documentation.

## Delivery record

- User approved the design in chat; work stayed in the authorized current checkout.
- Ruling: append enum members and author targeted reward assets so existing references and runtime clone ownership remain intact.
- Ruling: keep timing floors, integer representation safeguards, and concurrent-enemy limits; remove gameplay slot/rank limits as requested.
- Ruling: preserve the user's no-tests instruction. Only source/reference inspection and script diff whitespace checks were performed; no Unity run, build, or test suite.
- Static reference inspection: 20 scene rewards; zero missing/duplicate reward references, zero unresolved weapon targets, six open-ended waves with valid prefab/fileID links.
- Fresh-context read-only review found one P2 issue: saturated damage cards could consume levels without effect. Addressed by checking whether any supported damage field can increase. Damage rounding retains the original float multiplication before ceiling and clamps before conversion to int.
- No commits, pushes, or scene/gameplay execution. See 2026-11-04-endless-run-changes.md for the complete file list.
