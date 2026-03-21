using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private readonly GridSystem gridSystem;

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        // Uncomment if diagonal movement is allowed
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    public PathFinder(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    private int GetKey(int row, int col)
    {
        // range from -32768 to 32767, should be enough for most grid sizes
        return (row << 16) | (col & 0xFFFF);
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld)
    {
        var (startRow, startCol) = gridSystem.WorldToCell(startWorld);
        var (endRow, endCol) = gridSystem.WorldToCell(endWorld);

        var openHeap = new MinHeap<PathNode>();
        var allNodes = new Dictionary<int, PathNode>();

        int startKey = GetKey(startRow, startCol);
        var startNode = new PathNode(startRow, startCol);

        openHeap.Add(startNode);
        allNodes[startKey] = startNode;

        while (openHeap.Count > 0)
        {
            var current = openHeap.RemoveFirst();
            current.closed = true;

            if (current.row == endRow && current.col == endCol)
                return RetracePath(current);

            foreach (var dir in Directions)
            {
                int newRow = current.row + dir.y;
                int newCol = current.col + dir.x;

                if (!gridSystem.IsWalkable(newRow, newCol))
                    continue;

                int key = GetKey(newRow, newCol);

                if (!allNodes.TryGetValue(key, out var neighbor))
                {
                    neighbor = new PathNode(newRow, newCol);
                    allNodes[key] = neighbor;
                }

                if (neighbor.closed)
                    continue;

                int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10;
                int newGCost = current.gCost + moveCost;

                if (newGCost < neighbor.gCost || !neighbor.opened)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Heuristic(newRow, newCol, endRow, endCol);
                    neighbor.parent = current;

                    if (!neighbor.opened)
                    {
                        openHeap.Add(neighbor);
                        neighbor.opened = true;
                    }
                    else
                    {
                        openHeap.UpdateItem(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private int Heuristic(int r1, int c1, int r2, int c2)
    {
        // Manhattan
        return Mathf.Abs(r1 - r2) + Mathf.Abs(c1 - c2);
    }

    private List<Vector3> RetracePath(PathNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        PathNode current = endNode;

        while (current != null)
        {
            path.Add(gridSystem.CellToWorld(current.row, current.col));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}
