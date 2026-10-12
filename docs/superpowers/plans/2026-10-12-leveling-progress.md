# Execution ledger — plan: 2026-10-12-leveling-and-weapons.md

## Task 1

- Base commit: 823ed3cea34cd75e7120b601cdcbb58163aea33e.
- User authorized editing the current Unity checkout and requested exactly one task per turn. Tasks 2–7 remain pending.
- Pre-flight: Tasks 2, 3, 5, and 6 consume Task 1's RuntimeData. Existing field names and prefab references are preserved; runtime definitions are initialized before state construction.
- Ruling: use scalar snapshots for spawned projectile and lightning-chain parameters rather than creating ScriptableObjects per attack. This avoids asset allocations and lifetime coupling; the snapshots must include every damage, movement, lifetime, piercing, and chain parameter.
- Ruling: ChainLightningEffect must also change because its delayed coroutine currently retains a projectile's data reference after the projectile dies. Copy parameters before starting the coroutine.
- Ruling: use native PowerShell and this durable ledger instead of the skill's shell-specific workspace scripts. Work stays in the user-approved checkout; no separate worktree is created.
- Ruling: use a reflected PlayMode test assembly rather than a custom editor validation runner. The installed Unity Test Framework owns setup/results and exercises actual components; reflection avoids migrating the gameplay assembly. Test helper scripts are editor-only or test-only.
- Initial direct batch run was blocked because the user's editor has the project open. Validation uses a temporary project under Temp/Task1Validation/Project with copied scripts, prefabs, data, settings, and installed packages; the user's editor was not closed.
- RED: Temp/Task1Validation/Red.xml — 9 tests failed on the expected assertion: missing WeaponRuntimeData type.
- GREEN EditMode: Temp/Task1Validation/GreenEditMode.xml — 9 passed, 0 failed.
- GREEN PlayMode: Temp/Task1Validation/GreenPlayMode.xml — 5 passed, 0 failed. Actual damage checks cover rifle, shotgun, laser, flamethrower/burn, and delayed lightning chains after weapon/projectile destruction.
- Self-review: all weapon states now consume the runtime Weapon definition; specialized firing paths consume the matching clone; projectiles and chains copy all current data fields. Existing prefab/data serialization remains compatible. Script diff whitespace check passed.
- Task 1 implementation and validation complete; commit evidence follows. Tasks 2–7 were not started.

## Task 2

- Base commit: abee1c7. Implemented in the user-approved current checkout.
- User changed the workflow: no further tests; the user will test gameplay. Task 2 adds no tests and runs no Unity build, EditMode, or PlayMode validation. Completion means implementation delivered, not verified gameplay.
- Ruling: gate projectile firing through a shared resource helper called by both Fire implementations. This prevents direct Fire calls from bypassing reload, while preserving the existing StartFire interval and one-shell-per-shotgun-shot behavior.
- Ruling: add WeaponSystem.ManualWeapon now because reload input and HUD need the same lookup. This is a small shared accessor; Task 3's automatic loadout changes remain deferred.
- Ruling: wire the existing UnityEvent input mode and author a TMP HUD in Main Scene. Existing scene edits are preserved, and the existing font is reused; no runtime UI scaffolding or new art is added.
- Added WeaponAmmo and WeaponStatusUI. Modified WeaponData, WeaponBase, ShotgunBase, LaserBase, LaserWeapon, FlamethrowerBase, FlamethrowerWeapon, WeaponSystem, and PlayerInputHandler.
- Authored magazine/reload defaults for Rifle (12/1.5s), Shotgun (2/1.2s), Sniper (1/1.5s), Lightning (4/1.5s), and Laser (3s battery/1.5s reload). Runtime recovery reads Task 1's private data; these asset edits define starting configuration.
- Manual magazines auto-reload when empty; R reloads partial magazines/battery; held fire resumes after completion. Automatic modes bypass ammo/battery/heat. Reload, battery drain, heat recovery, and continuous damage updates stop while paused.
- Flamethrower cooling now uses cooldownTime, honors overheat jam followed by full cooling, and repeated StartFlame calls do not restart damage ticks.
- Task 2 implementation delivered for user testing. Tasks 3–7 remain pending.
