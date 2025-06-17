using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void Restart()
    {
        Time.timeScale = 1f; // Nếu bạn có dừng game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
