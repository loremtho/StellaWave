using System;

[System.Serializable]
public class StageClearData
{
    public int stageNumber;
    public bool isCleared;

    public StageClearData(int stageNumber, bool isCleared)
    {
        this.stageNumber = stageNumber;
        this.isCleared = isCleared;
    }
}
