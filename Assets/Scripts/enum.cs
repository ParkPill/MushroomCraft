using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Identity
{
    Mutual,
    Alli0,
    Alli1,
    Alli2,
    Alli3,
    Alli4,
    Alli5,
    Alli6,
    Alli7
}
/// <summary>
///  Starcraft like command types
/// </summary>
public enum Commands
{
    Move,
    Attack,
    Patrol,
    Hold,
    Stop,
    Num0,
    Num1,
    Num2,
    Num3,
    Num4,
    Num5,
    Num6,
    Num7,
    Num8,
    Num9
}
public enum UnitTypes
{
    Worker,
    Warrior,
    Archer,
    GoldMine,
    Castle,
    CastleLair,
    CastleHive,
    Tree,
    Drone,
    Catapult,
    Turret,
    Barracks,
    SupplyDepot,
    EngineeringBay,
    Factory,
}
public enum UpgradeTypes
{
    MeleeAttack,
    RangeAttack,
    GroundDefense,
    AirDefense,
    AttackRange,
    AttackCooldown,
    Lair,
    Hive
}
public enum UnitCategory
{
    Worker,
    Soldier,
    Building
}

public enum OrderTypes
{
    None, Move, Stop, Attack, Patrol, Hold, Skill
}
public enum UnitOrder
{
    Stop,
    Hold,
    Patrol,
    MoveToUnit,
    MoveToPosition,
    AttackUnit,
    AttackPosition
}
public enum WorkerState
{
    EmptyHand,
    GoldHand,
    LumberHand
}
public enum MoveStates
{
    Stand,
    Attacking,
    Run
}
public enum InstanceMessages
{
    NotEnoughCurrency,
    NotEnoughTicket,
    NotEnoughMaterial,
    Purchased,
    PurchaseFailed,
    PurchaseSuccess,
    BuyLimit,
    FailToUse,
    AdsNotReady,
    ServerFailed,
    AdsLimitArrived,
    ServerAdjusting,
    NoInternet
}

public enum CurrencyType
{
    Gold,
    Lumber,
    Oil
}
public enum WorkerCarryingItem
{
    None,
    Gold,
    Lumber
}
public enum MoveTypes
{
    Fixed,
    Walk,
    Fly
}
public enum WorkerJobs
{
    None,
    Gold,
    Tree
}