using UnityEngine;

[CreateAssetMenu(fileName = "newLevel", menuName = "ScriptableObjects/Level")]
public class LevelSetting : ScriptableObject
{
    [Tooltip("Width of the whole board, including borders left/right")]
    public int BoardWidth = 10;
    [Tooltip("Height of the whole board, including borders bottom/top")]
    public int BoardHeight = 20;
    public int[] GoalPositions;
    [Tooltip("Positions of the goal tiles")]
    public int[] ObstaclePositions;
    public TetrisMovement PlayerShape;
    [Tooltip("Length of one side of the tile for positioning")]
    public int TileSize = 50;
    [Tooltip("Offset on y-axis to keep board centered")]
    public int XOffset = 200;
    [Tooltip("Offset on x-axis to keep board centered")]
    public int YOffset = 450;
}
