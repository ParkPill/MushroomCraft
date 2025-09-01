using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBuilding : BuildingBase
{
    
    public GameObject Projectile;
    public Transform ShootPoint;
    public bool IsRaiderOn = true;
    float _raiderTimer = 0f;
    public Vector3 Destination;
    public float CoolDownTimer = 0f;
    public float CoolDown = 1f;
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
        if (CoolDownTimer > 0f)
        {
            CoolDownTimer -= Time.deltaTime;
            return;
        }
        if (targetUnit == null)
        {
            UnitBase enemy = FindNearestEnemy();
            if (enemy != null)
            {
                AttackUnit(enemy);
            }
        }
        else {
            AttackUnit(targetUnit);
        }
    }
    public override void TakeDamage(int damage, UnitBase attacker)
    {
        base.TakeDamage(damage, attacker);
        print($"TakeDamage. Destination:{name}/ {Destination}/{attacker.name}");
        // if targetUnit is not in attack range, attack who is attacking
        if (!attacker || (attacker.transform.position - transform.position).sqrMagnitude > attackRange * attackRange || !attacker.Aggressive)
            AttackUnit(attacker);
    }

    public override void AttackUnit(UnitBase target)
    {
        base.AttackUnit(target);
        if (CoolDownTimer > 0f)
        {
            return;
        }
        CoolDownTimer = CoolDown;
        if (targetUnit && targetUnit.HP > 0)
        {
            GameObject obj = Instantiate(Projectile, ShootPoint.position, Quaternion.identity);
            obj.GetComponent<Projectile>().SetTarget(targetUnit.transform, attackDamage, this);
        }
        else
        {
            targetUnit = null;
        }
    }

}
