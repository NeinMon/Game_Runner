using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{

    public Button map1Button;

    public void Start()
    {
        map1Button.onClick.AddListener(RenderMap1);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void RenderMap1()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
