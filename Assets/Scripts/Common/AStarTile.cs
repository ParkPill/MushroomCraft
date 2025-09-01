using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarTile : MonoBehaviour
{
    public List<Vector2> path;
    public bool isPathGenerated = false;

    // Visualization variables
    private static readonly Color OPEN_SET_COLOR = new Color(0, 1, 0, 0.3f);  // Semi-transparent green
    private static readonly Color CLOSED_SET_COLOR = new Color(1, 0, 0, 0.3f); // Semi-transparent red
    private static readonly Color PATH_COLOR = Color.green;
    private static readonly Color CURRENT_NODE_COLOR = Color.yellow;
    private static readonly float NODE_SIZE = 0.3f;

    // Object pool for Node instances
    private static readonly Stack<Node> nodePool = new Stack<Node>(100);
    private static Node[,] nodeArray;
    private static readonly HashSet<(int x, int y)> closedSet = new HashSet<(int x, int y)>(100);
    private static readonly SortedList<float, Node> openSet = new SortedList<float, Node>(100);
    private static float uniqueKeyCounter = 0f;

    private class Node
    {
        public int x, y;
        public float gCost;
        public float hCost;
        public Node parent;
        public float uniqueKey;

        public float fCost => gCost + hCost;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Reset(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.gCost = 0;
            this.hCost = 0;
            this.parent = null;
            this.uniqueKey = uniqueKeyCounter++;
        }
    }

    private static Node GetNode(int x, int y)
    {
        if (nodePool.Count > 0)
        {
            var node = nodePool.Pop();
            node.Reset(x, y);
            return node;
        }
        return new Node(x, y);
    }

    private static void ReturnNode(Node node)
    {
        nodePool.Push(node);
    }

    void Start()
    {
        path = new List<Vector2>(100);
    }

    private bool IsValidPosition(int x, int y)
    {
        return CCAStarManager.Instance.IsValidPosition(x, y);
    }

    private bool HasLineOfSight(int x1, int y1, int x2, int y2)
    {
        int[][] map = CCAStarManager.Instance.Map;
        int dx = Mathf.Abs(x2 - x1);
        int dy = Mathf.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x1 == x2 && y1 == y2) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                if (x1 == x2) break;
                err -= dy;
                x1 += sx;
            }
            if (e2 < dx)
            {
                if (y1 == y2) break;
                err += dx;
                y1 += sy;
            }

            // Check if current position is walkable
            if (!IsValidPosition(x1, y1) || map[y1][x1] != 0)
            {
                return false;
            }
        }
        return true;
    }

    private float GetDistance(int x1, int y1, int x2, int y2)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
    public List<Vector2> GenerateFlyPath(int startX, int startY, int endX, int endY)
    {
        print($"GenerateFlyPath: {startX}, {startY}, {endX}, {endY}");
        path.Clear();
        isPathGenerated = true;
        path.Add(new Vector2(startX, startY));
        path.Add(new Vector2(endX, endY));
        return path;
    }

    public List<Vector2> GeneratePath(int startX, int startY, int endX, int endY)
    {
        print($"GeneratePath: {startX}, {startY}, {endX}, {endY}");
        path.Clear();
        isPathGenerated = false;
        uniqueKeyCounter = 0f;
        int[][] map = CCAStarManager.Instance.Map;

        if (!IsValidPosition(startX, startY) || !IsValidPosition(endX, endY))
            return path;

        if (map[endY][endX] != 0) // 0 = Walkable
            return path;

        // Check if there's a direct line of sight
        if (HasLineOfSight(startX, startY, endX, endY))
        {
            path.Add(new Vector2(startX, startY));
            path.Add(new Vector2(endX, endY));
            isPathGenerated = true;
            return path;
        }

        // Initialize node array if needed
        if (nodeArray == null || nodeArray.GetLength(0) != CCAStarManager.Instance.mapWidth ||
            nodeArray.GetLength(1) != CCAStarManager.Instance.mapHeight)
        {
            nodeArray = new Node[CCAStarManager.Instance.mapWidth, CCAStarManager.Instance.mapHeight];
        }
        else
        {
            // Clear existing nodes
            for (int x = 0; x < CCAStarManager.Instance.mapWidth; x++)
            {
                for (int y = 0; y < CCAStarManager.Instance.mapHeight; y++)
                {
                    nodeArray[x, y] = null;
                }
            }
        }

        closedSet.Clear();
        openSet.Clear();

        var startNode = GetNode(startX, startY);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(startX, startY, endX, endY);

        openSet.Add(startNode.fCost + startNode.uniqueKey * 0.0001f, startNode);
        nodeArray[startX, startY] = startNode;

        while (openSet.Count > 0)
        {
            var current = openSet.Values[0];
            openSet.RemoveAt(0);

            if (current.x == endX && current.y == endY)
            {
                // Path found, reconstruct it
                while (current != null)
                {
                    path.Add(new Vector2(current.x, current.y));
                    current = current.parent;
                }
                path.Reverse();

                // Return all nodes to pool
                for (int x = 0; x < CCAStarManager.Instance.mapWidth; x++)
                {
                    for (int y = 0; y < CCAStarManager.Instance.mapHeight; y++)
                    {
                        if (nodeArray[x, y] != null)
                        {
                            ReturnNode(nodeArray[x, y]);
                            nodeArray[x, y] = null;
                        }
                    }
                }

                if (path.Count > 0)
                {
                    isPathGenerated = true;
                }

                return path;
            }

            closedSet.Add((current.x, current.y));

            // Check all 8 directions
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int newX = current.x + dx;
                    int newY = current.y + dy;

                    if (!IsValidPosition(newX, newY) || map[newY][newX] != 0) // 0 = Walkable
                        continue;

                    if (closedSet.Contains((newX, newY)))
                        continue;

                    float newGCost = current.gCost + (dx != 0 && dy != 0 ? 1.414f : 1f);

                    Node neighbor = nodeArray[newX, newY];
                    if (neighbor == null)
                    {
                        neighbor = GetNode(newX, newY);
                        nodeArray[newX, newY] = neighbor;
                    }
                    else if (newGCost >= neighbor.gCost)
                        continue;

                    neighbor.parent = current;
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistance(newX, newY, endX, endY);
                    neighbor.uniqueKey = uniqueKeyCounter++;

                    openSet.Add(neighbor.fCost + neighbor.uniqueKey * 0.0001f, neighbor);
                }
            }
        }

        // Return all nodes to pool
        for (int x = 0; x < CCAStarManager.Instance.mapWidth; x++)
        {
            for (int y = 0; y < CCAStarManager.Instance.mapHeight; y++)
            {
                if (nodeArray[x, y] != null)
                {
                    ReturnNode(nodeArray[x, y]);
                    nodeArray[x, y] = null;
                }
            }
        }

        return path;
    }

    void Update()
    {
        // if (isPathGenerated && path != null && path.Count >= 2)
        // {
        //     // Draw the final path
        //     for (int i = 0; i < path.Count - 1; i++)
        //     {
        //         Vector3 start = new Vector3(path[i].x, path[i].y, 0);
        //         Vector3 end = new Vector3(path[i + 1].x, path[i + 1].y, 0);
        //         Debug.DrawLine(start, end, PATH_COLOR, Time.deltaTime);
        //     }

        //     // Draw open set nodes
        //     foreach (var node in openSet.Values)
        //     {
        //         Vector3 pos = new Vector3(node.x, node.y, 0);
        //         DrawNode(pos, OPEN_SET_COLOR);
        //     }

        //     // Draw closed set nodes
        //     foreach (var (x, y) in closedSet)
        //     {
        //         Vector3 pos = new Vector3(x, y, 0);
        //         DrawNode(pos, CLOSED_SET_COLOR);
        //     }

        //     // Draw current node if there is one
        //     if (openSet.Count > 0)
        //     {
        //         Vector3 currentPos = new Vector3(openSet.Values[0].x, openSet.Values[0].y, 0);
        //         DrawNode(currentPos, CURRENT_NODE_COLOR);
        //     }
        // }
    }

    // private void DrawNode(Vector3 position, Color color)
    // {
    //     // Draw a small square at the node position
    //     Vector3 size = Vector3.one * NODE_SIZE;
    //     Vector3 halfSize = size * 0.5f;

    //     // Draw the square using Debug.DrawLine
    //     Debug.DrawLine(position - halfSize, position + new Vector3(halfSize.x, -halfSize.y, 0), color, Time.deltaTime);
    //     Debug.DrawLine(position - halfSize, position + new Vector3(-halfSize.x, halfSize.y, 0), color, Time.deltaTime);
    //     Debug.DrawLine(position + halfSize, position + new Vector3(-halfSize.x, halfSize.y, 0), color, Time.deltaTime);
    //     Debug.DrawLine(position + halfSize, position + new Vector3(halfSize.x, -halfSize.y, 0), color, Time.deltaTime);
    // }

    public void SetPath(List<Vector2> newPath)
    {
        path = new List<Vector2>(newPath);
        isPathGenerated = true;
    }
}
