# TurretSurvival — Game Description

Status: proposed design for review. This document describes intended gameplay, not the project's current feature set. No gameplay implementation is authorized by creating these documents.

## The game

TurretSurvival is a top-down survival shooter combining deliberate aiming and gun upgrades with an expanding arsenal of automatic attacks. The player moves through enemy waves, collects experience, and builds a loadout during each run.

One visible main weapon is controlled by the player. Additional weapons have invisible gun bodies but visible bullets, lightning, beams, flames, or orbiting effects. They attack independently and protect the player during the main weapon's reload or heat recovery.

The core loop is: fight and collect XP → choose a level-up reward → improve the build → survive stronger waves → evolve committed weapons. Aiming should remain valuable throughout the run, particularly against dangerous enemies and when opening escape routes.

## Main weapon

Choose one main weapon before combat. Each non-flamethrower main weapon has ammunition capacity, an interval between shots, and a reload duration. One shotgun shell produces multiple pellets; a proposed two-shell shotgun fires twice before reloading. A held firing input resumes firing after reload completes. Reload automatically when empty, or manually with R; partial magazines are replaced without an ammunition reserve in the first version.

The laser uses a battery measured in firing time and reloads when depleted. The flamethrower uses heat instead of ammunition: releasing fire cools it, while overheating stops fire, imposes a jam, and requires full cooling before firing again. Automatic weapons use their own attack cooldowns rather than magazines or manual heat management.

## Level-up choices and loadout

Each level-up pauses combat and presents up to three distinct eligible rewards. Choose one: improve the main gun, acquire an automatic weapon, upgrade an owned automatic weapon, or acquire/upgrade a passive item. Multiple levels earned together produce consecutive choices.

Provisional prototype limits are one manual weapon, three automatic weapons, three passive items, five automatic weapon ranks, and three item ranks. Owned weapons appear as upgrades, not duplicate acquisitions. Full slots prevent new acquisitions in that category but allow upgrades. These limits are starting design assumptions, not previously confirmed requirements.

Automatic attacks should have distinct roles: aimed bursts for focused damage, shotgun rings for surrounding enemies, chain lightning for groups, flame pulses for close protection, sweeping lasers for sector control, and orbiting attacks for positioning. The first playable slice uses nearest-enemy rifle bursts, shotgun rings, and chain lightning; other patterns follow later.

## Items and evolution

Items explicitly state their scope. Damage can benefit all weapons; projectile speed benefits traveling projectiles; piercing benefits eligible bullets; burning bonuses benefit burn effects. Reload bonuses apply to magazine/battery weapons, and heat recovery bonuses apply to the flamethrower. A Cooling Module can deliberately combine reload, heat recovery, and automatic cooldown benefits.

Prototype items: Power Core (damage), Cooling Module (recovery), and Conductor (lightning chains). Items never occupy weapon slots. Items with no current benefit are excluded from level-up offers.

A rank-five automatic weapon plus its matching item unlocks a guaranteed evolution choice on the next level-up. Evolution replaces that weapon in its existing slot, preserves applicable upgrades, and does not consume the item. Prototype evolutions: rifle + Cooling Module → Twin Burst; shotgun ring + Power Core → Shrapnel Nova; lightning + Conductor → Thunderstorm. Recipes are shown openly.

## First release boundary

Prototype balance values must remain editable in Unity assets. Initial work focuses on reload/heat, independent automatic attacks, level-up selection, three items, and three evolutions. Run duration, final balance, additional attacks, branching gun upgrades, aim-driven synergies, permanent progression, and new art are deferred. All upgrades reset between runs.
