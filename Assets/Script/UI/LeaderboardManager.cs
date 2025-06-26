using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{

    public GameObject closeBtn;

    void Start()
    {
        if (closeBtn != null)
        {
            Button btn = closeBtn.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(Close);
            }
        }
    }


    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
