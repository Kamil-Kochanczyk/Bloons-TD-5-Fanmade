using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * This file specifies some custom types that are used globally in the project
 */

public enum Difficulty : byte
{
    Easy,
    Medium,
    Hard
}

public enum BloonType : byte
{
    Red = 0,
    Blue = 1
}

public enum TowerType : byte
{
    DartMonkey = 0,
    TackShooter = 1
}

public enum ItemType : byte
{
    DartMonkeyProjectile = 0,
    TackShooterTacks = 1
}

public enum TargetPriority : byte
{
    First,
    Last,
    Close,
    Strong
}

public interface IBloon
{
    BloonType BloonType { get; }
    BloonType ChildType { get; }
    byte ChildrenCount { get; }
    float DefaultSpeed { get; }
    ushort HealthPoints { get; }
    ushort LivesWorth { get; }
    bool IsCamo { get; }
    bool IsRegrow { get; }
    bool IsSlowedDown { get; }
    bool IsFrozen { get; }
    bool IsDizzy { get; }

    bool TryPop(GameObject popper);
}

public interface ITower
{
    TowerType TowerType { get; }
    ItemType UsedItem { get; }
    float BreakTime { get; }
    GameObject BloonTarget { get; }
    bool IsPlacedOnMap { get; }
    TargetPriority TargetPriority { get; }
    uint KilledBloons { get; }
    bool CanSeeCamo { get; }

    IEnumerator Behave();

    void SetNewTarget(GameObject newTarget);
    void PlaceOnMap();
    void SetNewTargetPriority(TargetPriority newTargetPriority);
    void IncreaseKilledBloons(uint killedBloons);
    void EnableToSeeCamo();
}

public interface IItem
{
    ItemType ItemType { get; }
    GameObject Owner { get; }
    GameObject BloonTarget { get; }
    ushort Damage { get; }
    bool IsExplosive { get; }
    float BloonSlowingFactor { get; }
    float BloonSlowingDuration { get; }
    bool CanFreezeBloons { get; }
    float FreezeBloonsDuration { get; }

    void PerformAction();

    void SetNewOwner(GameObject newOwner);
    void SetNewTarget(GameObject newTarget);
}
