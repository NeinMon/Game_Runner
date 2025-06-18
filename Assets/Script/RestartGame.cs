using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    private Player_Controller playerController;

    private void Start()
    {
        // Tìm Player_Controller trong scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Player_Controller>();
        }
    }
    
    public void Restart()
    {
        // Đảm bảo timeScale được reset về 1 nếu có dừng game
        Time.timeScale = 1f;
        
        // Cách 1: Gọi hàm RestartGame trong Player_Controller (nếu có)
        if (playerController != null)
        {
            playerController.RestartGame();
            Debug.Log("Đang restart game sử dụng Player_Controller");
        }
        // Cách 2: Nếu không tìm thấy playerController, tải lại scene
        else
        {
            Debug.Log("Đang restart game bằng cách tải lại scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
