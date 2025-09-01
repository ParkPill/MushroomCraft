using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitBase : MonoBehaviour
{
    public UnitBase targetUnit;
    public Transform SelectedCircle;
    public Transform Model;
    public Identity Team;
    [Header("Basic")]
    public UnitTypes UnitType;
    public MoveTypes MoveType;
    [Header("Basic Stats")]
    public string unitName;
    [SerializeField] public int HPMax = 60;
    [SerializeField] public int HP = 60;
    [SerializeField] public int MPMax = 0;
    [SerializeField] public int MP;
    public int armor = 0;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;
    public float attackSpeed = 1f;
    public Vector2Int size = new Vector2Int(1, 1); // Building size in grid units
    public Vector2Int GetSize() => size;

    // [Header("Resource Costs")]
    // public int mineralCost = 50;
    // public int gasCost = 0;
    // public float buildTime = 10f;
    public UnityAction<UnitBase> OnDie;
    public UnityAction<UnitBase> OnDeselect;

    [Header("State")]
    protected bool isUnderConstruction = false;
    protected float constructionProgress = 0f;
    public bool IsSelected;
    // protected virtual void Start()
    // {
    //     HP = HPMax;
    // }

    [Header("Combat")]
    public float DetectRange = 10f;
    public bool CanAttackGround = true;
    public bool CanAttackAir = false;
    public bool MoveOnGround = true;
    public bool MoveOnAir = false;
    public bool Aggressive = true;
    public virtual void Init()
    {
        Model = transform.Find("Model");
        HandleTeam();
    }
    public UnitCategory GetUnitCategory()
    {
        if (UnitType == UnitTypes.Worker) return UnitCategory.Worker;
        return UnitCategory.Soldier;
    }
    public virtual void ShowTargetted()
    {
        GameObject obj = GameManager.InstantiatePrefab("SelectedCircle", transform);
        obj.transform.localScale = new Vector3(size.x * 0.5f, size.x * 0.25f, 1);
        obj.GetComponent<SpriteRenderer>().color = Team == GameManager.Instance.TheGameScript.MyTeam ? Color.green : Color.red;
        obj.name = "targettedCircle";
        Destroy(obj, 1f);
    }
    void HandleTeam()
    {
        Transform marker = transform.Find("Marker");
        if (marker != null)
        {
            Color color = Team == GameManager.Instance.TheGameScript.MyTeam ? Color.green : Color.red;
            if (Team == Identity.Mutual) color = Color.yellow;
            marker.GetComponent<SpriteRenderer>().color = color;
        }
    }
    public virtual void InteractWith(UnitBase clickedUnit)
    {

    }
    public virtual void AttackUnit(UnitBase target)
    {

    }
    public virtual void AttackPosition(Vector3 target)
    {

    }
    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        UpdateUI();
    }
    public virtual void UpdateUI()
    {
        // Update UI elements here
    }

    public virtual void TakeDamage(int damage, UnitBase attacker)
    {
        int actualDamage = Mathf.Max(1, damage - armor);
        HP -= actualDamage;

        GameManager.Instance.TheGameScript.ShowEffect("SmallHit", transform.position);

        if (HP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        OnDie?.Invoke(this);
        // Handle unit death
        Destroy(gameObject);
    }

    public virtual void Select()
    {
        IsSelected = true;
        // Add selection visual effects here
    }

    public virtual void Deselect()
    {
        IsSelected = false;
        // Remove selection visual effects here
    }

    public virtual void StartConstruction()
    {
        isUnderConstruction = true;
        constructionProgress = 0f;
    }

    public virtual void UpdateConstruction(float progress)
    {
        constructionProgress = progress;
        if (constructionProgress >= 1f)
        {
            CompleteConstruction();
        }
    }

    protected virtual void CompleteConstruction()
    {
        isUnderConstruction = false;
        constructionProgress = 1f;
    }

    // Getters
    public string GetUnitName() => unitName;
    public int GetCurrentHealth() => HP;
    public int GetMaxHealth() => HPMax;
    // public int GetMineralCost() => mineralCost;
    // public int GetGasCost() => gasCost;
    // public float GetBuildTime() => buildTime;
    public bool IsUnderConstruction() => isUnderConstruction;
    public float GetConstructionProgress() => constructionProgress;

    public virtual UnitBase FindNearestEnemy()
    {
        List<UnitBase> enemies = GameManager.Instance.TheGameScript.AllUnitList.FindAll(unit => unit.Team != Team && unit.Team != Identity.Mutual && unit.HP > 0);
        if (enemies.Count > 0)
        {
            UnitBase nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;
            foreach (UnitBase enemy in enemies)
            {
                if (!enemy) continue;
                float distanceSqr = (transform.position - enemy.transform.position).sqrMagnitude;
                if (distanceSqr < nearestDistance && distanceSqr < DetectRange * DetectRange)
                {
                    nearestEnemy = enemy;
                    nearestDistance = distanceSqr;
                }
            }
            return nearestEnemy;
        }
        return null;
    }
}
