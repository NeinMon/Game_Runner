using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float laneDistance = 1f;
    private int targetLane = 1; // 0 = Left, 1 = Mid, 2 = Right
    private CharacterController controller;
    [Header("Animation")]
    [SerializeField] private Animator player_Animation; // Kéo thả Animator vào Inspector
    private Queue<int> laneChangeQueue = new Queue<int>();
    private bool isChangingLane = false;
    private bool isJumping = false;
    [SerializeField] private float jumpForce = 2.2f; // Lực nhảy
    [SerializeField] private float gravity = -5f; // Trọng lực, có thể chỉnh trong Inspector
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject playPanel;
    private bool isGameStarted = false;
    private int score = 0;
    private float originalSpeed;
    [SerializeField] private float fastDuration = 3f; // Thời gian chạy nhanh (giây)
    [SerializeField] private float slowDuration = 3f; // Thời gian chạy chậm (giây)
    [SerializeField] private float fastMultiplier = 2f;
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float minForwardSpeed = 1f;
    [SerializeField] private float maxForwardSpeed = 4f;
    [SerializeField] private float speedIncreaseRate = 0.05f;
    private float baseSpeed;
    private float speedMultiplier = 1f;
    private Vector3 currentLanePosition; // Vị trí hiện tại để làm mượt chuyển làn
    private float currentLaneOffset = 0f; // Độ lệch X hiện tại để làm mượt chuyển làn
    private float laneOffsetVelocity = 0f;
    [SerializeField] private float laneSmoothTime = 0.15f; // Thời gian làm mượt chuyển làn
    private Vector3 laneVelocity = Vector3.zero;
    private Vector3 forwardMove = Vector3.zero; // Vector di chuyển thẳng
    private Vector3 laneMove = Vector3.zero;    // Vector di chuyển chuyển làn
    [SerializeField] private float invisibleDuration = 4f; // Thời gian tàng hình và hiệu ứng (giây)
    private bool isInvisible = false;
    private Renderer[] renderers;
    private Collider playerCollider;
    [SerializeField] private ParticleSystem invisibleEffect; // Hiệu ứng khi tàng hình
    [SerializeField] private ParticleSystem thunderEffect; // Hiệu ứng khi ăn Thunder
    [SerializeField] private ParticleSystem timeEffect;    // Hiệu ứng khi ăn Time

    void Start()
    {
        controller = GetComponent<CharacterController>();
        baseSpeed = minForwardSpeed;
        forwardSpeed = minForwardSpeed;
        originalSpeed = minForwardSpeed;
        if (restartPanel != null)
            restartPanel.SetActive(false); // Ẩn panel khi bắt đầu game
        if (playPanel != null)
            playPanel.SetActive(true); // Hiện panel PLAY khi vào game
        isGameStarted = false;
        // Đảm bảo animation idle được kích hoạt khi bắt đầu game
        if (player_Animation != null)
        {
            player_Animation.SetFloat("is_running", 0.0f); // Set trạng thái idle
        }
        if (scoreText != null)
            scoreText.text = "Score: 0";
        currentLanePosition = transform.position;
        currentLaneOffset = 0f;
        renderers = GetComponentsInChildren<Renderer>();
        playerCollider = GetComponent<Collider>();
        // Đã xóa dòng invisibleEffect.Play() test
    }

    void Update()
    {
        if (!isGameStarted) return;
        // Tăng baseSpeed dần đều
        if (baseSpeed < maxForwardSpeed)
        {
            baseSpeed += speedIncreaseRate * Time.deltaTime;
            if (baseSpeed > maxForwardSpeed)
                baseSpeed = maxForwardSpeed;
        }
        forwardSpeed = baseSpeed * speedMultiplier;
        HandleLaneSwitching();
        HandleJump();
        // Làm mượt chuyển làn trên trục X
        float targetLaneOffset = (targetLane - 1) * laneDistance;
        currentLaneOffset = Mathf.SmoothDamp(currentLaneOffset, targetLaneOffset, ref laneOffsetVelocity, laneSmoothTime);
        // Tính toán vector di chuyển tổng hợp cho cả X và Z
        Vector3 move = Vector3.zero;
        move.x = currentLaneOffset - transform.position.x;
        move.z = forwardSpeed * Time.deltaTime;
        move.y = 0; // Không thay đổi trục Y ở đây
        controller.Move(move);
        // Không đặt lại transform.position thủ công nữa
        // Xử lý queue chuyển làn liên tục
        if (Mathf.Abs(currentLaneOffset - targetLaneOffset) < 0.01f && laneChangeQueue.Count > 0)
        {
            int nextDir = laneChangeQueue.Dequeue();
            MoveLane(nextDir);
            if (player_Animation)
            {
                player_Animation.SetTrigger("is_sidejump");
            }
        }
    }

    private void HandleLaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!isChangingLane)
            {
                MoveLane(-1);
                if (player_Animation) player_Animation.SetTrigger("is_sidejump");
            }
            else
            {
                laneChangeQueue.Enqueue(-1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!isChangingLane)
            {
                MoveLane(1);
                if (player_Animation) player_Animation.SetTrigger("is_sidejump");
            }
            else
            {
                laneChangeQueue.Enqueue(1);
            }
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        // Trả về vị trí lane trên trục X, không thay đổi Z
        Vector3 pos = transform.position;
        pos.x = (targetLane - 1) * laneDistance; // 0=-1, 1=0, 2=1
        return pos;
    }

    private void MoveLane(int direction)
    {
        targetLane += direction;
        targetLane = Mathf.Clamp(targetLane, 0, 2);
    }

    private void HandleJump()
    {
        if (!isJumping && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
            if (player_Animation) player_Animation.SetTrigger("is_jump"); // Trigger animation running_jump
            StartCoroutine(JumpCoroutine());
        }
    }

    private System.Collections.IEnumerator JumpCoroutine()
    {
        float startY = transform.position.y;
        float velocity = jumpForce;
        // Sử dụng gravity từ biến thay vì gán cứng
        while (velocity > 0 || transform.position.y > startY)
        {
            float deltaY = velocity * Time.deltaTime;
            controller.Move(new Vector3(0, deltaY, 0));
            velocity += gravity * Time.deltaTime;
            yield return null;
        }
        // Đảm bảo nhân vật không bị tụt dưới mặt đất
        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
        isJumping = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isInvisible) return; // Nếu tàng hình thì bỏ qua va chạm
        if (hit.gameObject.CompareTag("Car"))
        {
            Vector3 normal = hit.normal.normalized;
            // Chỉ xử lý nếu va chạm phía trước hoặc hai bên xe, không phải từ trên xuống và không phải phía sau
            if ((normal.z < -0.5f || Mathf.Abs(normal.x) > 0.5f) && Mathf.Abs(normal.y) < 0.7f && normal.z < 0.5f)
            {
                if (restartPanel != null)
                    restartPanel.SetActive(true);
                if (player_Animation != null)
                    player_Animation.SetFloat("is_running", 0.0f); // Chuyển về idle
                isGameStarted = false; // Dừng toàn bộ gameplay
                // Dừng game nếu muốn: Time.timeScale = 0;
            }
            // Nếu va chạm phía sau xe hoặc từ trên xuống, không làm gì cả
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            score++;
            Destroy(other.gameObject);
            if (scoreText != null)
                scoreText.text = "Score: " + score;
            // TODO: cập nhật UI điểm số nếu có
        }
        else if (other.CompareTag("Thunder"))
        {
            StartCoroutine(SpeedBoost());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Time"))
        {
            StartCoroutine(SpeedSlow());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Invisible"))
        {
            StartCoroutine(InvisibleCoroutine());
            Destroy(other.gameObject);
        }
    }

    private System.Collections.IEnumerator SpeedBoost()
    {
        if (thunderEffect != null)
        {
            thunderEffect.Clear();
            thunderEffect.Play();
        }
        speedMultiplier = fastMultiplier;
        yield return new WaitForSeconds(fastDuration);
        speedMultiplier = 1f;
        if (thunderEffect != null) thunderEffect.Stop();
    }

    private System.Collections.IEnumerator SpeedSlow()
    {
        if (timeEffect != null)
        {
            timeEffect.Clear();
            timeEffect.Play();
        }
        speedMultiplier = slowMultiplier;
        yield return new WaitForSeconds(slowDuration);
        speedMultiplier = 1f;
        if (timeEffect != null) timeEffect.Stop();
    }

    private System.Collections.IEnumerator InvisibleCoroutine()
    {
        Debug.Log("[InvisibleCoroutine] Bắt đầu tàng hình");
        isInvisible = true;
        if (invisibleEffect != null)
        {
            invisibleEffect.Clear();
            invisibleEffect.Play();
        }
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Car"), true);
        yield return new WaitForSeconds(invisibleDuration);
        isInvisible = false;
        if (invisibleEffect != null) invisibleEffect.Stop();
        Debug.Log("[InvisibleCoroutine] Hết tàng hình");
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Car"), false);
    }

    public void PlayGame()
    {
        isGameStarted = true;
        if (playPanel != null)
            playPanel.SetActive(false);
        if (player_Animation != null)
        {
            player_Animation.SetFloat("is_running", 1.0f); // Set trạng thái chạy khi bắt đầu game
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}