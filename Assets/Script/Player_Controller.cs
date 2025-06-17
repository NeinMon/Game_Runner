using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private float sideSpeed = 15f;
    private int targetLane = 1; // 0 = Left, 1 = Mid, 2 = Right
    private CharacterController controller;
    [Header("Animation")]
    [SerializeField] private Animator player_Animation; // Kéo thả Animator vào Inspector
    private Queue<int> laneChangeQueue = new Queue<int>();
    private bool isChangingLane = false;
    private bool isJumping = false;
    [SerializeField] private float jumpForce = 7f; // Lực nhảy

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Đảm bảo animation run được kích hoạt khi bắt đầu game
        if (player_Animation != null)
        {
            player_Animation.SetFloat("is_running", 1.0f);
        }
    }

    void Update()
    {
        HandleForwardMovement();
        HandleLaneSwitching();
        MoveTowardsTargetLane();
        HandleJump();
    }

    private void HandleForwardMovement()
    {
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.deltaTime;
        Vector3 worldForwardMove = transform.TransformDirection(forwardMove);
        controller.Move(worldForwardMove);
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

    private void MoveTowardsTargetLane()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        Vector3 diff = targetPosition - transform.position;
        diff.y = 0; // Only move horizontally
        if (diff.sqrMagnitude > 0.001f)
        {
            isChangingLane = true;
            Vector3 moveDir = diff.normalized * sideSpeed * Time.deltaTime;
            if (moveDir.sqrMagnitude < diff.sqrMagnitude)
                controller.Move(moveDir);
            else
                controller.Move(diff);
        }
        else if (isChangingLane)
        {
            isChangingLane = false;
            // Xử lý queue chuyển làn liên tục
            if (laneChangeQueue.Count > 0)
            {
                int nextDir = laneChangeQueue.Dequeue();
                MoveLane(nextDir);
                if (player_Animation)
                {
                    player_Animation.SetTrigger("is_sidejump");
                }
            }
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (targetLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (targetLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }
        return targetPosition;
    }

    private void MoveLane(int direction)
    {
        targetLane += direction;
        targetLane = Mathf.Clamp(targetLane, 0, 2);
    }

    private void HandleJump()
    {
        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
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
        float gravity = -20f;
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
}