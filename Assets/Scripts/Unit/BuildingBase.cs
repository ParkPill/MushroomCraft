using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

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
    public List<Sprite> SmokeSprites = new List<Sprite>();
    public Transform SmokePoint;
    public float SmokeInterval = 1.5f;
    private float _smokeTimer = 0f;
    public float SmokeSpeed = 1f;
    [Header("UI")]
    public Text infoText;
    public bool IsRunning = false;
    public override void Init()
    {
        base.Init();
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
        if (SmokeSprites.Count > 0)
        {
            SmokePoint = transform.Find("Model/SmokePoint");
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
    public void UpdateSmoke(float dt)
    {
        if (!IsRunning)
        {
            return;
        }
        _smokeTimer += dt;
        if (_smokeTimer >= SmokeInterval)
        {
            _smokeTimer -= Random.Range(0.5f, 1.0f) * SmokeInterval;
            CreateSmoke();
        }
    }
    public void RunBuilding(bool run)
    {
        IsRunning = run;
        Model.GetComponent<Animator>().Play("running", -1, 0);
    }
    public void CreateSmoke()
    {
        // GameObject smoke = Instantiate(Resources.Load<GameObject>("Prefab/Model/ETC/Bubble"), SmokePoint.position, Quaternion.identity);
        GameObject smoke = ObjectsPool.Instance.GetObject("Bubble");
        smoke.transform.position = SmokePoint.position;
        smoke.transform.localScale = new Vector3(1f, 1f, 1f);
        smoke.GetComponent<SpriteRenderer>().color = Color.white;
        SpriteRenderer renderer = smoke.GetComponent<SpriteRenderer>();
        renderer.sprite = SmokeSprites[Random.Range(0, SmokeSprites.Count)];
        renderer.flipX = Random.value > 0.5f;
        StartCoroutine(CreateSmokeCoroutine(smoke));
    }
    IEnumerator CreateSmokeCoroutine(GameObject smoke)
    {
        // 연기 오브젝트가 x축으로 2~4번 랜덤하게 좌우로 움직이고, y축은 꾸준히 위로 이동
        // 시작시 scale을 0으로 하여 점점 커지는 효과로 "생겨나는" 느낌을 준다
        // 마지막 x축 이동에서는 alpha를 0으로 줄이고, 0이 되면 오브젝트를 파괴
        // 둥실둥실 느낌을 위해 scale도 살짝씩 변화시킴
        int xMoveCount = Random.Range(2, 5); // 2~4회
        float totalTime = 1.5f + 0.3f * xMoveCount; // 전체 시간 (대략)
        float elapsed = 0f;
        float ySpeed = Random.Range(0.7f, 1.2f) * SmokeSpeed; // y축 속도
        float xMoveDuration = totalTime / xMoveCount;
        Vector3 startPos = smoke.transform.position;
        SpriteRenderer sr = smoke.GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        float xDir = Random.value > 0.5f ? 1f : -1f;
        Vector3 originalScale = smoke.transform.localScale;
        float scaleBase = originalScale.x;
        float scaleAmp = Random.Range(0.07f, 0.15f); // scale 변화 폭

        // 시작시 scale을 0으로 설정
        smoke.transform.localScale = new Vector3(0f, 0f, originalScale.z);

        for (int i = 0; i < xMoveCount; i++)
        {
            float t = 0f;
            float xStart = smoke.transform.position.x;
            float xTarget = xStart + Random.Range(0.2f, 0.5f) * xDir;
            xDir *= -1f; // 다음 이동은 반대 방향
            float yStart = smoke.transform.position.y;
            float yTarget = yStart + ySpeed * xMoveDuration;

            // scale 변화 방향도 랜덤하게
            float scaleDir = Random.value > 0.5f ? 1f : -1f;
            float scaleStart;
            float scaleTarget;

            if (i == 0)
            {
                // 첫 구간에서는 0에서 시작해서 원래 scale로 커지게
                scaleStart = 0f;
                scaleTarget = scaleBase + scaleAmp * scaleDir;
            }
            else
            {
                scaleStart = smoke.transform.localScale.x;
                scaleTarget = scaleBase + scaleAmp * scaleDir;
            }

            while (t < xMoveDuration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / xMoveDuration);
                float x = Mathf.Lerp(xStart, xTarget, ratio);
                float y = Mathf.Lerp(yStart, yTarget, ratio);
                smoke.transform.position = new Vector3(x, y, smoke.transform.position.z);

                // 둥실둥실 scale 변화 (sin 곡선과 lerp 조합)
                float scaleSin = Mathf.Sin((elapsed + t) * Mathf.PI * 0.7f) * scaleAmp * 0.5f;
                float scaleLerp = Mathf.Lerp(scaleStart, scaleTarget, ratio);
                float scaleValue = scaleLerp + scaleSin;
                smoke.transform.localScale = new Vector3(scaleValue, scaleValue, originalScale.z);

                // 마지막 x축 이동일 때 alpha 감소
                if (i == xMoveCount - 1)
                {
                    float alpha = Mathf.Lerp(1f, 0f, ratio);
                    sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                }
                yield return null;
            }
            elapsed += xMoveDuration;
        }
        // alpha가 0이 되었으니 오브젝트 파괴
        // Destroy(smoke);
        smoke.GetComponent<ObjectPoolItem>().EndLife();
        yield break;
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

}
