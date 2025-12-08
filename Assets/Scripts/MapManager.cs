using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(20, 10);
    // Use HashSet records occupied coordinates
    private HashSet<Vector2> occupiedCells = new HashSet<Vector2>();

    // Mark the path as occupied
    public void MarkPathOccupied(List<Vector2> path)
    {
        foreach (var pos in path)
        {
            // Mark the width of path
            MarkCellOccupied(new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y)));
        }
    }

    public bool IsCellAvailable(Vector2 pos)
    {
        // Check boundaries
        if (Mathf.Abs(pos.x) > mapSize.x / 2 || Mathf.Abs(pos.y) > mapSize.y / 2) return false;
        return !occupiedCells.Contains(pos);
    }

    public void MarkCellOccupied(Vector2 pos)
    {
        occupiedCells.Add(pos);
    }
}
