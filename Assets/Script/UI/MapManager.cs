using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{

    private List<int> completedMaps = new List<int>();

    public Button map1Button;
    public Button map2Button;
    public Button map3Button;
    public Button map4Button;
    public Button map5Button;
    public Button map6Button;

    public Text scoreText;

    private int totalScore;

    public GameObject scoreZone;
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
        string uid = AuthService.Instance.GetUser()?.UserId;
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogWarning("User not logged in. Unlocking only map1.");
            completedMaps = new List<int> { 1 };
            HandleSetInteractableMap();
            gameObject.SetActive(true);
            scoreZone.SetActive(false);
            return;
        }

        DaoService.Instance.GetLatestScoreOfUser(uid, total_score =>
        {
            scoreText.text = total_score.ToString();
            scoreZone.SetActive(true);
            DaoService.Instance.GetCompletedProgressOfUserByUID(uid, maps_retrieved =>
            {
                completedMaps = maps_retrieved;
                HandleSetInteractableMap();
                gameObject.SetActive(true);
            });
        });

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

    private void HandleSetInteractableMap()
    {
        SetCannotInteractableMaps();
        foreach (int index in completedMaps)
        {
            switch (index)
            {
                case 1: map1Button.interactable = true; break;
                case 2: map2Button.interactable = true; break;
                case 3: map3Button.interactable = true; break;
                case 4: map4Button.interactable = true; break;
                case 5: map5Button.interactable = true; break;
                case 6: map6Button.interactable = true; break;
            }
        }
    }

    private void SetCannotInteractableMaps()
    {
        map1Button.interactable = true;
        map2Button.interactable = false;
        map3Button.interactable = false;
        map4Button.interactable = false;
        map5Button.interactable = false;
        map6Button.interactable = false;
    }

}
