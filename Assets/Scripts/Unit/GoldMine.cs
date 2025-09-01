using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

public class GoldMine : BuildingBase
{
    [Header("Gold Mine Properties")]
    [SerializeField] private int maxWorkers = 3;
    [SerializeField] private float miningTime = 3f;
    [SerializeField] private int initialGold = 5000;
    [SerializeField] private int goldPerMining = 10;

    [Header("UI")]
    [SerializeField] private Text infoText;
    public Sprite SptOn;
    public Sprite SptOff;

    public int CurrentGold;
    private List<Worker> workers = new List<Worker>();
    private Dictionary<Worker, Coroutine> miningCoroutines = new Dictionary<Worker, Coroutine>();

    void Start()
    {
        Init();
        CurrentGold = initialGold;
        UpdateUI();
    }

    void Update()
    {
        // UpdateUI();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if (infoText != null)
        {
            infoText.text = $"Gold: {CurrentGold}\nWorkers: {workers.Count}/{maxWorkers}";
        }
        Model.GetComponent<Animator>().SetBool("IsRunning", workers.Count > 0);
    }
    public bool TryEnter(Worker worker)
    {
        print($"GoldMine {name} try enter: {worker.name}");
        if (workers.Count >= maxWorkers)
        {
            print($"GoldMine {name} is full");
            return false;
        }

        if (!workers.Contains(worker))
        {
            print($"GoldMine {name} try enter: {worker.name}");
            workers.Add(worker);
            UpdateUI();
            // Worker를 보이지 않게 함
            worker.gameObject.SetActive(false);
            worker.CarryingItemCount = CurrentGold > Consts.GoldPerOnce ? Consts.GoldPerOnce : CurrentGold;
            print($"GoldMine {name} worker.CarryingItemCount: {worker.CarryingItemCount}");
            CurrentGold -= worker.CarryingItemCount;
            // 채굴 시작
            Coroutine miningCoroutine = StartCoroutine(MiningCoroutine(worker));
            miningCoroutines[worker] = miningCoroutine;

            return true;
        }

        print($"GoldMine {name} try enter: {worker.name} failed");
        return false;
    }

    private IEnumerator MiningCoroutine(Worker worker)
    {
        yield return null;
        worker.OnDeselect?.Invoke(worker);
        yield return new WaitForSeconds(miningTime);

        // 채굴 완료
        if (workers.Contains(worker))
        {
            workers.Remove(worker);
            miningCoroutines.Remove(worker);

            // 골드 차감
            CurrentGold -= goldPerMining;
            UpdateUI();

            // Worker를 다시 보이게 하고 금을 들고 나오게 함
            print($"GoldMine {name} mining complete: {worker.name}");
            worker.gameObject.SetActive(true);
            worker.SetCarryingItem(WorkerCarryingItem.Gold);

            // Worker를 GoldMine 주변에 배치
            // Vector3 exitPosition = GetExitPosition();
            // worker.transform.position = exitPosition;

            // 골드가 없으면 제거
            if (CurrentGold <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector3 GetExitPosition()
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

    private bool IsPositionOccupied(Vector3 position)
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

    public void RemoveWorker(Worker worker)
    {
        if (workers.Contains(worker))
        {
            workers.Remove(worker);
            UpdateUI();

            // 채굴 코루틴 중지
            if (miningCoroutines.ContainsKey(worker))
            {
                StopCoroutine(miningCoroutines[worker]);
                miningCoroutines.Remove(worker);
            }

            // Worker를 다시 보이게 함
            print($"GoldMine {name} remove worker: {worker.name}");
            worker.gameObject.SetActive(true);
            worker.SetCarryingItem(WorkerCarryingItem.None);
        }
    }

    public int GetCurrentGold()
    {
        return CurrentGold;
    }

    public int GetMaxWorkers()
    {
        return maxWorkers;
    }

    public int GetCurrentWorkerCount()
    {
        return workers.Count;
    }

    public bool HasSpace()
    {
        return workers.Count < maxWorkers;
    }

    void OnDestroy()
    {
        // 모든 Worker를 제거
        foreach (Worker worker in workers.ToArray())
        {
            RemoveWorker(worker);
        }
    }
}
