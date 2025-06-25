using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button startButton;
    public Button leaderboardButton;
    public Button settingsButton;

    [Header("Panels")]
    public GameObject leaderboardPanel;
    public GameObject settingsPopup;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        leaderboardButton.onClick.AddListener(OnLeaderboardButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    void OnStartButtonClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OnLeaderboardButtonClick()
    {
        leaderboardPanel.SetActive(true);
    }

    void OnSettingsButtonClick()
    {
        settingsPopup.SetActive(true);
    }
}
