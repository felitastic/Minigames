
public class ScoreEntry 
{
    public int boardID { get; private set; }
    public string name;
    public int score;

    public ScoreEntry(int boardID, string name, int score)
    {
        this.boardID = boardID;
        this.name = name;
        this.score = score;
    }
}
