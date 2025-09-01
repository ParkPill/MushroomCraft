using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movable : UnitBase
{
    [Header("Basic")]
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 0.1f;
    public float pathUpdateInterval = 0.2f;
    public float stuckTimeThreshold = 2f;

    [Header("Animation")]
    public Animator animator;
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int AttackHash = Animator.StringToHash("attack");
    protected static readonly int IdleHash = Animator.StringToHash("idle");
    protected static readonly int RunHash = Animator.StringToHash("run");

    [Header("Movable Combat")]
    // public float attackRange = 5f;
    // public float attackCooldown = 1f;
    public float lastAttackTime;
    public bool canAttack = true;

    public Vector3 TargetMovePoint;

    public bool isMoving = false;
    protected bool isAttacking = false;
    protected Vector3 lastPosition;
    protected float stuckTimer;
    protected float lastPathUpdateTime;
    protected AStarTile astarTile;
    protected int currentPathIndex;
    protected bool AttackRequested = false;
    float _targetPositionCheckTimer = 0f;
    void Start()
    {
        Init();
    }
    void Update()
    {
        UpdateAction();
    }
    override public void Init()
    {
        base.Init();

        astarTile = GetComponent<AStarTile>();
        lastPosition = transform.position;

        // Get Animator component if not assigned
        if (animator == null)
        {
            animator = Model.GetComponent<Animator>();
            Model.GetComponent<AnimationHandler>().OnAttack += OnModelAttack;
        }

        // Get model transform if not assigned
        if (Model == null)
        {
            // Try to find a child object named "Model"
            Model = transform.Find("Model");
            if (Model == null)
            {
                // If no "Model" child found, use the first child
                if (transform.childCount > 0)
                {
                    Model = transform.GetChild(0);
                }
                else
                {
                    // If no children, use this transform
                    Model = transform;
                }
            }
        }
    }
    public override void InteractWith(UnitBase clickedUnit)
    {
        base.InteractWith(clickedUnit);
        if (clickedUnit.Team == Team || clickedUnit.Team == Identity.Mutual)
        {
            MoveTo(clickedUnit.transform);
            targetUnit = null;
        }
        else
        {
            AttackUnit(clickedUnit);
        }
    }

    protected void UpdateAction()
    {
        if (targetUnit != null)
        {
            HandleCombat();
        }
        if (AttackRequested)
        {
            AttackRequested = false;
            isMoving = false;
            print("attack requested");
            animator.SetBool(IsMovingHash, false);
            // animator.SetBool(IsAttackingHash, true);
            animator.Play("attack");
            // animator.Play(AttackHash);
        }
        else if (isMoving)
        {
            animator.SetBool(IsMovingHash, true);
            animator.SetBool(IsAttackingHash, false);

            UpdateMovement();

            // animator.Play(RunHash);
        }

        // // Update animation states
        // if (animator != null)
        // {
        //     print("set animation");
        //     animator.SetBool(IsMovingHash, isMoving);
        //     animator.SetBool(IsAttackingHash, isAttacking);
        // }
    }
    void InitMove()
    {
        isMoving = true;
        isAttacking = false;
        // targetUnit = null;
        stuckTimer = 0f;
        lastPosition = transform.position;
        currentPathIndex = 0;
    }
    public virtual void FollowPath()
    {
        InitMove();
    }
    public virtual void MoveTo(Transform targetMovePoint)
    {
        TargetMovePoint = targetMovePoint.position;
        InitMove();
        print("move to target2:" + name);

        // animator.SetBool(IsMovingHash, true);
        // animator.SetBool(IsAttackingHash, false);
        // animator.Play(RunHash);
        UnitBase targetUnit = targetMovePoint.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            MoveToBoundary(targetUnit);
        }
        else
        {
            MoveTo(targetMovePoint.position);
        }
    }
    public virtual void MoveToBoundary(UnitBase building)
    {
        Vector2Int buildingSize = building.GetSize();
        Vector3 buildingPos = building.transform.position;

        List<Vector3> boundaryPoints = new List<Vector3>();
        // 건물 주변의 경계 포인트를 boundaryPoints에 추가
        // buildingSize.x, buildingSize.y는 각각 가로, 세로 크기
        // 각 타일의 중심을 기준으로, 바깥쪽 8방향(혹은 그 이상)을 모두 추가

        // 좌우, 상하, 네 귀퉁이, 그리고 크기에 따라 추가 포인트
        // 1x1: 8개, 2x1: 10개, 2x2: 12개

        float left = buildingPos.x - buildingSize.x / 2f;
        float right = buildingPos.x + buildingSize.x / 2f;
        float bottom = buildingPos.y - buildingSize.y / 2f;
        float top = buildingPos.y + buildingSize.y / 2f;

        // 좌우 라인(상하로 sweep)
        for (int y = 0; y < buildingSize.y; y++)
        {
            float py = bottom + 0.5f + y;
            boundaryPoints.Add(new Vector3(left - 0.5f, py, buildingPos.z));  // 왼쪽
            boundaryPoints.Add(new Vector3(right + 0.5f, py, buildingPos.z)); // 오른쪽
        }

        // 상하 라인(좌우로 sweep)
        for (int x = 0; x < buildingSize.x; x++)
        {
            float px = left + 0.5f + x;
            boundaryPoints.Add(new Vector3(px, bottom - 0.5f, buildingPos.z)); // 아래
            boundaryPoints.Add(new Vector3(px, top + 0.5f, buildingPos.z));    // 위
        }

        // 네 귀퉁이
        // boundaryPoints.Add(new Vector3(left, bottom, buildingPos.z));
        // boundaryPoints.Add(new Vector3(left, top - 0.5f, buildingPos.z));
        // boundaryPoints.Add(new Vector3(right, bottom, buildingPos.z));
        // boundaryPoints.Add(new Vector3(right, top - 0.5f, buildingPos.z));

        // // 빌딩의 경계를 계산
        // float buildingLeft = buildingPos.x - buildingSize.x / 2f;
        // float buildingRight = buildingPos.x + buildingSize.x / 2f;
        // float buildingBottom = buildingPos.y - buildingSize.y / 2f;
        // float buildingTop = buildingPos.y + buildingSize.y / 2f;

        // // 현재 위치에서 가장 가까운 빌딩 외부 지점 찾기
        // Vector3 currentPos = transform.position;
        // Vector3 closestPoint = currentPos;

        // // X축 방향으로 가장 가까운 지점 찾기
        // if (currentPos.x < buildingLeft)
        //     closestPoint.x = buildingLeft - 0.5f;
        // else if (currentPos.x > buildingRight)
        //     closestPoint.x = buildingRight + 0.5f;
        // else
        //     closestPoint.x = (currentPos.x < buildingPos.x) ? buildingLeft - 0.5f : buildingRight + 0.5f;

        // // Y축 방향으로 가장 가까운 지점 찾기
        // if (currentPos.y < buildingBottom)
        //     closestPoint.y = buildingBottom - 0.5f;
        // else if (currentPos.y > buildingTop)
        //     closestPoint.y = buildingTop + 0.5f;
        // else
        //     closestPoint.y = (currentPos.y < buildingPos.y) ? buildingBottom - 0.5f : buildingTop + 0.5f;
        // // print($"closestPoint: {closestPoint.x}, {closestPoint.y}");
        Vector3 closestPoint = boundaryPoints[0];
        float closestDistance = (transform.position - closestPoint).sqrMagnitude;
        foreach (Vector3 point in boundaryPoints)
        {
            float distance = (transform.position - point).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }
        // 가장 가까운 지점으로 이동
        MoveTo(closestPoint);

    }

    public virtual void MoveTo(Vector2 targetMovePos)
    {
        InitMove();

        // Get current grid position
        Vector3 currentPos = transform.position;
        int startX = Mathf.FloorToInt(currentPos.x);
        int startY = Mathf.FloorToInt(currentPos.y);

        // Get target grid position
        int endX = Mathf.FloorToInt(targetMovePos.x);
        int endY = Mathf.FloorToInt(targetMovePos.y);

        if (MoveType == MoveTypes.Walk)
        {
            // Generate path using A*
            astarTile.GeneratePath(startX, startY, endX, endY);
            if (astarTile.path.Count > 0)
            {
                astarTile.path[astarTile.path.Count - 1] = targetMovePos;
            }
            if (astarTile.path.Count > 1)
            {
                astarTile.path.RemoveAt(0);
            }
        }
        else if (MoveType == MoveTypes.Fly)
        {
            astarTile.GenerateFlyPath(startX, startY, endX, endY);
        }
    }

    public virtual void MoveToWithSharedPath(Transform targetMovePoint, List<Vector2> sharedPath)
    {
        TargetMovePoint = targetMovePoint.position;
        isMoving = true;
        isAttacking = false;
        targetUnit = null;
        stuckTimer = 0f;
        lastPosition = transform.position;
        currentPathIndex = 0;

        // Use the shared path directly
        if (sharedPath != null && sharedPath.Count > 0)
        {
            astarTile.SetPath(sharedPath);
            if (astarTile.path.Count > 1)
            {
                currentPathIndex = 1;
            }
        }
        else
        {
            // Fallback to normal path generation if shared path is invalid
            MoveTo(targetMovePoint);
        }
    }

    protected virtual void UpdateMovement()
    {
        if (astarTile.path == null || astarTile.path.Count == 0)
        {
            Stop();
            return;
        }

        // Check if stuck
        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckTimeThreshold)
            {
                Stop();
                return;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }

        // Get current target point from path
        if (currentPathIndex < astarTile.path.Count)
        {
            Vector2 targetPoint = astarTile.path[currentPathIndex];
            // Add 0.5 offset to move to tile center
            Vector3 targetPosition = new Vector3(targetPoint.x, targetPoint.y);
            Vector3 currentPosition = transform.position;

            // Calculate direction to target
            Vector3 direction = (targetPosition - currentPosition).normalized;
            float distanceToTarget = (targetPosition - currentPosition).sqrMagnitude;

            // Flip model based on movement direction
            if (Model != null)
            {
                Vector3 scale = Model.localScale;
                scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? -1 : 1);
                Model.localScale = scale;
            }

            // Calculate how far we'll move this frame
            float moveDistance = moveSpeed * Time.deltaTime;

            // If we would overshoot the target this frame
            if (moveDistance * moveDistance >= distanceToTarget)
            {
                // Move exactly to the target
                currentPathIndex++;

                // Check if reached final destination
                if (currentPathIndex >= astarTile.path.Count)
                {
                    // Check if the final position is occupied
                    bool isWorkingWorker = GetUnitCategory() == UnitCategory.Worker && (this as Worker).currentGoldMine != null;
                    if (!isWorkingWorker && IsPositionOccupied(transform.position))
                    {
                        // Try to find an alternative position
                        Vector3 alternativePos = FindAlternativePosition(transform.position);
                        if (alternativePos != transform.position)
                        {
                            // Add alternative position to the path
                            int altX = Mathf.FloorToInt(alternativePos.x);
                            int altY = Mathf.FloorToInt(alternativePos.y);
                            // astarTile.path.Add(new Vector2(altX, altY));
                            print($"add alternative position to the path: {altX}, {altY}");
                            return; // Continue movement in next frame
                        }
                    }
                    transform.position = targetPosition;
                    Stop();
                    return;
                }

                // Get next target point
                targetPoint = astarTile.path[currentPathIndex];
                targetPosition = new Vector3(targetPoint.x + 0.5f, targetPoint.y + 0.5f);
                direction = (targetPosition - transform.position).normalized;

                // Update model direction for the new path segment
                if (Model != null)
                {
                    Vector3 scale = Model.localScale;
                    scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? -1 : 1);
                    Model.localScale = scale;
                }
            }

            // Move towards target
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        bool isNotWalkable = CCAStarManager.Instance.Map[y][x] == 1;
        if (isNotWalkable)
        {
            return true;
        }
        // Create a small box around the position to check
        Vector2 size = new Vector2(0.8f, 0.8f); // Slightly smaller than tile size to avoid edge cases
        Vector2 center = new Vector2(position.x, position.y);

        // Cast a box to check for other units
        RaycastHit2D[] hits = Physics2D.BoxCastAll(center, size, 0f, Vector2.zero, 0f, LayerMask.GetMask("Unit"));
        // print($"hits: {hits.Length}");
        foreach (RaycastHit2D hit in hits)
        {
            UnitBase unit = hit.collider.GetComponent<UnitBase>();
            // print($"unit: {unit.name}");
            if (unit != null && unit != this) // Skip self
            {
                Movable walkable = unit as Movable;
                if (walkable != null && !walkable.isMoving)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private Vector3 FindAlternativePosition(Vector3 currentPos)
    {
        int currentX = Mathf.FloorToInt(currentPos.x);
        int currentY = Mathf.FloorToInt(currentPos.y);
        int[][] map = CCAStarManager.Instance.Map;

        // Check all 8 surrounding tiles
        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        // First, try to find an empty walkable tile
        // for (int i = 0; i < 8; i++)
        // {
        int i = Random.Range(0, 8);
        int newX = currentX + dx[i];
        int newY = currentY + dy[i];
        Vector3 checkPos = new Vector3(newX + 0.5f, newY + 0.5f, 0);
        bool isOut = newX < 0 || newX >= map[0].Length || newY < 0 || newY >= map.Length;
        if (isOut)
        {
            return FindAlternativePosition(currentPos);
        }

        // if (IsValidPosition(newX, newY) && map[newY][newX] == 0)// && !IsPositionOccupied(checkPos))
        // {
        //     return checkPos;
        // }
        // }

        // If no empty tile found, return current position
        // return currentPos;
        return checkPos;
    }

    private bool IsValidPosition(int x, int y)
    {
        return CCAStarManager.Instance.IsValidPosition(x, y);
    }

    public override void AttackUnit(UnitBase target)
    {
        base.AttackUnit(target);
        print($"attack {target}/{canAttack}");
        if (!canAttack || target == null) return;
        if (!GameManager.IsAttackable(this, target)) return;
        print($"{name} attack unit {target.name}!!");
        // Leave current group if exists
        // if (currentGroup != null)
        // {
        //     currentGroup.RemoveUnit(this);
        //     currentGroup = null;
        // }

        targetUnit = target;
        isAttacking = true;

        MoveTo(target.transform);
        // isMoving = false;
    }
    public override void AttackPosition(Vector3 target)
    {
        base.AttackPosition(target);
        print($"attack position {target}");
        MoveTo(target);
    }

    protected virtual void HandleCombat()
    {
        if (targetUnit == null)
        {
            isAttacking = false;
            return;
        }

        if (targetUnit)
        {
            _targetPositionCheckTimer += Time.deltaTime;
            if (_targetPositionCheckTimer >= 1)
            {
                // _targetPositionCheckTimer = 0f;
                // if (TargetMovePoint != targetUnit.transform.position)
                // {
                //     print($"target position changed. {TargetMovePoint} -> {targetUnit.transform.position}");
                //     MoveTo(targetUnit.transform);
                //     return;
                // }
                isMoving = false;
            }
        }

        if (astarTile.path.Count == 0) return;
        Vector2 lastPoint = astarTile.path[astarTile.path.Count - 1];
        Vector3 targetPos = new Vector3(lastPoint.x, lastPoint.y);
        // Vector3 distance = transform.position - targetPos;
        // float distanceToTarget = Mathf.Abs(distance.x) + Mathf.Abs(distance.y);
        // if (distanceToTarget > 0.5f)
        float distanceSqr = (transform.position - targetPos).sqrMagnitude;
        if (distanceSqr > attackRange * attackRange)
        {

            // Move towards target if out of range
            if (!isMoving)
            {
                print($"move to target. distanceToTarget:{name}/{targetUnit.name}/{distanceSqr}/{attackRange}/{attackRange * attackRange}");
                MoveTo(targetUnit.transform);
            }
            // print("move to target");
            return;
        }
        else
        {
            // print("arrived!");
        }

        // Face the target
        Vector3 direction = (targetUnit.transform.position - transform.position).normalized;
        float scale = Model.localScale.y;
        Model.localScale = new Vector3(direction.x > 0 ? -scale : scale, scale, 1);
        // Quaternion lookRotation = Quaternion.LookRotation(direction);
        // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Attack if cooldown is ready
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Stop();
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual void PerformAttack()
    {
        // print("perform attack: " + name);

        if (animator != null && targetUnit && targetUnit.HP > 0)
        {
            print($"perform attack animation {targetUnit.name}/{attackDamage}/{targetUnit.HP}");
            AttackRequested = true;
        }
        else
        {
            Stop();
        }
    }
    public virtual void OnModelAttack()
    {
        if (targetUnit)
        {
            targetUnit.TakeDamage(attackDamage, this);
        }
        else
        {
            animator.SetBool(IsAttackingHash, false);
        }
    }

    public virtual void Stop()
    {
        isMoving = false;
        isAttacking = false;
        // targetUnit = null;
        print("stop");
        stuckTimer = 0f;
        currentPathIndex = 0;

        // Update animation states
        if (animator != null)
        {
            print($"set stop animation. name: {name}");
            animator.SetBool(IsMovingHash, false);
            animator.SetBool(IsAttackingHash, false);
        }
    }

    public virtual void Hold()
    {
        isMoving = false;
        isAttacking = false;
        targetUnit = null;
        stuckTimer = 0f;
        print("hold");

        // Update animation states
        if (animator != null)
        {
            print("set hold animation");
            animator.SetBool(IsMovingHash, false);
            animator.SetBool(IsAttackingHash, false);
        }
    }

    // Getters
    public bool IsAttacking() => isAttacking;
    public UnitBase GetTargetUnit() => targetUnit;
    public Vector3 GetTargetPosition() => TargetMovePoint;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        print($"OnTriggerEnter2D: {collision.gameObject.name}");

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        print($"OnCollisionEnter2D: {collision.gameObject.name}");

    }
}
