using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button settingsButton;
    public Button leaderboardButton;
    public Button mapsButton;

    [Header("Managers")]
    public SettingManager settingManager;
    public LeaderboardManager leaderboardManager;
    public MapsManager mapsManager;

    void Start()
    {
        settingsButton.onClick.AddListener(OnOpenSettings);
        leaderboardButton.onClick.AddListener(OnOpenLeaderboard);
        mapsButton.onClick.AddListener(OnOpenMaps);
    }

    void OnOpenSettings()
    {
        settingManager.Open();
    }

    void OnOpenLeaderboard()
    {
        leaderboardManager.Open();
    }

    void OnOpenMaps()
    {
        leaderboardManager.Open();
    }
}
