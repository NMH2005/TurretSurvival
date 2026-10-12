# Leveling and Weapons Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Use superpowers:subagent-driven-development only if the user chooses delegation. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a playable progression loop with one reloadable manual weapon, independent invisible automatic weapons, scoped passive items, and weapon evolutions.

**Architecture:** Keep the existing MonoBehaviour weapon state machines, ScriptableObject definitions, Core components, and XP events. Give each equipped weapon its own runtime data and aim transform; add one progression controller to manage rewards, pause, and eligibility. Reuse projectile and continuous-effect implementations rather than replacing combat or enemy systems.

**Tech Stack:** Unity 6000.3.10f1, C#, Unity Input System 1.18.0, uGUI/TextMeshPro, installed Unity Test Framework 1.6.0. No new packages.

**Spec:** [Game description](../specs/2026-10-12-game-description.md). Both documents are proposals for review; the user requested documents only. Implement only after a subsequent instruction. Prototype values below are explicit defaults to test, not claims about balance.

## Global Constraints

- One visible manual weapon; automatic weapon gun bodies are hidden, while attack effects remain visible.
- Provisional prototype limits: three automatic slots, three passive slots, five automatic ranks, three passive ranks.
- All upgrades reset between runs; shared asset values must never be modified by runtime upgrades.
- Non-flamethrower manual weapons reload; manual flamethrower recovers heat; automatic weapons use independent cooldowns.
- Three distinct choices when available, fewer when fewer eligible rewards exist; one reward per level earned.
- Keep existing enemy, wave, movement, and XP collection systems; change XP level indexing only to establish the intended first-level requirement.
- No new dependencies, art pipeline, permanent progression, or unrelated refactoring.
- Never overwrite unrelated working-tree changes. The inspected scene and several data assets already have local changes.

## Current project and file boundaries

Existing entry points: `Assets/Scripts/Core/WeaponSystem.cs` equips the first weapon manually; `WeaponBase.cs` initializes its state machine in Start; `WeaponAttackState.cs` currently rotates the player's shared pivot. `LevelSystem.cs` raises level-up events, but `WeaponSelectUI.cs` currently only selects a starting weapon. `WeaponData.fireRate` is used as a time interval, so a smaller value means faster fire.

The flamethrower already tracks heat, but its recovery currently uses `timeToOverheat`; the serialized `cooldownTime` is not used by that logic. Laser firing currently has no battery. No test assemblies were found; the test framework is installed.

New code stays in existing folders: weapon runtime/ammunition helpers in `Assets/Scripts/WeaponFiniteStateMachine/`; reward/item definitions in `Assets/Scripts/Data/`; progression ownership in `Assets/Scripts/Core/`; HUD and reward UI in `Assets/Scripts/UI/`. Tasks below name all new files. Do not create a second general-purpose weapon framework.

## Review Focus

1. Two instances sharing an asset: upgrading one must leave the other and future runs unchanged (Task 1).
2. Automatic aim while the mouse moves: automatic weapons must not rotate the held gun or each other (Task 3).
3. XP overflow while paused: every earned level must receive exactly one choice; the final choice restores the prior time scale (Task 4).
4. Held fire, reload, heat, and pause overlap: no firing while unavailable and no timer progress while paused (Task 2).
5. Full or maxed loadouts: no dead cards, duplicate rewards, lost slots, or repeated evolutions (Tasks 5–6).

## Verification convention

Use Unity Test Runner: Window → General → Test Runner → EditMode or PlayMode → Run All. Filter by the named fixture for each task. A failing-first run must report the described assertion failure (or missing API before implementation); the passing run must report zero failed tests. UI and scene checks use a disposable test scene or a copy under `Assets/Tests/Fixtures/`; avoid opening the live scene for automated mutation.

For pure helper tests, use an Editor test assembly at `Assets/Tests/EditMode/TurretSurvival.EditModeTests.asmdef`, referencing `Assembly-CSharp` through reflection where necessary. Unity asmdef assemblies cannot directly reference predefined `Assembly-CSharp`; do not introduce a project-wide assembly migration just to add tests. Use a small reflection fixture helper at `Assets/Tests/EditMode/TestAccess.cs`. PlayMode fixture testing can use `Assets/Tests/PlayMode/WeaponProgressionValidation.cs` as an editor-invoked validation runner, outside an asmdef, to construct components, enter Play Mode, assert behavior, and emit a failure result. Do not claim a batch test run passed without reading its result/log.

### Task 1: Isolate equipped weapon data

**Files:** Create `Assets/Scripts/WeaponFiniteStateMachine/WeaponRuntimeData.cs`. Modify `Assets/Scripts/WeaponFiniteStateMachine/{WeaponBase,ShotgunBase,LaserBase,FlamethrowerBase,LaserWeapon}.cs`, `Assets/Scripts/Projectile/{Projectile,ChainLightningProjectile,FlamethrowerWeapon}.cs`, and `Assets/Scripts/Core/WeaponSystem.cs`. Test: `Assets/Tests/EditMode/WeaponRuntimeDataTests.cs` plus the test setup above.

**Interfaces:** Produce `WeaponRuntimeData.Create(WeaponBase source) : WeaponRuntimeData`, `Dispose() : void`, and typed properties `Weapon`, `Projectile`, `Shotgun`, `Laser`, `Flame`, `Chain` containing clones of the source's present definitions. Add `WeaponBase.Initialize(WeaponRuntimeData data, bool isManual) : void` and `RuntimeData : WeaponRuntimeData`. Add `Projectile.Initialize(ProjectileData data) : void` and `ChainLightningProjectile.Initialize(ProjectileData projectile, ChainLightningData chain) : void`; continuous effects accept their matching runtime definition through `Initialize`. Keep prefab serialization as the template source.

- [x] Write `CloneDoesNotChangeTemplateOrSibling`: change clone A's damage from 10 to 15; assert template and clone B remain 10. Verify distinct weapon definitions can reference the same projectile template safely.
- [x] Run the fixture and record the failing result.
- [x] Create the minimal typed clone holder. Initialize equipped instances before Start; all states and firing paths read runtime definitions. Initialize spawned bullets immediately after Instantiate. Snapshot projectile/chain values for spawned attacks so later upgrades cannot change in-flight damage or leave references to disposed runtime data. Dispose owned clones on weapon destruction; dispose projectile snapshots when projectiles/effects finish. Implementation uses scalar snapshots with automatic value lifetime; see the execution ledger.
- [x] Run EditMode tests and a PlayMode smoke check for rifle, shotgun, laser, flame, and chain damage. Restart the run and verify original stats return. Verified via isolated Unity test runs: 9 EditMode and 5 PlayMode tests passed.
- [x] Commit only this task's files, including Unity metadata: `feat: isolate runtime weapon data`.

### Task 2: Manual ammunition, battery, and heat recovery

Implementation delivered for user testing. User explicitly waived further tests; no Task 2 tests or Unity build were run.

**Files:** Create `Assets/Scripts/WeaponFiniteStateMachine/WeaponAmmo.cs` and `Assets/Scripts/UI/WeaponStatusUI.cs`. Modify `Assets/Scripts/Data/WeaponData.cs`, `Assets/Scripts/WeaponFiniteStateMachine/{WeaponBase,ShotgunBase,LaserBase,FlamethrowerBase}.cs`, `Assets/Scripts/Projectile/FlamethrowerWeapon.cs`, `Assets/Scripts/Player/Input/PlayerInputHandler.cs`, `Assets/InputSystem_Actions.inputactions`, and relevant weapon assets/prefabs. Tests: `Assets/Tests/EditMode/WeaponAmmoTests.cs`, PlayMode validation runner.

**Interfaces:** Add serialized `magazineCapacity`, `reloadDuration`, and laser `batteryDuration` to WeaponData. `WeaponAmmo(int capacity, float reloadDuration)` exposes `CurrentAmmo`, `IsReloading`, `TryConsumeShot() : bool`, `TryReload() : bool`, `Tick(float deltaTime) : void`. Add `WeaponBase.TryReload() : bool` and `PlayerInputHandler.OnReloadInput(InputAction.CallbackContext context) : void`. HUD reads ammo/reload or normalized battery/heat from the equipped manual weapon.

- [ ] Skipped at user request: write `TwoShellShotgunConsumesPerShot` and `ReloadCompletesOnce` tests.
- [ ] Skipped at user request: run the failing test fixture.
- [x] Gate manual firing centrally before calling the existing firing method. Empty magazine starts reload immediately; R replaces a partial magazine, with unlimited reserve and no cancel in this version. Add a Reload action bound to R and wire the existing PlayerInput notification mode. Use scaled delta time for all timers. Prototype rifle: 12 shots/1.5-second reload; shotgun: 2 shells/1.2-second reload; sniper: 1 shot/1.5-second reload; lightning: 4 shots/1.5-second reload. Retain existing shot intervals initially.
- [x] Give laser 3 seconds of battery and 1.5-second reload; drain only while firing and stop the beam immediately on depletion. Retain flame heat buildup/jam settings; cool using `FlamethrowerData.cooldownTime`, clamp heat, require zero heat after overheating, and avoid repeatedly restarting its damage tick timer. Automatic mode bypasses these manual resource rules.
- [ ] User testing: held fire resumes after reload, no damage during reload/jam, releasing flame cools it, and paused reload/battery/heat timers remain unchanged. Verify visible HUD for each resource type.
- [x] Commit scoped files: `feat: add main weapon reload and recovery`.

### Task 3: Independent invisible automatic attacks

**Files:** Modify `Assets/Scripts/Core/WeaponSystem.cs`, `Assets/Scripts/WeaponFiniteStateMachine/{WeaponBase,ShotgunBase}.cs`, `Assets/Scripts/WeaponStates/SubStates/{WeaponSearchState,WeaponAttackState}.cs`, `Assets/Scripts/Data/WeaponData.cs`, and weapon prefabs/assets. Create `Assets/Scripts/WeaponFiniteStateMachine/AutomaticAttackController.cs`. Test: PlayMode validation runner.

**Interfaces:** WeaponSystem exposes `ManualWeapon : WeaponBase`, `AutomaticWeapons : IReadOnlyList<WeaponBase>`, `TryEquipManual(WeaponData data) : bool`, `TryEquipAutomatic(WeaponData data) : bool`. Keep `EquipWeapon` as the starting UI compatibility entry point until Task 4 migrates it. Add `AutomaticAttackPattern` enum with `NearestBurst`, `RadialBurst`, `NearestChain`. `AutomaticAttackController` consumes runtime definitions and drives the automatic attack timer; it never reads mouse input or rotates PlayerPivotHandle.

- [ ] Build a failing PlayMode fixture: one manual plus two automatic weapons, opposite target directions; assert mouse aim remains unchanged by automatic firing, auto directions are independent, manual resource remains untouched, and a fourth automatic acquisition fails.
- [ ] Give each automatic weapon a separate player-centered transform, independent local aim, and serialized gun-body renderer references. Hide only those renderers; keep fire points, collisions, beam/particle/projectile objects alive. Stop the existing automatic state path from firing a second time alongside the controller.
- [ ] Author rifle nearest-target bursts (3 shots, 0.1-second intra-burst interval, 3-second start-to-start cooldown), shotgun ring (8 pellets, 3-second cooldown), and nearest-target lightning (4-second cooldown). Rifle/lightning wait when there is no target and fire when one appears; radial shotgun fires regardless. Reacquire invalid targets; spread target searches across intervals rather than scanning every frame.
- [ ] Run the fixture, then check simultaneous effects and movement. Existing automatic laser/flame cannot remain endlessly active: keep them out of this prototype reward catalog until their timed sweep/pulse task is implemented.
- [ ] Commit scoped files: `feat: add independent automatic attack patterns`.

### Task 4: Queue level-up choices and preserve pause

**Files:** Create `Assets/Scripts/Core/UpgradeSystem.cs`, `Assets/Scripts/Data/UpgradeData.cs`, `Assets/Scripts/UI/{LevelUpUI,UpgradeChoiceCard}.cs`. Modify `Assets/Scripts/Core/LevelSystem.cs`, `Assets/Scripts/UI/WeaponSelectUI.cs`, and `Assets/Scenes/Main Scene.unity` only for intended wiring. Tests: `Assets/Tests/EditMode/UpgradeQueueTests.cs`, PlayMode validation runner.

**Interfaces:** `UpgradeData` identifies reward category, target weapon/item, rank, icon, description, and supported stat changes. `UpgradeSystem.QueueLevelUp(int level) : void`, `GetChoices(int count) : IReadOnlyList<UpgradeData>`, `TryApply(UpgradeData choice) : bool`; `LevelUpUI.Show(IReadOnlyList<UpgradeData> choices) : void`. Task 5 supplies detailed eligibility. For this task, provide a small catalog of owned-main-gun damage and interval upgrades with explicit rank caps.

- [ ] Write a failing queue test: one XP grant earns three levels; three choices apply exactly once; repeated clicks do not apply twice; closing one panel does not discard remaining levels. Starting player level is 1 and first required XP matches LevelData's baseExpToLevel.
- [ ] Subscribe/unsubscribe to EventsManager.OnLevelUp through lifecycle methods. Save prior time scale when the first choice opens, pause, process one selection at a time, restore prior time scale after the last. Ignore combat input while the panel is active and clear pending requests on run teardown/death. Preserve preexisting pause rather than forcing time scale to 1.
- [ ] Migrate starting selection to TryEquipManual; use separate UI for upgrades. Replace fixed-index setup with up to three dynamic distinct cards. Validate choice eligibility again at click time. Fallback when nothing remains is Repeatable Recovery: restore 10% maximum health, or grant 5% run damage if already full health.
- [ ] Run queue tests and verify paused waves, movement, bullets, reload, and heat do not advance; cards remain clickable; one overflowing XP pickup yields all choices. Check XP bar and first-level threshold after indexing change.
- [ ] Commit scoped files: `feat: add queued level-up reward selection`.

### Task 5: Weapon ranks and scoped passive items

**Files:** Create `Assets/Scripts/Data/PassiveItemData.cs`. Modify `UpgradeData.cs`, `UpgradeSystem.cs`, `WeaponSystem.cs`, and runtime firing/effect files from Task 1 as needed. Create definitions under `Assets/ScriptableObjects/Upgrades/` and `Assets/ScriptableObjects/Items/`. Tests: `Assets/Tests/EditMode/UpgradeEligibilityTests.cs` and `PassiveModifierTests.cs`.

**Interfaces:** `UpgradeSystem.GetWeaponRank(WeaponData weapon) : int`, `GetItemRank(PassiveItemData item) : int`. PassiveItemData contains stable identity, supported weapon capabilities, maximum rank, and per-rank modifiers. GetChoices and TryApply remain the public reward APIs.

- [ ] Write failing assertions for no duplicate acquisition, three-slot rejection, max-rank exclusion, and no Conductor offer without chain-capable damage. Power Core raises damage for both bullets and continuous ticks; Cooling Module affects applicable reload/recovery/cooldown fields; neither changes projectile templates or already-spawned attacks.
- [ ] Add rank-one automatic acquisition and ranks 2–5. Rifle: +1 burst shot, cooldown ×0.85, damage ×1.20, +1 pierce. Shotgun ring: +2 pellets, cooldown ×0.85, damage ×1.20, +1 pierce. Lightning: +1 chain, cooldown ×0.85, +1 chain, chain damage ×1.20. Each entry is the next rank's cumulative unlock; assets hold values.
- [ ] Author three-rank items. Per rank, Power Core adds 10% base damage; Cooling Module reduces reload, heat recovery, and automatic cooldown by 8% of base duration; Conductor adds one chain target. Sum contributions before applying them once to base runtime copies; round damage to nearest integer, minimum 1. Do not repeatedly multiply already-modified data when refreshing stats. Heat jam and manual shot interval are unaffected by Cooling Module.
- [ ] Generate unique eligible choices: eligible evolutions first (Task 6), then at least one owned-weapon upgrade when available, then random eligible rewards. Main-gun cards initially have three ranks each: +10% base damage, interval −8% of base, and recovery −8% of base; recovery changes reload or flame cooling according to capability. Keep effective interval/cooldown/recovery at least 0.05 seconds.
- [ ] Run fixtures and verify each UI description names its affected weapons. Acquire an item before another eligible weapon; verify the new weapon inherits the item bonus. Restart and verify all ranks reset.
- [ ] Commit scoped files: `feat: add weapon ranks and scoped passive items`.

### Task 6: Three automatic evolutions

**Files:** Create `Assets/Scripts/Data/EvolutionData.cs` and `Assets/Scripts/Projectile/ShrapnelProjectile.cs`. Modify UpgradeSystem, WeaponSystem, AutomaticAttackController, and runtime projectile initialization. Create definitions under `Assets/ScriptableObjects/Evolutions/` and required projectile prefabs. Tests: `Assets/Tests/EditMode/EvolutionEligibilityTests.cs`, PlayMode validation runner.

**Interfaces:** `EvolutionData` references base weapon, required weapon rank, required item/rank, and evolved attack definition. `WeaponSystem.TryEvolve(WeaponBase weapon, EvolutionData evolution) : bool` replaces behavior in-place or atomically swaps the instance in the same list position. Original weapon identity remains available for rank/recipe lookup.

- [ ] Write failing assertions: rank 4 cannot evolve; rank 5 without its item cannot evolve; rank 5 with item rank 1 receives an evolution on the next choice screen; a full loadout can evolve; evolution cannot repeat. Multiple eligible evolutions are offered before ordinary rewards, up to three cards; unchosen recipes remain eligible.
- [ ] Implement Twin Burst (two sequential rifle bursts), Shrapnel Nova (each ring pellet spawns two smaller non-recursive fragments on its first valid hit, each dealing 50% parent damage), and Thunderstorm (two distinct initial enemies when possible, otherwise one, each starting a chain). Retain slot, weapon identity/rank, passive inventory, and applicable modifiers. Reset the attack timer to a full cooldown on evolution to prevent accidental immediate double fire.
- [ ] Show recipe requirements and evolved behavior on cards. If replacement construction fails, retain the old instance and leave the choice unapplied. Ensure temporary runtime assets and old effects are cleaned up safely.
- [ ] Run eligibility tests and PlayMode checks for fragments that cannot recursively split, distinct lightning targets, item preservation, and stable slot count.
- [ ] Commit scoped files: `feat: add automatic weapon evolutions`.

### Task 7: Integrate the playable slice and tune

**Files:** Intentional wiring only in `Assets/Scenes/Main Scene.unity`, weapon/projectile prefabs, and the new reward/item/evolution assets. Update this plan's checkboxes and add measured tuning notes to the game description if requested.

- [ ] Run all added EditMode tests and PlayMode validation; require zero failures and no new Unity compile/runtime errors. Review the diff for unrelated scene/asset changes.
- [ ] Play a temporary accelerated-XP run: select each main weapon, empty/recover its resource, acquire all three automatic patterns, reach full slots, upgrade an item, evolve an attack, and restart. Restore normal XP settings afterward.
- [ ] Play normal XP progression. Measure levels reached, reload downtime, manual/automatic damage contribution, and time to first evolution; tune assets against the actual wave/XP economy. Do not promise a fixed run length before it is selected and tested.
- [ ] Confirm screen readability: visible held gun, invisible auto gun bodies, distinguishable damage effects, clear ammo/heat status, legible descriptions, and successful movement/aim after each choice closes.
- [ ] Commit only verified integration: `feat: integrate survival progression prototype`.

## Later work, after the slice is fun

Add timed laser sweeps, flame pulses, and orbiting attacks one at a time with targeting/cooldown checks. Add projectile-speed, area, pierce, and burning items only when their capabilities and UI descriptions are defined. Branching gun upgrades, manual evolution, marking/aim synergies, rerolls, and persistent unlocks each need a separate scope decision; they are not prerequisites for this slice.

## Execution handoff

Recommended approach: implement sequentially in the current session after review, because tasks share weapon runtime and progression interfaces. No implementation, test execution, commits, or scene changes were performed when writing these documents. Review the provisional limits, automatic patterns, main-weapon resource defaults, and evolution recipes before requesting implementation.
