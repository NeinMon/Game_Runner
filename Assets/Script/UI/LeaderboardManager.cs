using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LeaderboardManager : MonoBehaviour
{

    public Text textMapNumber;

    public GameObject contentParent;

    public GameObject prefabScore;


    private int mapNumber = 1;



    public void Open()
    {
        gameObject.SetActive(true);
        RenderTextMapNumber();
        RenderRowsForMap();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void RenderRowsForMap()
    {
        foreach (Transform child in contentParent.transform)
        {
            Debug.Log("Erase objects for render rows");
            Destroy(child.gameObject);
        }

        DaoService.Instance.GetLeaderBoardOfMap(mapNumber, list =>
        {
            if (list == null)
            {
                Debug.Log("LEADER BOARD CANNOT GET");
                return;
            }
            int rank = 1;
            string uid = AuthService.Instance.GetUser()?.UserId;
            foreach (Dictionary<string, object> rowData in list)
            {
                GameObject row = Instantiate(prefabScore, contentParent.transform);

                string name = rowData.ContainsKey("name") ? rowData["name"].ToString() : "Unknown";
                int distance = rowData.ContainsKey("distance") ? Convert.ToInt32(rowData["distance"]) : 0;
                string uidCompared = rowData.ContainsKey("uid") ? rowData["uid"].ToString() : "___";

                bool isCurrentUser = uid != null && uid.Equals(uidCompared);

                row.GetComponent<LeaderboardRow>()?.SetData(rank, name, distance, isCurrentUser);

                rank++;
            }
        });
    }

    public void NextMap()
    {
        mapNumber++;
        if (mapNumber == 7) mapNumber = 6;
        RenderTextMapNumber();
        RenderRowsForMap();
    }

    public void PreviousMap()
    {
        mapNumber--;
        if (mapNumber == 0) mapNumber = 1;
        RenderTextMapNumber();
        RenderRowsForMap();
    }

    public void RenderTextMapNumber()
    {
        textMapNumber.text = "MAP " + mapNumber.ToString();
    }
}
