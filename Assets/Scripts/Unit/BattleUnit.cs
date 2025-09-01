using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : Movable
{
    public GameObject Projectile;
    public Transform ShootPoint;
    public bool IsRaiderOn = true;
    float _raiderTimer = 0f;
    public Vector3 Destination;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        if (ShootPoint == null)
        {
            ShootPoint = transform.Find("Model/ShootPoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAction();

        if (IsRaiderOn)
        {
            float dt = Time.deltaTime;
            _raiderTimer -= dt;
            if (_raiderTimer <= 0f)
            {
                _raiderTimer = Random.Range(0.1f, 1f);
                UnitBase enemy = FindNearestEnemy();
                if (enemy != null)
                {
                    AttackUnit(enemy);
                }
            }
        }
    }
    public override void Stop()
    {
        base.Stop();
        IsRaiderOn = true;
        // print($"Stop. Destination:{name}/ {Destination}/{targetUnit}/{targetUnit.HP}");
        if (Destination != Vector3.zero && (!targetUnit || targetUnit.HP <= 0))
        {
            if ((Destination - transform.position).sqrMagnitude > 1)
                MoveTo(Destination);
            else
                Destination = Vector3.zero;
        }
    }
    public override void MoveTo(Transform targetMovePoint)
    {
        base.MoveTo(targetMovePoint);
        IsRaiderOn = false;
    }
    public override void TakeDamage(int damage, UnitBase attacker)
    {
        base.TakeDamage(damage, attacker);
        print($"TakeDamage. Destination:{name}/ {Destination}/{targetUnit}/{attacker.name}");
        // if targetUnit is not in attack range, attack who is attacking
        if (!targetUnit || (targetUnit.transform.position - transform.position).sqrMagnitude > attackRange * attackRange || !targetUnit.Aggressive)
            AttackUnit(attacker);
    }
    public override void AttackPosition(Vector3 target)
    {
        base.AttackPosition(target);
        if (astarTile.path.Count > 0) Destination = astarTile.path[astarTile.path.Count - 1];
    }
    public override void OnModelAttack()
    {
        if (UnitType == UnitTypes.Warrior)
        {
            base.OnModelAttack();
            if (!targetUnit || targetUnit.HP <= 0)
            {
                Stop();
            }
        }
        else if (UnitType == UnitTypes.Archer)
        {
            if (targetUnit && targetUnit.HP > 0)
            {
                GameObject obj = Instantiate(Projectile, ShootPoint.position, Quaternion.identity);
                obj.GetComponent<Projectile>().SetTarget(targetUnit.transform, attackDamage, this);
            }
            else
            {
                Stop();
                // animator.SetBool(IsAttackingHash, false);
                // animator.Play(IdleHash);
            }
        }
    }


}
