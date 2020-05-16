using UnityEngine;
using UnityEditor;

/// <summary>
/// Shows position of the piece in the inspector
/// </summary>
public class BoardTileInfo
{
    static GUIStyle Style(Color textCol)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = textCol;
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        return style;
    }

    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawTilePos(Piece piece, GizmoType gizmoType)
    {
        GUIStyle style = Style(Color.cyan);
        //string pos = piece.name;
        string pos = Mathf.RoundToInt(piece.transform.position.x)+","+ Mathf.RoundToInt(piece.transform.position.y);
        Handles.Label(new Vector3(piece.transform.position.x - 0.45f, piece.transform.position.y + 0.5f, piece.transform.position.z), pos, style);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(piece.transform.position, new Vector3(1, 1, 0));

    }
    
    [DrawGizmo(GizmoType.Selected)]
    static void DrawSelectedTilePos(Piece piece, GizmoType gizmoType)
    {
        GUIStyle style = Style(Color.red);
        //string pos = piece.name;
        string pos = Mathf.RoundToInt(piece.transform.position.x) + "," + Mathf.RoundToInt(piece.transform.position.y);
        Handles.Label(new Vector3(piece.transform.position.x - 0.45f, piece.transform.position.y + 0.5f, piece.transform.position.z), pos, style);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(piece.transform.position, new Vector3(1, 1, 0));
    }
}
