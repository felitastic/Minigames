using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "data.file");
        print(savePath);
    }

    /// <summary>
    /// Returns true if the save was successful
    /// </summary>
    public bool Save(List<ScoreEntry> scores)
    {
        using
            (  
            var writer = new BinaryWriter                
            (File.Open(savePath, FileMode.Create))
            )
        {
            return SaveScores(new GameDataWriter(writer), scores);
        }
    }

    private bool SaveScores(GameDataWriter writer, List<ScoreEntry> scores)
    {
        if (scores.Count <= 0)
            return false;

        writer.Write(scores.Count);

        foreach (ScoreEntry entry in scores)
        {
            writer.Write(entry.boardID);
            writer.Write(entry.name);
            writer.Write(entry.score);
        }
        print("saved " + scores.Count + " scores");
        return true;
    }

    /// <summary>
    // Returns true if Load was successful
    /// </summary>
    public bool Load()
    {
        if (!File.Exists(savePath))
            return false;

        using
            (
            var reader = new BinaryReader
            (File.Open(savePath, FileMode.Open))
            )
        {            
            LoadScores(new GameDataReader(reader));
        }
        return true;
    }

    private void LoadScores(GameDataReader reader)
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();

        int scoreCount = reader.ReadInt();
        for (int i = 0; i < scoreCount; i++)
        {
            int boardID = reader.ReadInt();
            string name = reader.ReadString();
            int score = reader.ReadInt();
            //print(boardID+". board - "+ name + ": " + score);
            scores.Add(new ScoreEntry(boardID, name, score));
        }

        GameManager.Instance.SetScores(scores);
        print("loaded " + scores.Count + " scores");
    }

}

