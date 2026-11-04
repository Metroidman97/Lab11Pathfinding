using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pathfinding : MonoBehaviour
{
    public List<Vector2Int> path = new List<Vector2Int>();
    public Vector2Int start = new Vector2Int(0, 1);
    public Vector2Int goal = new Vector2Int(4, 4);
    private Vector2Int next;
    private Vector2Int current;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    public int[,] grid;

    public int gridWidth;
    public int gridHeight;

    [Range(0,1)]
    public float obstacleChance;

    public Vector2Int obstaclePosition;

    // Set to determine if grid obstacles can be set manually or randomly
    public bool randomGrid = true;

    private void Start()
    {
        grid = GenerateRandomGrid(gridWidth, gridHeight, obstacleChance);
        FindPath(start, goal);
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        // Draw start and goal
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    public int[,] GenerateRandomGrid(int width,  int height, float obstacleProbability)
    {
        grid = new int[width, height];
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                float obstacleChance = Random.Range(0, 1f);
                if (obstacleProbability > obstacleChance)
                {
                    // Check if the selected grid space is also the start or end goal
                    if ((y == start.y && x == start.x) || (y == goal.y && x == goal.x))
                    {
                        grid[y, x] = 0;
                    }
                    else
                    {
                        grid[y, x] = 1;
                    }
                }
                else
                {
                    grid[y, x] = 0;
                }
            }
        }

        return grid;
    }

    public bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    public void FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }

    public void AddObstacle(Vector2Int position)
    {
        if (grid[position.x, position.y] == 0)
        {
            grid[position.x, position.y] = 1;
        }
        else
        {
            grid[position.x, position.y] = 0;
        }
    }
}

[CustomEditor(typeof(Pathfinding))]
public class PathfindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var t = target as Pathfinding;
        base.OnInspectorGUI();

        if (!t.randomGrid)
        {
            if (GUILayout.Button("Set obstacle"))
            {
                if (t.IsInBounds(t.obstaclePosition))
                {
                    t.path.Clear();
                    t.AddObstacle(t.obstaclePosition);
                    t.FindPath(t.start, t.goal);
                }
                else
                {
                    Debug.LogError("Obstacle position is out of bounds");
                }
            }
        }
        else if (t.randomGrid)
        {
            if (GUILayout.Button("Generate new grid"))
            {
                t.path.Clear();
                t.grid = t.GenerateRandomGrid(t.gridWidth, t.gridHeight, t.obstacleChance);
                t.FindPath(t.start, t.goal);
            }
        }
    }
}

