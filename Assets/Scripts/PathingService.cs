using System.Collections.Generic;
using UnityEngine;

public class PathingService : MonoBehaviour
{
    public Transform[] waypointTransforms;

    public List<Vector2> GetPath()
    {
        // Transform to Vector2 list
        List<Vector2> path = new List<Vector2>();
        foreach (var t in waypointTransforms)
        {
            path.Add(t.position);
        }
        return path;
    }

    void OnDrawGizmos()
    {
        if (waypointTransforms == null || waypointTransforms.Length < 2) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < waypointTransforms.Length - 1; i++)
        {
            if (waypointTransforms[i] && waypointTransforms[i + 1])
                Gizmos.DrawLine(waypointTransforms[i].position, waypointTransforms[i + 1].position);
        }
    }
}
