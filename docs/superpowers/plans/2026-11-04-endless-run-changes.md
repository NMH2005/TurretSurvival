# Các thay đổi cho màn chơi vô hạn — 2026-11-04

Phạm vi đã duyệt: mỗi loại súng một bản trong toàn bộ loadout; một súng chính điều khiển tay; bỏ giới hạn slot và rank nâng cấp; sáu loại quái sinh liên tục. Không chạy Unity, build, test hay gameplay theo yêu cầu của người dùng.

## Script đã sửa trong yêu cầu này

| File trong Assets/Scripts | Logic thay đổi |
| --- | --- |
| Player/Input/PlayerInputHandler.cs | Đọc trạng thái InputAction.IsPressed thay cho bool giữ từ callback. Chặn input khi pause, mất focus, component tắt, hoặc đang chờ một lần nhấn mới sau UI. |
| WeaponFiniteStateMachine/WeaponBase.cs | Tắt AutomaticAttackController trên súng chính; kiểm tra input thật ở StartFire và trước khi tiêu hao đạn. |
| WeaponFiniteStateMachine/LaserBase.cs | Áp dụng cùng điều kiện điều khiển khi bắt đầu laser. |
| WeaponFiniteStateMachine/FlamethrowerBase.cs | Áp dụng cùng điều kiện điều khiển khi bắt đầu phun lửa. |
| WeaponFiniteStateMachine/LaserWeapon.cs | Ẩn tia laser lúc khởi tạo, chờ lệnh bắn mới hiển thị. |
| Core/WeaponSystem.cs | Bỏ maxAutomaticSlots/HasFreeSlot; chặn nhận súng phụ trùng súng chính và trùng súng phụ đã có. |
| UI/WeaponSelectUI.cs | Xóa input bắn sau khi chọn súng chính để thao tác UI không trở thành lệnh bắn tiếp. |
| Data/WeaponData.cs | Thêm kiểu NearestPulse ở cuối enum và automaticAttackDuration; giữ nguyên chỉ số enum cũ. |
| WeaponFiniteStateMachine/AutomaticAttackController.cs | Chặn khởi tạo controller cho súng chính; thêm đợt bắn laser/lửa có thời hạn, bám mục tiêu, dừng khi hết hạn/mất mục tiêu/tắt controller. |
| Data/UpgradeData.cs | Thêm AutomaticDamage/AutomaticCooldown; maxRank bằng 0 nghĩa là không giới hạn rank. |
| Core/UpgradeSystem.cs | Lọc và áp dụng nâng cấp đúng súng phụ; xóa input quanh UI; chỉ áp dụng rank cap khi maxRank > 0; chặn cooldown vô ích khi đã chạm thời gian hoàn thành đợt bắn; tránh damage tràn thành số âm. |
| UI/UpgradeChoiceCard.cs | Nhãn AUTO DAMAGE và AUTO COOLDOWN cho hai loại thẻ mới. |
| Data/LevelData.cs | Tính yêu cầu XP bằng double, giữ kết quả dương trong miền int để chơi lâu không phát sinh ngưỡng XP âm. |
| Core/LevelSystem.cs | Cộng XP bằng long và cập nhật CurrentExp trước sự kiện lên cấp khi nhận XP vượt ngưỡng. |
| Wave/Spawner.cs | Không chạy thời gian hoặc sinh thêm quái trong lúc pause. |

PlayerPivotHandle.cs, ShotgunBase.cs, LevelUpUI.cs và các file settings/art đã có thay đổi từ trước yêu cầu này; không sửa thêm chúng trong lượt này.

## Scene và asset có sẵn đã sửa

- Assets/Scenes/Main Scene.unity: bỏ trường giới hạn slot cũ và thêm 15 reference phần thưởng, tổng cộng 20 lựa chọn trong catalog.
- Assets/ScriptableObjects/Weapon/Sniper.asset: bắn tự động một phát mỗi bốn giây.
- Assets/ScriptableObjects/Weapon/Laser.asset: đợt bắn hai giây, cooldown năm giây.
- Assets/ScriptableObjects/Weapon/Flamethrower.asset: đợt phun hai giây, cooldown năm giây.
- Assets/ScriptableObjects/Upgrades/MainDamageUpgrade.asset: maxRank = 0.
- Assets/ScriptableObjects/Upgrades/MainFireIntervalUpgrade.asset: maxRank = 0.
- Assets/ScriptableObjects/Upgrades/RecoveryReward.asset: maxRank = 0.
- Assets/ScriptableObjects/Spawner/WaveData.asset: sáu đợt bắt đầu tại 0/30/60/90/120/180 giây; tất cả endTime = -1. Miniboss xanh lặp mỗi 45 giây, tím mỗi 60 giây.
- Assets/ScriptableObjects/Spawner/SpawnerData.asset: pool ban đầu 30 mỗi loại; giữ giới hạn 500 quái cùng lúc.
- Assets/ScriptableObjects/Level/LevelData.asset: bỏ dữ liệu maxLevel cũ không còn được script sử dụng.

## Asset mới

Các file dưới đây nằm trong Assets/ScriptableObjects/Upgrades/. Mỗi file .asset có một file .asset.meta mới cùng tên:

- AcquireSniper.asset
- AcquireLaser.asset
- AcquireFlamethrower.asset
- AutomaticRifleDamage.asset
- AutomaticRifleCooldown.asset
- AutomaticShotgunDamage.asset
- AutomaticShotgunCooldown.asset
- AutomaticLightningDamage.asset
- AutomaticLightningCooldown.asset
- AutomaticSniperDamage.asset
- AutomaticSniperCooldown.asset
- AutomaticLaserDamage.asset
- AutomaticLaserCooldown.asset
- AutomaticFlamethrowerDamage.asset
- AutomaticFlamethrowerCooldown.asset

Damage cộng 10% trên chỉ số runtime hiện tại mỗi lần chọn, làm tròn lên. Cooldown giảm 8% trên thời gian hiện tại mỗi lần chọn đến ngưỡng có tác dụng. Khi thời gian cooldown đã bằng thời gian đợt bắn hoặc 0.05 giây, thẻ cooldown đó không xuất hiện tiếp; thẻ damage vẫn lặp lại.

## Tài liệu mới

- docs/superpowers/specs/2026-11-04-endless-run-design.md
- docs/superpowers/plans/2026-11-04-endless-run.md
- docs/superpowers/plans/2026-11-04-endless-run-changes.md

## Soát tĩnh

- Catalog scene: 20 phần thưởng; không có reference phần thưởng thiếu hoặc trùng.
- TargetWeapon của các thẻ đều trỏ tới asset hiện có.
- Sáu wave đều không có thời điểm kết thúc; các GUID/fileID prefab quái tồn tại.
- Chưa xác nhận biên dịch, độ cân bằng hoặc hành vi thực tế trong Unity.
