/*using System.Collections.Generic;
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
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps; // 必须引用这个命名空间

public class MapManager : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(20, 10);
    
    [Header("Tilemaps")]
    public Tilemap roadTilemap;       // 拖入你的道路层
    public Tilemap decorationTilemap; // 拖入你的装饰层（石头、树木）

    private HashSet<Vector2> occupiedCells = new HashSet<Vector2>();

    public bool IsCellAvailable(Vector2 worldPos)
    {
        // 1. 检查边界
        if (Mathf.Abs(worldPos.x) > mapSize.x / 2 || Mathf.Abs(worldPos.y) > mapSize.y / 2) return false;

        // 2. 将世界坐标转换为瓦片坐标 (Grid Coordinates)
        Vector3Int cellPos = roadTilemap.WorldToCell(worldPos);

        // 3. 核心逻辑：检查瓦片地图
        // 如果道路层有瓦片，或者是装饰层有瓦片，则不可用
        if (roadTilemap.HasTile(cellPos)) return false;
        if (decorationTilemap != null && decorationTilemap.HasTile(cellPos)) return false;

        // 4. 检查是否已经被其他塔占用（你原本的逻辑）
        // 建议转换成网格中心点进行比较，防止误差
        Vector2 snappedPos = new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y));
        return !occupiedCells.Contains(snappedPos);
    }

    public void MarkCellOccupied(Vector2 pos)
    {
        // 记录时也取整，确保位置对齐
        Vector2 snappedPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        occupiedCells.Add(snappedPos);
    }
}