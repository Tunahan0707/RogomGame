# Known Issues in RogomGame

## EnemyManager state initialization
- `EnemyManager.currentEnemys` is declared but never initialized before it is used. Both `SelectEnemy` and `SpawnEnemyByID` call `currentEnemys.AddRange(...)`, which will throw a `NullReferenceException` unless the list is manually assigned in the inspector. Even if the list were assigned, `SelectEnemy` accesses `currentEnemy.health` before checking that the lookup succeeded, so a missing entry in the database will also throw before the guard executes.【F:Assets/Scripts/Managers/InGameManagers/FightManagers/EnemyManager.cs†L22-L70】

## FightDataHolder assumes an active Enemy AI
- `FightDataHolder.SaveDatas` blindly invokes `ai.Save()` as soon as the continue or main-menu buttons are pressed. When there is no active enemy (e.g., in non-fight rooms or right after an enemy dies), the cached `ai` reference is null or points to a destroyed component, so saving raises an exception and blocks persistence.【F:Assets/Scripts/Data/FightDataHolder.cs†L31-L47】【F:Assets/Scripts/Managers/SceneManagers/GameSceneManager.cs†L43-L75】

## Runtime assemblies depend on NUnit
- Production scripts under `Assets/Scripts` include `using NUnit.Framework;` even though they do not use any test APIs. Unity strips the NUnit assembly from player builds, so these usings break compilation outside the editor. Removing the unnecessary dependency fixes the build pipeline.【F:Assets/Scripts/Managers/InGameManagers/CardManagers/CardManager.cs†L1-L4】【F:Assets/Scripts/Managers/InGameManagers/FightManagers/PlayerManager.cs†L1-L4】【F:Assets/Scripts/Helpers/RandomRoomSelector.cs†L1-L4】

