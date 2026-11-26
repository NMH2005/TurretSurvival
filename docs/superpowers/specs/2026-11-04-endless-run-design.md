# Endless run adjustments

Status: approved by the user on 2026-11-04; one instance per weapon type with unlimited upgrades.

## Purpose

Keep one mouse-controlled main weapon, let the player collect the other weapon types as independent automatic weapons, and make the existing Main Scene a continuing survival encounter containing all six existing enemy prefabs.

## Approaches

- Repeated copies of the same automatic gun would increase weapon count by duplicating instances. The user rejected this in favor of one instance per type.
- Selected approach: one main weapon plus one instance of each other supported type, no automatic-slot limit, and repeatable stat upgrades without a rank cap. An automatic reward matching the main weapon is excluded, so the held gun cannot also appear as an invisible copy.

## Behavior

- Main weapon: require held attack input; release, lost focus, disabled input, and reward selection stop manual fire. Automatic controllers are disabled on manual instances.
- Weapons: offer Rifle, Shotgun, Lightning, Sniper, Laser, and Flamethrower where the type is not already owned. Rifle/sniper use aimed shots, shotgun uses rings, lightning uses chains, and laser/flame use bounded automatic firing pulses.
- Upgrades: retain main damage and shot-interval rewards, add targeted automatic damage and cooldown rewards. A maximum rank of zero means unlimited. Each offered reward must still change a supported stat; minimum firing/cooldown durations remain to prevent a zero-time firing loop. Damage upgrades continue to be repeatable.
- Waves: introduce GreenEnemy1, GreenEnemy2, PurpleEnemy1, PurpleEnemy2, GreenMiniBoss, and PurpleMiniboss in stages across the first three minutes. Each stage has no end time and continues spawning afterward; no victory time limit is added.
- Keep the existing pool and concurrent-enemy limit as runtime safeguards. They do not cap survival time, owned weapon types, or upgrade ranks.

## Scope and verification

Reuse existing scene/UI/prefabs and runtime ScriptableObject copies. No new packages, permanent progression, new enemy art, or evolution system. Preserve unrelated local edits. Per the user's instruction, do not run tests or gameplay; inspect the changed code and serialized references and report that runtime behavior remains unverified.
