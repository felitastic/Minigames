using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : ScriptableObject
{
    public int TreasureSize { get; private set; }
    public string[] Name { get; private set; }
    public int[] TotalScore { get; private set; }
    public int[] GemsPerMove { get; private set; }

    public void WriteAllValues(string[] pName, int[] pTotalScore, int[] pMoveScore)
    {
        Name = pName;
        TotalScore = pTotalScore;
        GemsPerMove = pMoveScore;
    }

    public void OverwriteOneEntry(int pNumber, string pName, int pTotalScore, int pMoveScore)
    {
        Name[pNumber] = pName;
        TotalScore[pNumber] = pTotalScore;
        GemsPerMove[pNumber] = pMoveScore;
    }
}
