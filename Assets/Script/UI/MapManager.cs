using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{

    public Button map1Button;
    public Button map2Button;
    public Button map3Button;
    public Button map4Button;
    public Button map5Button;
    public Button map6Button;


    public void Start()
    {
        map1Button.onClick.AddListener(RenderMap1);
        map2Button.onClick.AddListener(RenderMap2);
        map3Button.onClick.AddListener(RenderMap3);
        map4Button.onClick.AddListener(RenderMap4);
        map5Button.onClick.AddListener(RenderMap5);
        map6Button.onClick.AddListener(RenderMap6);

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
        SceneManager.LoadScene("map1");
    }

    public void RenderMap2()
    {
        SceneManager.LoadScene("map2");
    }
    public void RenderMap3()
    {
        SceneManager.LoadScene("map3");
    }
    public void RenderMap4()
    {
        SceneManager.LoadScene("map4");
    }
    public void RenderMap5()
    {
        SceneManager.LoadScene("map5");
    }
    public void RenderMap6()
    {
        SceneManager.LoadScene("map6");
    }
}
