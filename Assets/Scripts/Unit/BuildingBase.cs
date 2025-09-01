using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : UnitBase
{
    [Header("Building Properties")]
    [SerializeField] protected bool requiresPower = false;
    [SerializeField] protected bool providesPower = false;
    [SerializeField] protected int powerProvided = 0;
    [SerializeField] protected int powerRequired = 0;
    [SerializeField] protected bool isPowered = false;

    [Header("Production")]
    [SerializeField] protected bool canProduceUnits = false;
    [SerializeField] protected Transform spawnPoint; // Where units will spawn
    [SerializeField] protected float productionProgress = 0f;
    [SerializeField] protected bool isProducing = false;
    public List<Vector3> SurroundingTiles = new List<Vector3>();
    public List<UnitTypes> UnitSpawnList = new List<UnitTypes>();
    public List<UpgradeTypes> UpgradeList = new List<UpgradeTypes>();

    public override void Init()
    {
        base.Init();
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    public virtual bool CanBuildAt(Vector3 position)
    {
        // Check if the building can be placed at the given position
        // This should be implemented in derived classes based on your game's grid system
        return true;
    }

    public virtual void SetPowered(bool powered)
    {
        isPowered = powered;
        // Handle power state changes
        if (!isPowered && requiresPower)
        {
            // Disable building functionality
            DisableBuilding();
        }
        else
        {
            // Enable building functionality
            EnableBuilding();
        }
    }

    protected virtual void DisableBuilding()
    {
        // Disable building functionality when power is lost
        if (isProducing)
        {
            PauseProduction();
        }
    }

    protected virtual void EnableBuilding()
    {
        // Enable building functionality when power is restored
        if (isProducing)
        {
            ResumeProduction();
        }
    }

    public virtual void StartProduction()
    {
        if (!canProduceUnits || !isPowered) return;

        isProducing = true;
        productionProgress = 0f;
    }

    public virtual void UpdateProduction(float progress)
    {
        if (!isProducing) return;

        productionProgress = progress;
        if (productionProgress >= 1f)
        {
            CompleteProduction();
        }
    }
    protected virtual Vector3 GetExitPosition()
    {
        // GoldMine 주변의 빈 위치를 찾아서 반환
        Vector3 basePosition = transform.position;
        Vector3[] possiblePositions = {
            basePosition + Vector3.right * 1.5f,
            basePosition + Vector3.left * 1.5f,
            basePosition + Vector3.up * 1.5f,
            basePosition + Vector3.down * 1.5f,
            basePosition + Vector3.right * 1.5f + Vector3.up * 1.5f,
            basePosition + Vector3.left * 1.5f + Vector3.up * 1.5f,
            basePosition + Vector3.right * 1.5f + Vector3.down * 1.5f,
            basePosition + Vector3.left * 1.5f + Vector3.down * 1.5f
        };

        foreach (Vector3 pos in possiblePositions)
        {
            if (!IsPositionOccupied(pos))
            {
                return pos;
            }
        }

        // 모든 위치가 차있으면 기본 위치 반환
        return basePosition + Vector3.right * 2f;
    }


    protected virtual bool IsPositionOccupied(Vector3 position)
    {
        // 주변에 다른 유닛이 있는지 확인
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<UnitBase>() != null)
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void CompleteProduction()
    {
        isProducing = false;
        productionProgress = 0f;
        // Spawn the produced unit at spawnPoint
    }

    public virtual void PauseProduction()
    {
        isProducing = false;
    }

    public virtual void ResumeProduction()
    {
        if (isPowered)
        {
            isProducing = true;
        }
    }

    // Getters
    public bool RequiresPower() => requiresPower;
    public bool ProvidesPower() => providesPower;
    public int GetPowerProvided() => powerProvided;
    public int GetPowerRequired() => powerRequired;
    public bool IsPowered() => isPowered;
    public bool CanProduceUnits() => canProduceUnits;
    public bool IsProducing() => isProducing;
    public float GetProductionProgress() => productionProgress;

    // Update is called once per frame
    void Update()
    {

    }
}
