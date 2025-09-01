using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    private static FlowFieldManager _instance;
    public static FlowFieldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("FlowFieldManager").AddComponent<FlowFieldManager>();
                _instance.name = "FlowFieldManager";
            }
            return _instance;
        }
    }

    [Header("Map Settings")]
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);
    [SerializeField] private float mapCellSize = 1f;

    [Header("FlowField Settings")]
    [SerializeField] public float cellSize = 1f;
    [SerializeField] public float fieldRadius = 100f;
    [SerializeField] public LayerMask obstacleLayer;
    public static float targetUpdateInterval = 1f;

    public List<FlowFieldGroup> activeGroups = new List<FlowFieldGroup>();
    private float _oneSecTimer = 0;
    public int[][] Map;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Update all active groups
        for (int i = activeGroups.Count - 1; i >= 0; i--)
        {
            var group = activeGroups[i];
            group.Update();

            // Remove inactive groups
            if (!group.isActive)
            {
                activeGroups.RemoveAt(i);
                Destroy(group.gameObject);
            }
        }

        if (activeGroups.Count > 0)
        {
            // Draw map state
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    var group = activeGroups[activeGroups.Count - 1];
                    Color color;
                    int state = Map[y][x];
                    if (state == 1)
                    {
                        color = Color.red;
                        Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + 1, y + 1, 0), color);
                    }
                    else if (state == 0)
                    {
                        // Draw flow field direction
                        Vector2 flowDir = group.flowField.GetDirection(new Vector2(x, y));
                        if (flowDir != Vector2.zero)
                        {
                            Vector3 start = new Vector3(x + 0.5f, y + 0.5f, 0);
                            Vector3 end = start + new Vector3(flowDir.x, flowDir.y, 0) * 0.3f;
                            Debug.DrawLine(start, end, Color.cyan);
                        }
                    }
                }
            }
        }

        _oneSecTimer += Time.deltaTime;
        if (_oneSecTimer > 1)
        {
            _oneSecTimer = 0;

        }
    }

    public void SetMap(int width, int height)
    {
        mapSize = new Vector2Int(width, height);
        print("mapSize: " + mapSize);
        Map = new int[height][];

        for (int i = 0; i < height; i++)
        {
            Map[i] = new int[width];
        }
    }

    public FlowFieldGroup CreateNewGroup(Transform target, List<Movable> units)
    {
        if (target == null)
        {
            Debug.LogWarning("Target transform is null!");
            return null;
        }

        // Validate target position is within map bounds
        if (!IsPositionInMap(target.position))
        {
            Debug.LogWarning("Target position is outside map bounds!");
            return null;
        }

        var newGroup = new GameObject().AddComponent<FlowFieldGroup>();
        newGroup.SetTarget(target);
        activeGroups.Add(newGroup);

        foreach (var unit in units)
        {
            newGroup.AddUnit(unit);
        }

        return newGroup;
    }

    public Vector2 GetFlowDirection(Vector2 position, FlowFieldGroup group)
    {
        if (group == null || !group.isActive) return Vector2.zero;
        return group.flowField.GetDirection(position);
    }

    public bool IsPositionInMap(Vector3 position)
    {
        float halfMapWidth = mapSize.x * mapCellSize * 0.5f;
        float halfMapHeight = mapSize.y * mapCellSize * 0.5f;

        return position.x >= -halfMapWidth && position.x <= halfMapWidth &&
               position.y >= -halfMapHeight && position.y <= halfMapHeight;
    }

    public Vector2Int GetMapSize() => mapSize;
    public float GetMapCellSize() => mapCellSize;
}

public class FlowFieldGroup : MonoBehaviour
{
    public List<Movable> units = new List<Movable>();
    public Transform target;
    public FlowField flowField;
    public bool isActive = true;
    private Vector2 lastTargetPosition;
    private float lastUpdateTime;

    public void SetTarget(Transform target)
    {
        this.target = target;
        target.SetParent(transform);
        lastTargetPosition = new Vector2(target.position.x, target.position.y);
        lastUpdateTime = Time.time;
        if (flowField == null)
        {
            flowField = gameObject.AddComponent<FlowField>();
        }
        flowField.SetField(FlowFieldManager.Instance.cellSize,
                            FlowFieldManager.Instance.fieldRadius,
                            FlowFieldManager.Instance.obstacleLayer);
        flowField.GenerateField(new Vector2(target.position.x, target.position.y));
    }

    public void AddUnit(Movable unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
            // unit.SetFlowFieldGroup(this);
        }
    }

    public void RemoveUnit(Movable unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
            // unit.SetFlowFieldGroup(null);
        }

        if (units.Count == 0)
        {
            isActive = false;
        }
    }

    void OnDestroy()
    {
        if (target != null)
        {
            Destroy(target.gameObject);
        }
    }

    public void Update()
    {
        if (!isActive || target == null) return;

        // Check if target has moved
        if (Time.time >= lastUpdateTime + FlowFieldManager.targetUpdateInterval)
        {
            Vector2 currentPos = new Vector2(target.position.x, target.position.y);
            if (Vector2.Distance(currentPos, lastTargetPosition) > 0.1f)
            {
                // Target has moved, update flow field
                lastTargetPosition = currentPos;
                flowField.GenerateField(currentPos);
            }
            lastUpdateTime = Time.time;
        }

        // Check if all units have reached their destination or are stuck
        bool allUnitsInactive = true;
        foreach (var unit in units)
        {
            // if (!unit.IsValid(this))
            // {
            //     allUnitsInactive = false;
            //     break;
            // }
        }
    }

    public Vector2 GetTargetPosition()
    {
        return target != null ? new Vector2(target.position.x, target.position.y) : lastTargetPosition;
    }
}