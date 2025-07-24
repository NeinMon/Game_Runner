using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
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
    private int targetLane = 1;
    private CharacterController controller;
    [Header("Animation")]
    [SerializeField] private Animator player_Animation;
    private Queue<int> laneChangeQueue = new Queue<int>();
    private bool isChangingLane = false;
    private bool isJumping = false;
    [SerializeField] private float jumpForce = 2.2f;
    [SerializeField] private float gravity = -5f;
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject wholeUIPanel;
    [SerializeField] private GameObject pausePanel;
    private PopUpEffect popUp;
    private bool isGameStarted = false;
    private int score = 0;
    private int dis_long = 0;
    private int[] scoreRequire;
    private Vector3 startPos;
    private float originalSpeed;
    [SerializeField] private float fastDuration = 3f;
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private float fastMultiplier = 2f;
    [SerializeField] private float slowMultiplier = 0.5f;
    private Text scoreText;
    private Text disText;
    [SerializeField] private float minForwardSpeed = 1f;
    [SerializeField] private float maxForwardSpeed = 4f;
    [SerializeField] private float speedIncreaseRate = 0.05f;
    private float baseSpeed;
    private float speedMultiplier = 1f;
    private Vector3 currentLanePosition;
    private float currentLaneOffset = 0f;
    private float laneOffsetVelocity = 0f;
    [SerializeField] private float laneSmoothTime = 0.15f;
    private Vector3 laneVelocity = Vector3.zero;
    private Vector3 forwardMove = Vector3.zero;
    private Vector3 laneMove = Vector3.zero;
    [SerializeField] private float invisibleDuration = 4f;
    private bool isInvisible = false;
    private Renderer[] renderers;
    private Collider playerCollider;
    [SerializeField] private ParticleSystem invisibleEffect;
    [SerializeField] private ParticleSystem thunderEffect;
    [SerializeField] private ParticleSystem timeEffect;
    [SerializeField] private ParticleSystem freezeEffect;

    private bool isFreezing = false;


    private bool completed;
    private string uid;
    private string displayName;
    private int scene_num;



    void Start()
    {
        // scoreRequire = new int[] { 100, 150, 200, 100, 150, 200 };
        scoreRequire = new int[] { 10, 15, 20, 10, 15, 20 };
        startPos = transform.position;
        controller = GetComponent<CharacterController>();
        baseSpeed = minForwardSpeed;
        forwardSpeed = minForwardSpeed;
        originalSpeed = minForwardSpeed;
        if (restartPanel != null)
            restartPanel.SetActive(false);
        if (playPanel != null)
            playPanel.SetActive(true);
        isGameStarted = false;

        if (player_Animation != null)
        {
            player_Animation.SetFloat("is_running", 0.0f);
        }
        if (scoreText == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("ScoreText");
            if (obj != null) scoreText = obj.GetComponent<Text>();
            Debug.Log("Đã có score text");
        }

        if (disText == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("DisText");
            if (obj != null) disText = obj.GetComponent<Text>();
        }


        if (scoreText != null)
            scoreText.text = "0";
        if (disText != null)
            disText.text = "0 m";

        currentLanePosition = transform.position;
        currentLaneOffset = 0f;
        renderers = GetComponentsInChildren<Renderer>();
        playerCollider = GetComponent<Collider>();

        scene_num = GetSceneNumber();
        var authService = AuthService.Instance;
        if (authService == null)
        {
            Debug.LogWarning("authService is null (chưa đăng nhập?)");
            return;
        }
        FirebaseUser user = authService.GetUser();
        if (user == null)
        {
            Debug.LogWarning("User is null (chưa đăng nhập?)");
            return;
        }
        uid = user?.UserId;
        displayName = user?.DisplayName;
        Debug.Log(uid.ToString());

        DaoService.Instance.GetCompletedStatusOfUserInMap(GetSceneNumber(), uid, latest_completed_status =>
        {
            completed = latest_completed_status;
        });
        PopUpEffect[] allObjects = Resources.FindObjectsOfTypeAll<PopUpEffect>();
        foreach (PopUpEffect obj in allObjects)
        {
            if (obj.CompareTag("UnlockNewMap"))
            {
                popUp = obj;
                break;
            }
        }

    }

    void Update()
    {
        dis_long = (int)Vector3.Distance(startPos, transform.position);
        if (disText != null)
            disText.text = dis_long + "m";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isGameStarted) return;

            if (Time.timeScale > 0f)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
            return;
        }
        if (!isGameStarted) return;
        if (baseSpeed < maxForwardSpeed)
        {
            baseSpeed += speedIncreaseRate * Time.deltaTime;
            if (baseSpeed > maxForwardSpeed)
                baseSpeed = maxForwardSpeed;
        }
        forwardSpeed = baseSpeed * speedMultiplier;
        HandleLaneSwitching();
        HandleJump();
        float targetLaneOffset = (targetLane - 1) * laneDistance;
        currentLaneOffset = Mathf.SmoothDamp(currentLaneOffset, targetLaneOffset, ref laneOffsetVelocity, laneSmoothTime);
        Vector3 move = Vector3.zero;
        move.x = currentLaneOffset - transform.position.x;
        move.z = forwardSpeed * Time.deltaTime;
        move.y = 0;
        controller.Move(move);
        if (Mathf.Abs(currentLaneOffset - targetLaneOffset) < 0.01f && laneChangeQueue.Count > 0)
        {
            int nextDir = laneChangeQueue.Dequeue();
            MoveLane(nextDir);
            if (player_Animation)
            {
                player_Animation.SetTrigger("is_sidejump");
            }
        }

        if (completed == false && (score >= scoreRequire[scene_num - 1]))
        {
            completed = true;
            popUp.StartFade();
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

        Vector3 pos = transform.position;
        pos.x = (targetLane - 1) * laneDistance;
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
            if (player_Animation) player_Animation.SetTrigger("is_jump");
            StartCoroutine(JumpCoroutine());
        }
    }

    private System.Collections.IEnumerator JumpCoroutine()
    {
        float startY = transform.position.y;
        float velocity = jumpForce;

        while (velocity > 0 || transform.position.y > startY)
        {
            float deltaY = velocity * Time.deltaTime;
            controller.Move(new Vector3(0, deltaY, 0));
            velocity += gravity * Time.deltaTime;
            yield return null;
        }

        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
        isJumping = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isInvisible) return;
        if (hit.gameObject.CompareTag("Car") || hit.gameObject.CompareTag("HighObstacle"))
        {
            Vector3 normal = hit.normal.normalized;
            if ((normal.z < -0.5f || Mathf.Abs(normal.x) > 0.5f) && Mathf.Abs(normal.y) < 0.7f && normal.z < 0.5f)
            {
                if (restartPanel != null)
                {
                    CloseAllPanels();
                    restartPanel.SetActive(true);
                }
                if (player_Animation != null)
                    player_Animation.SetFloat("is_running", 0.0f);
                isGameStarted = false;
                SoundManager.Instance.PlayCollideSFX();
                HandleSaveProgressAndScore();
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            score++;
            Destroy(other.gameObject);
            if (scoreText != null)
                scoreText.text = "" + score;
            SoundManager.Instance.PlayCoinSFX();
        }
        else if (other.CompareTag("Thunder"))
        {
            StartCoroutine(SpeedBoost());
            Destroy(other.gameObject);
            SoundManager.Instance.PlayPowerUpSFX();
        }
        else if (other.CompareTag("Time"))
        {
            StartCoroutine(SpeedSlow());
            Destroy(other.gameObject);
            SoundManager.Instance.PlayPowerUpSFX();
        }
        else if (other.CompareTag("Invisible"))
        {
            StartCoroutine(InvisibleCoroutine());
            Destroy(other.gameObject);
            SoundManager.Instance.PlayPowerUpSFX();
        }
        else if (other.CompareTag("FreezeCircle"))
        {
            if (!isFreezing)
            {
                isFreezing = true;
                if (freezeEffect != null)
                {
                    freezeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    freezeEffect.Play();
                    StartCoroutine(StopFreezeEffectAfterDuration(2f));
                }
                SoundManager.Instance.PlayFreezeSFX();
                StartCoroutine(FreezeAndRestartCoroutine());
            }
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
        isInvisible = true;
        if (invisibleEffect != null)
        {
            invisibleEffect.Clear();
            invisibleEffect.Play();
        }
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Car"), true);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), true);
        yield return new WaitForSeconds(invisibleDuration);
        isInvisible = false;
        if (invisibleEffect != null) invisibleEffect.Stop();
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Car"), false);
        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), false);
    }

    private System.Collections.IEnumerator StopFreezeEffectAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (freezeEffect != null)
            freezeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private System.Collections.IEnumerator FreezeAndRestartCoroutine()
    {
        isGameStarted = false;
        if (player_Animation != null)
            player_Animation.SetFloat("is_running", 0.0f);
        yield return new WaitForSeconds(2f);
        if (restartPanel != null)
            restartPanel.SetActive(true);
        isFreezing = false;
        HandleSaveProgressAndScore();
    }



    public void PlayGame()
    {
        isGameStarted = true;
        CloseAllPanels();
        wholeUIPanel.SetActive(true);
        if (player_Animation != null)
        {
            player_Animation.SetFloat("is_running", 1.0f);
        }
    }

    public void RestartGame()
    {
        CloseAllPanels();
        playPanel.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        CloseAllPanels();
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        if (!isGameStarted) return;
        Time.timeScale = 0f;
        CloseAllPanels();
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        CloseAllPanels();
        wholeUIPanel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        wholeUIPanel.SetActive(false);
        pausePanel.SetActive(false);
        restartPanel.SetActive(false);
        playPanel.SetActive(false);
    }


    private int GetSceneNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("map"))
        {
            string mapNumberStr = sceneName.Substring(3);
            int mapNumber = int.Parse(mapNumberStr);
            return mapNumber;
        }
        return 0;
    }

    private void HandleSaveProgressAndScore()
    {

        if (uid == null) return;
        Debug.Log("CURRENT COMPLETED STATUS: " + completed.ToString());
        DaoService.Instance.SaveProgressForUser(scene_num, uid, displayName, dis_long, completed);
        DaoService.Instance.UpdateLatestScoreOfAUser(uid, score);
        if (completed == true)
        {
            DaoService.Instance.AddCompletedMapToProgress(uid, scene_num);
        }
        HandleSetGameOverDataPanel();
    }

    private void HandleSetGameOverDataPanel()
    {
        Text currentDistanceText = GameObject.FindGameObjectWithTag("CurrentDistance").GetComponent<Text>();
        currentDistanceText.text = dis_long.ToString() + " m";
        DaoService.Instance.GetHighestDistanceOfUser(scene_num, uid, highestDistance =>
        {
            Text highestDistanceText = GameObject.FindGameObjectWithTag("HighestDistance").GetComponent<Text>();
            highestDistanceText.text = highestDistance.ToString() + " m";
        });
    }

}