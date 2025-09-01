using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Movable
{
    public WorkerCarryingItem CarryingItem;
    // public WorkerJobs WorkerJob;
    public int CarryingItemCount;

    [Header("Worker Properties")]
    // [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private SpriteRenderer modelRenderer;

    public GoldMine currentGoldMine;
    public Castle currentCastle;
    public Tree currentTree;
    public List<UnitTypes> BuildingList = new List<UnitTypes>();
    public List<UnitTypes> HighTechBuildingList = new List<UnitTypes>();
    // Start is called before the first frame update
    void Start()
    {
        Init();
        if (modelRenderer == null)
        {
            modelRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        UpdateAction();
        // GoldMine으로 이동 중이고 도착했는지 확인
        if (!isAttacking)
        {

            if (currentGoldMine != null)
            {
                if (CarryingItem != WorkerCarryingItem.Gold)
                {
                    // print($"currentGoldMine try: {currentGoldMine.name}");
                    // float distance = Vector3.Distance(transform.position, currentGoldMine.transform.position);
                    // if (distance <= interactionRange)
                    BoxCollider2D mineCollider = currentGoldMine.GetComponent<BoxCollider2D>();
                    if (mineCollider != null && mineCollider.OverlapPoint(transform.position))
                    {
                        print($"currentGoldMine try2: {currentGoldMine.name}");
                        // GoldMine에 들어가기 시도
                        if (currentGoldMine.TryEnter(this))
                        {
                            Stop(); // 이동 중지
                        }
                    }
                }
                else if (CarryingItem == WorkerCarryingItem.Gold)
                {
                    if (currentCastle != null)
                    {
                        BoxCollider2D collider = currentCastle.GetComponent<BoxCollider2D>();
                        BoxCollider2D box = GetComponent<BoxCollider2D>();

                        if (collider != null && collider.bounds.Intersects(box.bounds) && currentCastle.TryEnter(this))
                        {
                            print($"Worker {name} try send to castle: {currentCastle.name}");
                            GameManager.Instance.TheGameScript.AddGold(CarryingItemCount);
                            CarryingItem = WorkerCarryingItem.None;
                            CarryingItemCount = 0;
                            UpdateAnimation();
                            if (currentGoldMine)
                            {
                                TryEnterGoldMine(currentGoldMine);
                            }
                            else
                            {
                                currentGoldMine = GameManager.Instance.TheGameScript.FindGoldMine(this);
                            }
                        }
                    }
                }
            }

            if (currentTree != null)
            {
                if (CarryingItem != WorkerCarryingItem.Lumber)
                {
                    float dt = Time.deltaTime;
                    BoxCollider2D collider = currentTree.GetComponent<BoxCollider2D>();
                    BoxCollider2D box = GetComponent<BoxCollider2D>();
                    if (collider != null && collider.bounds.Intersects(box.bounds))
                    {
                        // print($"currentTree try2: {currentTree.name}/{dt}");
                        // Tree에서 나무 채굴
                        int lumber = currentTree.DigLumber(dt);
                        if (lumber > 0)
                        {
                            print($"currentTree dig lumber: {lumber}");
                            CarryingItemCount = lumber;
                            // currentTree = null;
                            SetCarryingItem(WorkerCarryingItem.Lumber);
                            return;
                        }
                        if (isMoving) Stop(); // 이동 중지
                    }
                }
                else if (CarryingItem == WorkerCarryingItem.Lumber)
                {
                    if (currentCastle != null)
                    {
                        BoxCollider2D collider = currentCastle.GetComponent<BoxCollider2D>();
                        BoxCollider2D box = GetComponent<BoxCollider2D>();

                        if (collider != null && collider.bounds.Intersects(box.bounds) && currentCastle.TryEnter(this))
                        {
                            print($"Worker {name} try send to castle: {currentCastle.name}");
                            GameManager.Instance.TheGameScript.AddLumber(CarryingItemCount);
                            CarryingItem = WorkerCarryingItem.None;
                            CarryingItemCount = 0;
                            UpdateAnimation();
                            if (currentTree)
                            {
                                TryGatherTree(currentTree);
                            }
                            else
                            {
                                currentTree = GameManager.Instance.TheGameScript.FindTree(this);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void InteractWith(UnitBase clickedUnit)
    {
        BuildingBase building = clickedUnit as BuildingBase;
        if (building != null)
        {
            // GoldMine인지 확인
            GoldMine goldMine = building as GoldMine;
            if (goldMine != null)
            {
                TryEnterGoldMine(goldMine);
            }
            else if (building is Castle && CarryingItem == WorkerCarryingItem.Gold)
            {
                TrySendToCastle();
            }
            else if (building is Tree)
            {
                TryGatherTree(building as Tree);
            }
            else if (building is Castle && CarryingItem == WorkerCarryingItem.Lumber)
            {
                TrySendToCastle();
            }
        }
        else
        {
            base.InteractWith(clickedUnit);
        }
    }

    public void TryEnterGoldMine(GoldMine goldMine)
    {
        if (goldMine == null) return;
        currentTree = null;
        // 거리 확인
        BoxCollider2D mineCollider = goldMine.GetComponent<BoxCollider2D>();
        if (mineCollider != null && !mineCollider.OverlapPoint(transform.position))
        {
            // 거리가 멀면 이동
            MoveTo(goldMine.transform);
            currentGoldMine = goldMine;
            return;
        }

        // GoldMine에 들어가기 시도
        if (goldMine.TryEnter(this))
        {
            currentGoldMine = goldMine;
            Stop(); // 이동 중지
        }
    }
    public void TryGatherTree(Tree tree)
    {
        if (tree == null) return;
        currentGoldMine = null;
        BoxCollider2D treeCollider = tree.GetComponent<BoxCollider2D>();
        if (treeCollider != null && !treeCollider.OverlapPoint(transform.position))
        {
            MoveTo(tree.transform);
            currentTree = tree;
            return;
        }
        else
        {
            currentTree = tree;
            Stop();
        }
    }
    public void TrySendToCastle()
    {
        if (!currentCastle)
        {
            currentCastle = GameManager.Instance.TheGameScript.FindCastle(this);
            if (!currentCastle) return;
        }

        // 거리 확인
        BoxCollider2D collider = currentCastle.GetComponent<BoxCollider2D>();
        if (collider != null && !collider.OverlapPoint(transform.position))
        {
            // 거리가 멀면 이동
            MoveTo(currentCastle.transform);
            return;
        }

        // if (currentCastle.TryEnter(this))
        // {
        //     if (CarryingItem == WorkerCarryingItem.Gold) GameManager.Instance.TheGameScript.AddGold(CarryingItemCount);
        //     else if (CarryingItem == WorkerCarryingItem.Lumber) GameManager.Instance.TheGameScript.AddLumber(CarryingItemCount);
        // }
    }

    public void SetVisible(bool visible)
    {
        if (modelRenderer != null)
        {
            modelRenderer.enabled = visible;
        }
    }

    public void SetCarryingItem(WorkerCarryingItem item)
    {
        CarryingItem = item;
        UpdateAnimation();
        TrySendToCastle();
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // CarryingItem에 따라 애니메이션 변경
        // 기존 애니메이터 파라미터를 사용하여 간단하게 처리
        // string animatorPath = "Animation/Worker/Worker";
        switch (CarryingItem)
        {
            case WorkerCarryingItem.Gold:
                // animatorPath = "Animation/Worker/Gold/WorkerGold";
                break;
            case WorkerCarryingItem.Lumber:
                // animatorPath = "Animation/Worker/Lumber/WorkerLumber";
                break;
            case WorkerCarryingItem.None:
                break;
        }
        // animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(animatorPath);
    }

    public override void Stop()
    {
        base.Stop();

        // GoldMine에서 나오기
        // if (currentGoldMine != null)
        // {
        //     currentGoldMine.RemoveWorker(this);
        //     currentGoldMine = null;
        // }
    }

    void OnDestroy()
    {
        // GoldMine에서 나오기
        if (currentGoldMine != null)
        {
            currentGoldMine.RemoveWorker(this);
        }

    }
}
