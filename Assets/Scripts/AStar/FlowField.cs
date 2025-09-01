using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    private float cellSize;
    private float fieldRadius;
    private LayerMask obstacleLayer;
    private Vector2Int gridSize;
    private Vector3 origin;
    private Vector2[,] flowField;
    private float[,] costField;
    private int[,] integrationField;
    private bool[,] visited;
    private PriorityQueue<Vector2Int> openList;

    private class PriorityQueue<T>
    {
        private List<(T item, int priority)> items = new List<(T, int)>();

        public int Count => items.Count;

        public void Enqueue(T item, int priority)
        {
            items.Add((item, priority));
            int i = items.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (items[parent].priority <= items[i].priority)
                    break;

                var temp = items[parent];
                items[parent] = items[i];
                items[i] = temp;
                i = parent;
            }
        }

        public T Dequeue()
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            T result = items[0].item;
            int lastIndex = items.Count - 1;
            items[0] = items[lastIndex];
            items.RemoveAt(lastIndex);

            int i = 0;
            while (true)
            {
                int leftChild = 2 * i + 1;
                if (leftChild >= items.Count)
                    break;

                int rightChild = leftChild + 1;
                int minIndex = i;

                if (items[leftChild].priority < items[minIndex].priority)
                    minIndex = leftChild;

                if (rightChild < items.Count && items[rightChild].priority < items[minIndex].priority)
                    minIndex = rightChild;

                if (minIndex == i)
                    break;

                var temp = items[i];
                items[i] = items[minIndex];
                items[minIndex] = temp;
                i = minIndex;
            }

            return result;
        }
    }

    public void SetField(float cellSize, float fieldRadius, LayerMask obstacleLayer)
    {
        this.cellSize = cellSize;
        this.fieldRadius = fieldRadius;
        this.obstacleLayer = obstacleLayer;

        // Calculate grid size based on field radius
        int cellsPerSide = Mathf.CeilToInt(fieldRadius * 2 / cellSize);
        gridSize = new Vector2Int(cellsPerSide, cellsPerSide);

        flowField = new Vector2[gridSize.x, gridSize.y];
        costField = new float[gridSize.x, gridSize.y];
        integrationField = new int[gridSize.x, gridSize.y];
        visited = new bool[gridSize.x, gridSize.y];
        openList = new PriorityQueue<Vector2Int>();
    }

    public void GenerateField(Vector3 targetPosition)
    {
        StartCoroutine(GenerateFieldCoroutine(targetPosition));
    }
    IEnumerator GenerateFieldCoroutine(Vector3 targetPosition)
    {
        origin = targetPosition - new Vector3(fieldRadius, fieldRadius);
        yield return null;
        GenerateCostField();
        yield return null;
        GenerateIntegrationField(targetPosition);
        yield return null;
        GenerateFlowField();
    }

    private void GenerateCostField()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = GridToWorld(new Vector2Int(x, y));

                // Check for obstacles
                Collider[] obstacles = Physics.OverlapSphere(worldPos, cellSize * 0.5f, obstacleLayer);
                if (obstacles.Length > 0)
                {
                    costField[x, y] = float.MaxValue; // Impassable
                }
                else
                {
                    costField[x, y] = 1f; // Normal cost
                }
            }
        }
    }

    private void GenerateIntegrationField(Vector3 targetPosition)
    {
        // Reset visited array
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                visited[x, y] = false;
                integrationField[x, y] = int.MaxValue;
            }
        }

        // Get target cell
        Vector2Int targetCell = WorldToGrid(targetPosition);
        if (!IsValidCell(targetCell)) return;

        // Set target cell cost to 0
        integrationField[targetCell.x, targetCell.y] = 0;
        openList.Enqueue(targetCell, 0);

        // Pre-allocate neighbor offsets
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1),
            new Vector2Int(0, -1),                          new Vector2Int(0, 1),
            new Vector2Int(1, -1),  new Vector2Int(1, 0),   new Vector2Int(1, 1)
        };

        while (openList.Count > 0)
        {
            Vector2Int current = openList.Dequeue();
            if (visited[current.x, current.y]) continue;
            visited[current.x, current.y] = true;

            int currentCost = integrationField[current.x, current.y];

            // Check all 8 neighboring cells
            foreach (var offset in neighborOffsets)
            {
                Vector2Int neighbor = new Vector2Int(current.x + offset.x, current.y + offset.y);
                if (!IsValidCell(neighbor) || visited[neighbor.x, neighbor.y]) continue;

                // Skip if cell is impassable
                if (costField[neighbor.x, neighbor.y] == float.MaxValue) continue;

                // Calculate new cost
                int newCost = currentCost + 1;
                if (offset.x != 0 && offset.y != 0) newCost++; // Diagonal movement costs more

                if (newCost < integrationField[neighbor.x, neighbor.y])
                {
                    integrationField[neighbor.x, neighbor.y] = newCost;
                    openList.Enqueue(neighbor, newCost);
                }
            }
        }
    }

    private void GenerateFlowField()
    {
        // Pre-allocate neighbor offsets
        Vector2Int[] neighborOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1),
            new Vector2Int(0, -1),                          new Vector2Int(0, 1),
            new Vector2Int(1, -1),  new Vector2Int(1, 0),   new Vector2Int(1, 1)
        };

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (costField[x, y] == float.MaxValue)
                {
                    flowField[x, y] = Vector2.zero;
                    continue;
                }

                Vector2Int current = new Vector2Int(x, y);
                Vector2Int lowestNeighbor = current;
                int lowestCost = integrationField[x, y];

                // Check all 8 neighboring cells
                foreach (var offset in neighborOffsets)
                {
                    Vector2Int neighbor = new Vector2Int(x + offset.x, y + offset.y);
                    if (!IsValidCell(neighbor)) continue;

                    if (integrationField[neighbor.x, neighbor.y] < lowestCost)
                    {
                        lowestCost = integrationField[neighbor.x, neighbor.y];
                        lowestNeighbor = neighbor;
                    }
                }

                // Set flow direction towards the lowest cost neighbor
                if (lowestNeighbor != current)
                {
                    Vector2 direction = new Vector2(lowestNeighbor.x - current.x, lowestNeighbor.y - current.y).normalized;
                    flowField[x, y] = direction;
                }
                else
                {
                    flowField[x, y] = Vector2.zero;
                }
            }
        }
    }

    public Vector2 GetDirection(Vector2 worldPosition)
    {
        Vector2Int cell = WorldToGrid(worldPosition);
        if (!IsValidCell(cell)) return Vector2.zero;
        return flowField[cell.x, cell.y];
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 relativePos = worldPosition - origin;
        int x = Mathf.FloorToInt(relativePos.x / cellSize);
        int y = Mathf.FloorToInt(relativePos.y / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return origin + new Vector3(
            gridPosition.x * cellSize + cellSize * 0.5f,
            gridPosition.y * cellSize + cellSize * 0.5f,
            0
        );
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridSize.x && cell.y >= 0 && cell.y < gridSize.y;
    }
}