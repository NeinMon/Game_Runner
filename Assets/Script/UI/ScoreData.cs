[System.Serializable]
public class ScoreData
{
    public int score;
    public long completeTime;
    public string displayName;

    public ScoreData(int score, long completeTime, string displayName)
    {
        this.score = score;
        this.completeTime = completeTime;
        this.displayName = displayName;
    }
}