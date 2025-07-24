using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    public Text rankText;
    public Text nameText;
    public Text distanceText;
    public void SetData(int rank, string name, int distance, bool isUser)
    {
        rankText.text = "#" + rank.ToString();
        nameText.text = name;
        distanceText.text = distance.ToString() + " m";

        if (isUser)
        {
            rankText.color = new Color32(255, 165, 0, 255);
            nameText.color = new Color32(255, 165, 0, 255);
            distanceText.color = new Color32(255, 165, 0, 255);
        }
    }
}
