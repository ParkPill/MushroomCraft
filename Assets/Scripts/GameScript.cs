using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class GameScript : MonoBehaviour
{
    public bool IsPlaceMode = false;
    public Transform TileTemplate;
    public Vector2Int CurrentBuildingTileSize;
    public Material GrayMaterial;
    public ObjectsPool EffectPool;
    public Transform Trees;
    public int Gold = 0;
    public int Lumber = 0;
    public int Oil = 0;
    public Identity MyTeam;
    public UIScript TheUIScript;
    public OrderTypes CurrentOrder = OrderTypes.None;

    public Camera TheCamera;
    public List<UnitBase> AlliList = new List<UnitBase>();
    public List<UnitBase> AllUnitList = new List<UnitBase>();
    public List<UnitBase> SelectedList = new List<UnitBase>();
    private List<Vector2> path = new List<Vector2>();

    // Camera movement variables
    public float cameraSpeed = 20f;
    public float edgeScrollThreshold = 0.05f;
    public Vector3 MapBoundsMin;
    public Vector3 MapBoundsMax;
    public float minCameraSize = 5f;
    public float maxCameraSize = 20f;
    public float zoomSpeed = 2f;

    // Selection box variables
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private LineRenderer selectionBox;
    private Material selectionBoxMaterial;

    // Input handling variables
    private bool isCtrlPressed = false;
    private bool isShiftPressed = false;

    public int width = 100;
    public int height = 100;

    public Transform TestObject1;
    public Transform TestObject2;
    public Tilemap BlockTileMap;
    float _oneSecTimer = 0;
    List<Movable> _walkableSelectedList = new List<Movable>();
    float lastClickTime = 0;

    void Awake()
    {
        TheUIScript = GetComponent<UIScript>();
        GameManager.Instance.TheGameScript = this;
    }
    void Start()
    {
        // Initialize camera bounds based on NavMeshSurface
        // Bounds surfaceBounds = TheSurface.navMeshData.sourceBounds;
        // cameraBoundsMin = surfaceBounds.min;
        // cameraBoundsMax = surfaceBounds.max;

        // print("MapBoundsMax: " + MapBoundsMax);
        // print("MapBoundsMin: " + MapBoundsMin);

        AllUnitList.AddRange(FindObjectsByType<UnitBase>(FindObjectsSortMode.None));

        // Create selection box
        CreateSelectionBox();

        SetBlockTileMap();


        foreach (UnitBase obj in AllUnitList)
        {
            BuildingBase building = obj.GetComponent<BuildingBase>();
            if (building != null)
            {
                // print($"building: {building.name}/{building.transform.position}");
                AddBuilding(building.gameObject, (int)building.transform.position.x - (int)building.GetSize().x / 2, (int)building.transform.position.y - (int)building.GetSize().y / 2);
                building.transform.localPosition = new Vector3(
                    (int)building.transform.position.x + ((int)building.GetSize().x % 2 == 0 ? 0 : 0.5f),
                    (int)building.transform.position.y + ((int)building.GetSize().y % 2 == 0 ? 0 : 0.5f),
                    building.transform.localPosition.z
                );
            }
            obj.OnDie += OnUnitDie;
            obj.OnDeselect += OnUnitDeselectRequired;
        }
    }
    void OnUnitDeselectRequired(UnitBase obj)
    {
        if (SelectedList.Contains(obj))
        {
            SelectedList.Remove(obj);
            if (obj) obj.SetSelected(false);
        }
    }
    void OnUnitDie(UnitBase obj)
    {
        AllUnitList.Remove(obj);
        if (obj.Team == MyTeam)
        {
            AlliList.Remove(obj);
        }
        obj.OnDie -= OnUnitDie;
        obj.OnDeselect -= OnUnitDeselectRequired;
    }
    void SetBlockTileMap()
    {
        BlockTileMap.CompressBounds();
        BoundsInt bounds = BlockTileMap.cellBounds;
        MapBoundsMin = BlockTileMap.CellToWorld(bounds.min);

        width = bounds.size.x;
        height = bounds.size.y;

        MapBoundsMax = new Vector3(width, height);
        MapBoundsMin = new Vector3(0, -2);

        print($"타일맵 크기 - width: {width}, height: {height}");

        // Initialize map
        CCAStarManager.Instance.SetMap(width, height);

        // Set blocked tiles
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = BlockTileMap.GetTile(cellPos);

                if (tile != null)
                {
                    // print($"tile: {tile.name}");
                    if (tile.name.EndsWith("tree"))
                    {
                        GameObject tree = Instantiate(Resources.Load<GameObject>("Prefab/Building/Tree"), Trees);
                        // print("tree created");
                        AddBuilding(tree, x, y);
                    }
                    else
                    {
                        CCAStarManager.Instance.SetState(x, y, 1); // 1 = Block
                    }
                }
            }
        }

        BlockTileMap.gameObject.SetActive(false);
    }
    void AddBuilding(GameObject building, int x, int y)
    {
        BuildingBase buildingBase = building.GetComponent<BuildingBase>();
        Vector2Int size = buildingBase.GetSize();
        building.transform.position = new Vector3(x, y, 0) + new Vector3(size.x / 2f, size.y / 2f, 0);


        // Set building area as blocked
        for (int blockY = y; blockY < y + size.y; blockY++)
        {
            for (int blockX = x; blockX < x + size.x; blockX++)
            {
                // print($"building: {building.name} blockX: {blockX}, blockY: {blockY}");
                CCAStarManager.Instance.SetState(blockX, blockY, 1); // 1 = Block
            }
        }
    }

    void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
        HandleLeftClick();
        HandleInputModifiers();
        UpdateSelectedObjectsBoxes();
        HandleRightClick();
        HandleMouseMove();
    }

    void OnDrawGizmos()
    {
        // if (Application.isPlaying) DrawOccupiedTile();
    }

    void DrawOccupiedTile()
    {
        for (int y = 0; y < CCAStarManager.Instance.mapHeight; y++)
        {
            for (int x = 0; x < CCAStarManager.Instance.mapWidth; x++)
            {
                if (CCAStarManager.Instance.GetState(x, y) == 1)
                {
                    Vector3 center = new Vector3(x + 0.5f, y + 0.5f, 0);
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(center, Vector3.one);
                }
            }
        }
    }

    private void HandleCameraMovement()
    {
        if (Application.isEditor) return;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // Check screen edges for camera movement
        if (mousePosition.x <= Screen.width * edgeScrollThreshold)
            moveDirection.x = -1;
        else if (mousePosition.x >= Screen.width * (1 - edgeScrollThreshold))
            moveDirection.x = 1;

        if (mousePosition.y <= Screen.height * edgeScrollThreshold)
            moveDirection.y = -1;
        else if (mousePosition.y >= Screen.height * (1 - edgeScrollThreshold))
            moveDirection.y = 1;

        // Move camera
        Vector3 newPosition = TheCamera.transform.position + moveDirection * cameraSpeed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, MapBoundsMin.x, MapBoundsMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, MapBoundsMin.y, MapBoundsMax.y);
        TheCamera.transform.position = newPosition;
    }
    void LateUpdate()
    {
        // Draw map state
        // for (int y = 0; y < height; y++)
        // {
        //     for (int x = 0; x < width; x++)
        //     {
        //         Color color;
        //         MapState state = CCAstarManager.Instance.GetState(x, y);
        //         if (state == MapState.Block)
        //         {
        //             color = Color.red;
        //             Debug.DrawLine(new Vector3(x, y + 1, 0), new Vector3(x + 1, y, 0), color);
        //         }
        //         else if (state == MapState.Walkable)
        //         {
        //             // Draw flow field direction
        //             Vector2 flowDir = CCAstarManager.Instance.GetFlowDirection(x, y);
        //             if (flowDir != Vector2.zero)
        //             {
        //                 Vector3 start = new Vector3(x + 0.5f, y + 0.5f, 0);
        //                 Vector3 end = start + new Vector3(flowDir.x, flowDir.y, 0) * 0.3f;
        //                 Debug.DrawLine(start, end, Color.cyan);
        //             }
        //         }
        //         else if (state == MapState.Swimable)
        //         {
        //             color = Color.blue;
        //             Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + 1, y + 1, 0), color);
        //         }
        //     }
        // }

        // Draw path
        if (path != null && path.Count >= 2)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = new Vector3(path[i].x, path[i].y, 0);
                Vector3 end = new Vector3(path[i + 1].x, path[i + 1].y, 0);
                Debug.DrawLine(start, end, Color.green, Time.deltaTime);
            }
        }

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.y += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.y -= 1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x -= 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x += 1f;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            OnCommand(Commands.Attack);
            TheUIScript.SetKeyDown(Commands.Attack);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            OnCommand(Commands.Move);
            TheUIScript.SetKeyDown(Commands.Move);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            OnCommand(Commands.Patrol);
            TheUIScript.SetKeyDown(Commands.Patrol);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            OnCommand(Commands.Hold);
            TheUIScript.SetKeyDown(Commands.Hold);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            OnCommand(Commands.Stop);
            TheUIScript.SetKeyDown(Commands.Stop);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TheUIScript.IsBuildSelected || TheUIScript.IsHighTechBuildSelected)
            {
                TheUIScript.OnCancelMenuClick();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) OnCommand(Commands.Num0);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) OnCommand(Commands.Num1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) OnCommand(Commands.Num2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) OnCommand(Commands.Num3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) OnCommand(Commands.Num4);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) OnCommand(Commands.Num5);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) OnCommand(Commands.Num6);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) OnCommand(Commands.Num7);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) OnCommand(Commands.Num8);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) OnCommand(Commands.Num9);

        if (Input.GetKeyUp(KeyCode.A) ||
        Input.GetKeyUp(KeyCode.M) ||
        Input.GetKeyUp(KeyCode.P) ||
        Input.GetKeyUp(KeyCode.H) ||
        Input.GetKeyUp(KeyCode.S)) TheUIScript.ResetKeyDown();

        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            Vector3 newPosition = TheCamera.transform.position + moveDirection * cameraSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, MapBoundsMin.x + TheCamera.orthographicSize * TheCamera.aspect, MapBoundsMax.x - TheCamera.orthographicSize * TheCamera.aspect);
            newPosition.y = Mathf.Clamp(newPosition.y, MapBoundsMin.y + TheCamera.orthographicSize, MapBoundsMax.y - TheCamera.orthographicSize);
            TheCamera.transform.position = newPosition;
            TheUIScript.UpdateMiniMapRectPosition();
        }
    }
    public void OnCommand(Commands command)
    {
        switch (command)
        {
            case Commands.Move:
                OnMoveClick();
                break;
            case Commands.Attack:
                OnAttackClick();
                break;
            case Commands.Patrol:
                OnPatrolClick();
                break;
            case Commands.Hold:
                OnHoldClick();
                break;
            case Commands.Stop:
                OnStopClick();
                break;
            case Commands.Num0:
            case Commands.Num1:
            case Commands.Num2:
            case Commands.Num3:
            case Commands.Num4:
            case Commands.Num5:
            case Commands.Num6:
            case Commands.Num7:
            case Commands.Num8:
            case Commands.Num9:
                SelectShortCut((int)command - (int)Commands.Num0);
                break;
        }
    }
    public void SelectShortCut(int index)
    {

    }
    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newSize = TheCamera.orthographicSize - scroll * zoomSpeed;
            float maxSize = Mathf.Min(Mathf.Min((MapBoundsMax.x - MapBoundsMin.x) / TheCamera.aspect / 2, (MapBoundsMax.y - MapBoundsMin.y) / 2), maxCameraSize);
            newSize = Mathf.Clamp(newSize, minCameraSize, maxSize);
            TheCamera.orthographicSize = newSize;
            TheUIScript.UpdateMiniMapRectSize();

            Vector3 newPosition = TheCamera.transform.position;
            // newPosition.x = Mathf.Clamp(newPosition.x, MapBoundsMin.x + TheCamera.orthographicSize * TheCamera.aspect, MapBoundsMax.x - TheCamera.orthographicSize * TheCamera.aspect);
            // newPosition.y = Mathf.Clamp(newPosition.y, MapBoundsMin.y + TheCamera.orthographicSize, MapBoundsMax.y - TheCamera.orthographicSize);
            bool isModified = TheUIScript.MoveCameraToPosition(newPosition);
            if (isModified)
            {
                TheUIScript.UpdateMiniMapRectPosition();
            }
        }
    }
    public void TurnOnTowerPlaceMode(int index)
    {
        IsPlaceMode = true;

        // TurnOnPlaceMode();

        // TileTemplate.transform.position = TheCamera.transform.position + new Vector3((CurrentTowerData.TileSize.x / 2 + 1) * (TheCamera.transform.localScale.x < 0 ? -1 : 1), 0, 0);
        // CheckPlaceTower(TileTemplate.OccupiedTileSize);
        // TheUIScript.TheCanvas.Find("btnCancel").gameObject.SetActive(true);
        // TheUIScript.TheCanvas.Find("btnOk").gameObject.SetActive(true);
        // TileTemplate.GetComponent<FieldTower>().enabled = false;
    }
    void CheckPlaceTower(Vector2Int tileSize)
    {
        // print($"towerPos1: {TileTemplate.transform.position}");
        Vector2Int towerPos = new Vector2Int(Mathf.RoundToInt(TileTemplate.transform.position.x), Mathf.RoundToInt(TileTemplate.transform.position.y));
        // print($"towerPos2: {towerPos}");
        int mazeWidth = 0;
        int mazeHeight = 0;

        // Map map = null;
        // if (GameType == GameTypes.Maze) map = TheMazeMap;
        // else if (GameType == GameTypes.Fine || GameType == GameTypes.Field) map = TheFineMap;
        // bool isPlaceable = true;
        // for (int x = 0; x < tileSize.x; x++)
        // {
        //     for (int y = 0; y < tileSize.y; y++)
        //     {
        //         Vector2 checkPos = new Vector2(towerPos.x + x, towerPos.y + y);

        //         // 타일 체크
        //         TileBase tile = TheFloor.GetTile(new Vector3Int((int)checkPos.x, (int)checkPos.y, 0));
        //         if (tile == null)
        //         {
        //             isPlaceable = false;
        //             break;
        //         }

        //         // 범위 체크
        //         if (checkPos.x < 0 || checkPos.y < 0)//|| checkPos.x >= mazeWidth || checkPos.y >= mazeHeight)
        //         {
        //             isPlaceable = false;
        //             break;
        //         }
        //         // print($"checkPos: {checkPos}/{[(int)checkPos.x, (int)checkPos.y]}");
        //         // 해당 위치가 벽인지 확인 (maze에서 true는 벽, false는 빈 공간)
        //         if (map.IsWallAt(checkPos))
        //         {
        //             isPlaceable = false;
        //             break;
        //         }
        //     }
        //     if (!isPlaceable) break;
        // }

        // float roadWidth = FieldManager.roadWidth;
        // float smallRoadRoadWidth = FieldManager.smallRoadRoadWidth;
        // float smallBoxWidth = FieldManager.smallBoxWidth;
        // float boxWidth = smallBoxWidth * 2 + smallRoadRoadWidth;
        // Vector3 fieldPos = GetFieldPosition(_fusionClient.SlotIndex);
        // if (fieldPos.x + boxWidth < towerPos.x || fieldPos.y + boxWidth < towerPos.y ||
        // fieldPos.x > towerPos.x || fieldPos.y > towerPos.y)
        // {
        //     isPlaceable = false;
        // }

        // foreach (Transform square in TileTemplate.transform.Find("Squares"))
        // {
        //     SpriteRenderer spriteRenderer = square.GetComponent<SpriteRenderer>();
        //     Color color = isPlaceable ? Color.green : Color.red;
        //     spriteRenderer.color = new Color(color.r, color.g, color.b, spriteRenderer.color.a);
        // }
        // IsPlaceReady = isPlaceable;
    }
    bool _isLeftClickDownWorked = false;
    private void HandleLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isLeftClickDownWorked = false;
            // print($"HandleLeftClick: {_isLeftClickDownWorked}");
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -TheCamera.transform.position.z;
            dragStartPosition = TheCamera.ScreenToWorldPoint(mousePosition);

            // Handle AttackPosition order
            if (CurrentOrder == OrderTypes.Attack)
            {
                UnitBase clickedUnit = GetClickedUnit(dragStartPosition);
                if (clickedUnit != null)
                {
                    foreach (UnitBase obj in SelectedList)
                    {
                        Movable Walkable = obj.GetComponent<Movable>();
                        if (Walkable != null)
                        {
                            Walkable.AttackUnit(clickedUnit);
                            print($"HandleLeftClick: {_isLeftClickDownWorked}");
                        }
                    }
                    if (_isLeftClickDownWorked)
                    {
                        clickedUnit.ShowTargetted();
                    }
                }
                else
                {
                    // Create target object
                    GameObject targetObj = new GameObject("AttackPositionTarget");
                    targetObj.transform.position = dragStartPosition;

                    // Issue attack position command to selected units
                    foreach (UnitBase obj in SelectedList)
                    {
                        Movable Walkable = obj.GetComponent<Movable>();
                        if (Walkable != null)
                        {
                            Walkable.AttackPosition(dragStartPosition);
                        }
                    }
                    ShowAttackTarget(dragStartPosition);

                    // Start checking for arrival
                    StartCoroutine(CheckArrival(targetObj));
                }
                _isLeftClickDownWorked = true;

                // Reset order type
                CurrentOrder = OrderTypes.None;
                return;
            }
            else if (CurrentOrder == OrderTypes.Move)
            {
                selectionBox.gameObject.SetActive(true);

                UnitBase clickedUnit = GetClickedUnit(dragStartPosition);
                if (clickedUnit != null)
                {
                    foreach (UnitBase obj in SelectedList)
                    {
                        Movable walkable = obj.GetComponent<Movable>();
                        if (walkable != null)
                        {
                            walkable.MoveTo(clickedUnit.transform);
                            walkable.targetUnit = null;
                        }
                    }
                    clickedUnit.ShowTargetted();
                }
                else
                {
                    // Create target object
                    GameObject targetObj = new GameObject("MovePositionTarget");
                    targetObj.transform.position = dragStartPosition;

                    // Issue attack position command to selected units
                    foreach (UnitBase obj in SelectedList)
                    {
                        Movable walkable = obj.GetComponent<Movable>();
                        if (walkable != null)
                        {
                            walkable.MoveTo(dragStartPosition);
                            walkable.targetUnit = null;
                        }
                    }

                    // Start checking for arrival
                    StartCoroutine(CheckArrival(targetObj));
                }
                ShowMoveTarget(dragStartPosition);

                // Reset order type
                CurrentOrder = OrderTypes.None;
                return;
            }

            // UI 위에 있다면 드래그 시작하지 않음
            if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            isDragging = true;
            selectionBox.gameObject.SetActive(true);
        }

        if (isDragging)
        {
            UpdateSelectionBox();
        }

        if (Input.GetMouseButtonUp(0))
        {
            // print($"HandleLeftClickUp: {_isLeftClickDownWorked}");
            isDragging = false;
            selectionBox.gameObject.SetActive(false);

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -TheCamera.transform.position.z;
            Vector3 endPosition = TheCamera.ScreenToWorldPoint(mousePosition);
            bool isClickedOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            if (!_isLeftClickDownWorked && !isClickedOnUI)
            {
                // Check if it was a single click (no drag)
                if (Vector3.Distance(dragStartPosition, endPosition) < 0.1f)
                {
                    float doubleClickTime = 0.25f;
                    if (lastClickTime > 0 && (Time.time - lastClickTime) < doubleClickTime)
                    {
                        HandleSingleClickSelection(true);
                    }
                    else
                    {
                        HandleSingleClickSelection();
                    }
                    lastClickTime = Time.time;

                }
                else
                {
                    // Box selection
                    HandleBoxSelection();
                }
            }
        }
    }
    private void HandleMouseMove()
    {
        if (IsPlaceMode)
        {
            // HandlePlaceMode();
        }
    }
    public void ShowMoveTarget(Vector3 worldPosition)
    {
        GameObject obj = GameManager.InstantiatePrefab("Action/MoveTarget");
        obj.transform.position = worldPosition;
        obj.name = "targettedCircle";
        Destroy(obj, 1f);
    }

    public void ShowAttackTarget(Vector3 worldPosition)
    {
        GameObject obj = GameManager.InstantiatePrefab("Action/AttackPosition");
        obj.transform.position = worldPosition;
        obj.name = "targettedCircle";
        Destroy(obj, 1f);
    }

    private void HandleSingleClickSelection(bool isDoubleClick = false)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -TheCamera.transform.position.z;
        Vector3 worldPosition = TheCamera.ScreenToWorldPoint(mousePosition);
        if (!isShiftPressed)
        {
            ClearSelection();
        }
        // foreach (UnitBase obj in AllUnitList)
        // {

        //     Vector2Int size = obj.GetSize();
        //     Vector3 position = obj.transform.position;

        //     float halfWidth = size.x * 0.5f;
        //     float halfHeight = size.y * 0.5f;

        //     float minX = position.x - halfWidth;
        //     float maxX = position.x + halfWidth;
        //     float minY = position.y - halfHeight;
        //     float maxY = position.y + halfHeight;

        //     if (worldPosition.x >= minX && worldPosition.x <= maxX &&
        //         worldPosition.y >= minY && worldPosition.y <= maxY)
        //     {


        //         if (obj.Team == MyTeam || !isCtrlPressed)
        //         {
        //             SelectObject(obj);
        //         }
        //         break;
        //     }
        // }

        // Collider2D collider = Physics2D.OverlapCircle(worldPosition, 0.1f);
        // UnitBase unit = null;
        // if (collider != null)
        // {
        //     unit = collider.gameObject.GetComponent<UnitBase>();
        //     if (unit != null)
        //     {
        //         SelectObject(unit);
        //     }
        // }
        UnitBase unit = GetClickedUnit(worldPosition);
        if (isDoubleClick && unit != null && unit.Team == MyTeam)
        {
            foreach (UnitBase obj in AllUnitList)
            {
                if (obj.Team == MyTeam && obj.UnitType == unit.UnitType)
                {
                    SelectObject(obj);
                }
            }
        }
        if (unit != null)
        {
            SelectObject(unit);
        }
        TheUIScript.UpdateMenuBox(SelectedList);
    }

    private void HandleBoxSelection()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -TheCamera.transform.position.z;
        Vector3 endPosition = TheCamera.ScreenToWorldPoint(mousePosition);

        // Convert world positions to screen positions for selection rect
        Vector3 startScreenPos = TheCamera.WorldToScreenPoint(dragStartPosition);
        Vector3 endScreenPos = TheCamera.WorldToScreenPoint(endPosition);

        Rect selectionRect = new Rect(
            Mathf.Min(startScreenPos.x, endScreenPos.x),
            Mathf.Min(startScreenPos.y, endScreenPos.y),
            Mathf.Abs(endScreenPos.x - startScreenPos.x),
            Mathf.Abs(endScreenPos.y - startScreenPos.y)
        );

        ClearSelection();

        foreach (UnitBase unit in AllUnitList)
        {
            if (unit.Team != MyTeam) continue;

            Vector3 screenPos = TheCamera.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(screenPos))
            {
                SelectObject(unit);
            }
        }
        TheUIScript.UpdateMenuBox(SelectedList);
    }

    private void HandleInputModifiers()
    {
        isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void CreateSelectionBox()
    {
        GameObject selectionBoxObj = new GameObject("SelectionBox");
        selectionBox = selectionBoxObj.AddComponent<LineRenderer>();
        selectionBox.positionCount = 5;
        selectionBox.loop = false;
        selectionBox.startWidth = 0.1f;
        selectionBox.endWidth = 0.1f;
        selectionBox.material = new Material(Shader.Find("Sprites/Default"));
        selectionBox.startColor = Color.green;
        selectionBox.endColor = Color.green;
        selectionBox.sortingOrder = 4;
        selectionBoxObj.SetActive(false);
    }

    private void UpdateSelectionBox()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -TheCamera.transform.position.z;
        Vector3 currentPosition = TheCamera.ScreenToWorldPoint(mousePosition);

        Vector3[] corners = new Vector3[5];
        corners[0] = dragStartPosition;
        corners[1] = new Vector3(currentPosition.x, dragStartPosition.y, 0);
        corners[2] = currentPosition;
        corners[3] = new Vector3(dragStartPosition.x, currentPosition.y, 0);
        corners[4] = dragStartPosition;

        selectionBox.SetPositions(corners);
    }

    private void ClearSelection()
    {
        print($"ClearSelection: {SelectedList.Count}");
        foreach (UnitBase obj in SelectedList)
        {
            obj.SetSelected(false);
        }
        SelectedList.Clear();
    }

    private void SelectObject(UnitBase obj)
    {
        print($"SelectObject: {obj.name}");
        obj.SetSelected(true);
        SelectedList.Add(obj);
    }

    private void UpdateSelectedObjectsBoxes()
    {
        foreach (UnitBase obj in AllUnitList)
        {
            if (!obj) continue;
            Vector2Int size = obj.GetSize(); // NavMeshAgent의 기본 크기 사용
            if (obj.SelectedCircle == null)
            {
                obj.SelectedCircle = GameManager.InstantiatePrefab("SelectedCircle", obj.transform).transform;
                obj.SelectedCircle.localScale = new Vector3(size.x * 0.5f, size.x * 0.25f, 1);
                obj.SelectedCircle.name = "circle";
                if (obj as BuildingBase != null)
                {
                    obj.SelectedCircle.localPosition = new Vector3(0, -0.4f, 0);
                }
            }

            if (obj.IsSelected)
            {
                obj.SelectedCircle.gameObject.SetActive(true);
                // Set color based on team
                SpriteRenderer spriteRenderer = obj.SelectedCircle.GetComponent<SpriteRenderer>();
                if (obj.Team == MyTeam)
                {
                    spriteRenderer.color = GameManager.HexToColor("85F822");
                }
                else if (obj.Team == Identity.Mutual)
                {
                    spriteRenderer.color = Color.yellow;
                }
                else
                {
                    spriteRenderer.color = Color.red;
                }

                // 타원을 그리기 위해 더 많은 점 사용 (32개 점으로 부드러운 타원 생성)
                int ellipsePointCount = 32;
                Vector3[] ellipsePoints = new Vector3[ellipsePointCount + 1];

                NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
                // NavMeshAgent의 크기를 기반으로 타원 크기 설정
                Vector3 center = Vector3.zero; // 중심점은 원점 사용
                Vector3 position = obj.transform.position;

                float halfWidth = size.x * 0.5f;
                float halfHeight = size.y * 0.5f;


                // Draw HP bar
                LineRenderer hpBar = obj.transform.Find("HPBar")?.GetComponent<LineRenderer>();
                LineRenderer hpBarBorder = obj.transform.Find("HPBarBorder")?.GetComponent<LineRenderer>();
                if (hpBar == null)
                {
                    GameObject hpBarObj = new GameObject("HPBar");
                    hpBarObj.transform.SetParent(obj.transform);
                    hpBar = hpBarObj.AddComponent<LineRenderer>();
                    hpBar.positionCount = 2;
                    hpBar.startWidth = 0.1f;
                    hpBar.endWidth = 0.1f;
                    hpBar.material = new Material(Shader.Find("Sprites/Default"));
                    hpBar.startColor = Color.green;
                    hpBar.endColor = Color.green;
                    hpBar.sortingOrder = 4;

                    GameObject hpBarBorderObj = new GameObject("HPBarBorder");
                    hpBarBorderObj.transform.SetParent(obj.transform);
                    hpBarBorder = hpBarBorderObj.AddComponent<LineRenderer>();
                    hpBarBorder.positionCount = 5;
                    hpBarBorder.loop = false;
                    hpBarBorder.startWidth = 0.02f;
                    hpBarBorder.endWidth = 0.02f;
                    hpBarBorder.material = new Material(Shader.Find("Sprites/Default"));
                    hpBarBorder.startColor = Color.gray;
                    hpBarBorder.endColor = Color.gray;
                    hpBarBorder.sortingOrder = 3;
                }

                hpBar.enabled = true;
                hpBarBorder.enabled = true;
                float hpRatio = (float)obj.HP / obj.HPMax;
                Vector3[] hpCorners = new Vector3[2];
                float barY = position.y + center.y - halfHeight + 0.1f;
                if (obj as BuildingBase != null)
                {
                    barY -= 0.4f;
                }
                float barStartX = position.x + center.x - halfWidth;
                float barEndX = position.x + center.x + halfWidth;

                // Draw HP bar
                hpCorners[0] = new Vector3(barStartX, barY, 0);
                hpCorners[1] = new Vector3(barStartX + size.x * hpRatio, barY, 0);
                hpBar.SetPositions(hpCorners);

                // Draw HP bar border
                Vector3[] borderCorners = new Vector3[5];
                borderCorners[0] = new Vector3(barStartX, barY - 0.05f, 0);
                borderCorners[1] = new Vector3(barEndX, barY - 0.05f, 0);
                borderCorners[2] = new Vector3(barEndX, barY + 0.05f, 0);
                borderCorners[3] = new Vector3(barStartX, barY + 0.05f, 0);
                borderCorners[4] = borderCorners[0];
                hpBarBorder.SetPositions(borderCorners);

                // MagicUnit magicUnit = obj.GetComponent<MagicUnit>();
                if (obj.Team == MyTeam && obj.MP > 0)
                {
                    LineRenderer mpBar = obj.transform.Find("MPBar")?.GetComponent<LineRenderer>();
                    LineRenderer mpBarBorder = obj.transform.Find("MPBarBorder")?.GetComponent<LineRenderer>();
                    if (mpBar == null)
                    {
                        GameObject mpBarObj = new GameObject("MPBar");
                        mpBarObj.transform.SetParent(obj.transform);
                        mpBar = mpBarObj.AddComponent<LineRenderer>();
                        mpBar.positionCount = 2;
                        mpBar.startWidth = 0.1f;
                        mpBar.endWidth = 0.1f;
                        mpBar.material = new Material(Shader.Find("Sprites/Default"));
                        mpBar.startColor = Color.blue;
                        mpBar.endColor = Color.blue;
                        mpBar.sortingOrder = 4;
                    }
                    if (mpBarBorder == null)
                    {
                        GameObject mpBarBorderObj = new GameObject("MPBarBorder");
                        mpBarBorderObj.transform.SetParent(obj.transform);
                        mpBarBorder = mpBarBorderObj.AddComponent<LineRenderer>();
                        mpBarBorder.positionCount = 5;
                        mpBarBorder.loop = false;
                        mpBarBorder.startWidth = 0.02f;
                        mpBarBorder.endWidth = 0.02f;
                        mpBarBorder.material = new Material(Shader.Find("Sprites/Default"));
                        mpBarBorder.startColor = Color.gray;
                        mpBarBorder.endColor = Color.gray;
                        mpBarBorder.sortingOrder = 3;
                    }

                    mpBar.enabled = true;
                    mpBarBorder.enabled = true;
                    if (obj.IsSelected)
                    {
                        float mpRatio = 1;//;(float)magicUnit.MP / magicUnit.MPMax;
                        Vector3[] mpCorners = new Vector3[2];
                        float mpBarY = position.y + center.y - halfHeight - 0.3f; // Position below HP bar
                        barStartX = position.x + center.x - halfWidth;
                        barEndX = position.x + center.x + halfWidth;

                        // Draw MP bar
                        mpCorners[0] = new Vector3(barStartX, mpBarY, 0);
                        mpCorners[1] = new Vector3(barStartX + size.x * mpRatio, mpBarY, 0);
                        mpBar.SetPositions(mpCorners);

                        // Draw MP bar border
                        Vector3[] mpBarBorderCorners = new Vector3[5];
                        mpBarBorderCorners[0] = new Vector3(barStartX, mpBarY - 0.05f, 0);
                        mpBarBorderCorners[1] = new Vector3(barEndX, mpBarY - 0.05f, 0);
                        mpBarBorderCorners[2] = new Vector3(barEndX, mpBarY + 0.05f, 0);
                        mpBarBorderCorners[3] = new Vector3(barStartX, mpBarY + 0.05f, 0);
                        mpBarBorderCorners[4] = mpBarBorderCorners[0];
                        mpBarBorder.SetPositions(mpBarBorderCorners);
                    }
                }
            }
            else
            {
                obj.SelectedCircle.gameObject.SetActive(false);
                LineRenderer hpBar = obj.transform.Find("HPBar")?.GetComponent<LineRenderer>();
                LineRenderer hpBarBorder = obj.transform.Find("HPBarBorder")?.GetComponent<LineRenderer>();
                if (hpBar != null)
                {
                    hpBar.enabled = false;
                }
                if (hpBarBorder != null)
                {
                    hpBarBorder.enabled = false;
                }
                LineRenderer mpBar = obj.transform.Find("MPBar")?.GetComponent<LineRenderer>();
                LineRenderer mpBarBorder = obj.transform.Find("MPBarBorder")?.GetComponent<LineRenderer>();
                if (mpBar != null)
                {
                    mpBar.enabled = false;
                }
                if (mpBarBorder != null)
                {
                    mpBarBorder.enabled = false;
                }
            }
        }
    }
    public void OnMoveClick()
    {
        if (SelectedList.Count > 0 && SelectedList[0].Team == MyTeam)
        {
            CurrentOrder = OrderTypes.Move;
        }
        else
        {
            CurrentOrder = OrderTypes.None;
        }
    }
    public void OnStopClick()
    {
        foreach (UnitBase obj in SelectedList)
        {
            Movable Walkable = obj.GetComponent<Movable>();
            if (Walkable != null)
            {
                Walkable.Stop();
            }
        }
    }
    public void OnHoldClick()
    {
        if (SelectedList.Count > 0 && SelectedList[0].Team == MyTeam)
        {
            foreach (UnitBase obj in SelectedList)
            {
                Movable Walkable = obj.GetComponent<Movable>();
                if (Walkable != null)
                {
                    Walkable.Hold();
                }
            }
        }
        else
        {
            CurrentOrder = OrderTypes.None;
        }
    }
    public void OnAttackClick()
    {
        print($"OnAttackClick: {SelectedList.Count}");
        if (SelectedList.Count > 0 && SelectedList[0].Team == MyTeam)
        {
            CurrentOrder = OrderTypes.Attack;
        }
        else
        {
            CurrentOrder = OrderTypes.None;
        }
    }
    public void OnPatrolClick()
    {
        if (SelectedList.Count > 0 && SelectedList[0].Team == MyTeam)
        {
            CurrentOrder = OrderTypes.Patrol;
        }
        else
        {
            CurrentOrder = OrderTypes.None;
        }
    }
    public void OnMenuButtonClick(UnitTypes unit)
    {

    }
    bool IsBuilding(UnitTypes unit)
    {
        return unit == UnitTypes.EngineeringBay ||
         unit == UnitTypes.Barracks ||
         unit == UnitTypes.Factory ||
         unit == UnitTypes.SupplyDepot ||
         unit == UnitTypes.Turret ||
         unit == UnitTypes.CastleLair ||
         unit == UnitTypes.CastleHive ||
         unit == UnitTypes.Castle ||
         unit == UnitTypes.Tree ||
         unit == UnitTypes.GoldMine;
    }
    public void OnBuildClick(GameObject obj)
    {
        string buildingName = obj.name.Replace("btnBuild", "");
        UnitTypes buildingType = (UnitTypes)System.Enum.Parse(typeof(UnitTypes), buildingName);

    }
    public void OnHTBuildClick(GameObject obj)
    {
        string buildingName = obj.name.Replace("btnHTBuild", "");
        UnitTypes buildingType = (UnitTypes)System.Enum.Parse(typeof(UnitTypes), buildingName);

    }
    public void OnSpawnClick(GameObject obj)
    {
        string buildingName = obj.name.Replace("btnSpawn", "");
        UnitTypes buildingType = (UnitTypes)System.Enum.Parse(typeof(UnitTypes), buildingName);

    }
    public void OnMagicClick(GameObject obj)
    {
        string buildingName = obj.name.Replace("btnMagic", "");
        UnitTypes buildingType = (UnitTypes)System.Enum.Parse(typeof(UnitTypes), buildingName);

    }
    UnitBase GetClickedUnit(Vector3 worldPosition)
    {

        // Check if clicked on a unit
        // UnitBase clickedUnit = null;
        // foreach (UnitBase obj in AllUnitList)
        // {
        //     BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        //     if (boxCollider != null && boxCollider.OverlapPoint(worldPosition))
        //     {
        //         clickedUnit = obj;
        //         break;
        //     }
        // }
        // return clickedUnit;

        Collider2D[] collider = Physics2D.OverlapPointAll(worldPosition);
        if (collider != null)
        {
            UnitBase unit = null;
            foreach (Collider2D col in collider)
            {
                unit = col.gameObject.GetComponent<UnitBase>();
                if (unit.MoveType != MoveTypes.Fixed)
                {
                    return unit;
                }
            }
            if (unit != null)
            {
                return unit;
            }
        }
        return null;
    }
    public void AddGold(int gold)
    {
        print($"AddGold: {gold}");
        Gold += gold;
        TheUIScript.UpdateCurrency();
    }
    public void AddLumber(int lumber)
    {
        print($"AddLumber: {lumber}");
        Lumber += lumber;
        TheUIScript.UpdateCurrency();
    }
    public Castle FindCastle(Worker worker)
    {
        foreach (UnitBase obj in AllUnitList)
        {
            if (obj.UnitType == UnitTypes.Castle && obj.Team == worker.Team)
            {
                return obj as Castle;
            }
        }
        return null;
    }
    public GoldMine FindGoldMine(Worker worker)
    {
        foreach (UnitBase obj in AllUnitList)
        {
            if (obj.UnitType == UnitTypes.GoldMine && obj.Team == Identity.Mutual)
            {
                return obj as GoldMine;
            }
        }
        return null;
    }
    public Tree FindTree(Worker worker)
    {
        foreach (UnitBase obj in AllUnitList)
        {
            if (obj.UnitType == UnitTypes.Tree && obj.Team == worker.Team)
            {
                return obj as Tree;
            }
        }
        return null;
    }
    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1) && SelectedList.Count > 0)
        {
            if (SelectedList[0].Team != MyTeam)
            {
                return;
            }
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -TheCamera.transform.position.z;
            Vector3 worldPosition = TheCamera.ScreenToWorldPoint(mousePosition);

            // Check if clicked on a unit
            UnitBase clickedUnit = GetClickedUnit(worldPosition);
            print($"world target Position: {worldPosition}/{clickedUnit}");

            if (clickedUnit != null)
            {
                // If clicked on a unit, handle each unit individually
                foreach (UnitBase obj in SelectedList)
                {
                    // Walkable walkable = obj.GetComponent<Walkable>();
                    // if (walkable != null)
                    {
                        obj.InteractWith(clickedUnit);
                    }
                }
                clickedUnit.ShowTargetted();
            }
            else
            {
                ShowMoveTarget(worldPosition);
                // If clicked on empty space, optimize for multiple units
                if (SelectedList.Count >= 10)
                {
                    // Find the center unit to use as reference
                    Vector3 centerPos = Vector3.zero;
                    foreach (UnitBase unit in SelectedList)
                    {
                        centerPos += unit.transform.position;
                    }
                    centerPos /= SelectedList.Count;

                    // Find the closest unit to the center
                    UnitBase centerUnit = null;
                    float minDist = float.MaxValue;
                    foreach (UnitBase unit in SelectedList)
                    {
                        float dist = Vector3.Distance(unit.transform.position, centerPos);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            centerUnit = unit;
                        }
                    }

                    // Generate path for the center unit
                    Movable centerWalkable = centerUnit.GetComponent<Movable>();
                    if (centerWalkable != null)
                    {
                        // Create target object for center unit
                        GameObject centerTarget = new GameObject("CenterCommandTarget");
                        centerTarget.transform.position = worldPosition;

                        // 중앙 유닛의 경로를 먼저 생성
                        centerWalkable.MoveTo(centerTarget.transform);

                        // 중앙 유닛의 AStarTile 컴포넌트 가져오기
                        AStarTile centerAstar = centerWalkable.GetComponent<AStarTile>();
                        if (centerAstar.path.Count == 0)
                        {
                            print("failed to find path");
                            return;
                        }
                        // 다른 유닛들이 중앙 유닛의 경로를 따라가도록 설정
                        int unitCount = SelectedList.Count;
                        float formationRadius = Mathf.Sqrt(unitCount) * 0.5f;
                        int currentIndex = 0;

                        foreach (UnitBase unit in SelectedList)
                        {
                            if (unit == centerUnit) continue;

                            // 원형 대형 오프셋 계산
                            float angle = (currentIndex * 2 * Mathf.PI) / (unitCount - 1);
                            float radius = formationRadius * (currentIndex / (float)(unitCount - 1));
                            // Vector3 offset = new Vector3(
                            //     Mathf.Cos(angle) * radius,
                            //     Mathf.Sin(angle) * radius,
                            //     0
                            // );

                            // GameObject targetObj = new GameObject($"CommandTarget_{currentIndex}");
                            // targetObj.transform.position = worldPosition;

                            Movable walkable = unit.GetComponent<Movable>();
                            if (walkable != null)
                            {
                                // 중앙 유닛의 경로를 복사하여 사용
                                AStarTile unitAstar = walkable.GetComponent<AStarTile>();
                                if (centerAstar.path != null)// && centerAstar.path.Count > 0)
                                {
                                    // 경로의 각 지점에 오프셋을 적용
                                    // unitAstar.path.Clear();
                                    // foreach (Vector2 pathPoint in centerAstar.path)
                                    // {
                                    //     unitAstar.path.Add(pathPoint + (Vector2)offset);
                                    // }
                                }
                                int joinIndex = Random.Range(0, centerAstar.path.Count);
                                walkable.MoveTo(centerAstar.path[joinIndex]);
                                walkable.targetUnit = null;
                                unitAstar.path.AddRange(centerAstar.path.GetRange(joinIndex, centerAstar.path.Count - joinIndex));
                                walkable.FollowPath();
                            }

                            currentIndex++;
                        }

                        StartCoroutine(CheckArrival(centerTarget));
                    }
                }
                else
                {
                    // For small groups, use individual paths
                    GameObject targetObj = new GameObject("CommandTarget");
                    targetObj.transform.position = worldPosition;

                    foreach (UnitBase obj in SelectedList)
                    {
                        Movable walkable = obj.GetComponent<Movable>();
                        if (walkable != null)
                        {
                            walkable.MoveTo(targetObj.transform);
                            walkable.targetUnit = null;
                        }
                    }

                    StartCoroutine(CheckArrival(targetObj));
                }
            }
        }
    }

    private IEnumerator CheckArrival(GameObject targetObj)
    {
        // Wait for a short time to ensure all units have started moving
        yield return new WaitForSeconds(0.1f);

        // Destroy the target object after a delay
        yield return new WaitForSeconds(1.0f);
        Destroy(targetObj);
    }

    public void ShowEffect(string effectName, Vector2 pos)
    {
        GameObject obj = EffectPool.GetObject(effectName);
        obj.transform.position = pos;
        obj.SetActive(true);
    }
}
