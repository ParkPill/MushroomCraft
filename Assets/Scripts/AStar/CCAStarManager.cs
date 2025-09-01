using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAStarManager : MonoBehaviour
{
    private static CCAStarManager _instance;
    public static CCAStarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("CCAstarManager").AddComponent<CCAStarManager>();
                _instance.name = "CCAstarManager";
            }
            return _instance;
        }
    }

    public int[][] Map;
    public int mapWidth;
    public int mapHeight;

    public void SetMap(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;
        Map = new int[height][];

        for (int i = 0; i < height; i++)
        {
            Map[i] = new int[width];
            for (int j = 0; j < width; j++)
            {
                Map[i][j] = 0; // 0 = Walkable
            }
        }
    }

    public void SetState(int x, int y, int state)
    {
        if (IsValidPosition(x, y))
        {
            // print($"SetState: {x}, {y}, {state}");
            Map[y][x] = state;
        }
    }

    public int GetState(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            return Map[y][x];
        }
        return 1; // 1 = Block
    }

    public bool IsValidPosition(int x, int y)
    {
        return y >= 0 && y < mapHeight && x >= 0 && x < mapWidth;
    }

    void Update()
    {
        // Draw blocked cells with red lines
        // for (int y = 0; y < mapHeight; y++)
        // {
        //     for (int x = 0; x < mapWidth; x++)
        //     {
        //         if (Map[y][x] == 1) // 1 = Block
        //         {
        //             // Draw red box around blocked cell
        //             Vector3 bottomLeft = new Vector3(x, y, 0);
        //             Vector3 bottomRight = new Vector3(x + 1, y, 0);
        //             Vector3 topLeft = new Vector3(x, y + 1, 0);
        //             Vector3 topRight = new Vector3(x + 1, y + 1, 0);

        //             // Draw lines for blocked cell boundaries
        //             Debug.DrawLine(bottomLeft, bottomRight, Color.red, Time.deltaTime);
        //             Debug.DrawLine(bottomRight, topRight, Color.red, Time.deltaTime);
        //             Debug.DrawLine(topRight, topLeft, Color.red, Time.deltaTime);
        //             Debug.DrawLine(topLeft, bottomLeft, Color.red, Time.deltaTime);
        //         }
        //     }
        // }

    }
}