using UnityEngine;
using UnityEditor;

public class TetrisTileInfo
{
    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawTilePos(Tile tile, GizmoType gizmoType)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(new Vector3 (tile.transform.position.x-0.25f, tile.transform.position.y+0.25f, tile.transform.position.z),tile.PosInArray.ToString(), style);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(tile.transform.position, new Vector3(1,1,0));

    }

    [DrawGizmo(GizmoType.Selected)]
    static void DrawSelectedTilePos(Tile tile, GizmoType gizmoType)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(new Vector3(tile.transform.position.x - 0.25f, tile.transform.position.y + 0.25f, tile.transform.position.z), tile.PosInArray.ToString(), style);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(tile.transform.position, new Vector3(1, 1, 0));
    }
}
