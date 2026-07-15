using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.0f;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;

    // 给 HeadBob 用：当前是否在移动
    public bool IsMoving { get; private set; }
    // 给 HeadBob 用：当前是否在冲刺
    public bool IsSprinting { get; private set; }

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (GameState.IsOpening || GameState.IsEnding)
        {
            IsMoving = false;
            IsSprinting = false;
            _velocity = Vector3.zero;
            return;
        }

        // 故事卡或笔记本打开时：禁止移动
        if ((StoryUI.Instance != null && StoryUI.Instance.IsOpen) || (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen))
        {
            IsMoving = false;
            IsSprinting = false;
            _velocity = Vector3.zero;
            return;
        }

        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * x + transform.forward * z;

        // 是否在移动（有输入）
        IsMoving = move.sqrMagnitude > 0.001f;

        // 是否冲刺（按住 Shift 且正在前进/移动）
        IsSprinting = IsMoving && Input.GetKey(sprintKey);

        float speed = IsSprinting ? sprintSpeed : walkSpeed;

        _controller.Move(move * speed * Time.deltaTime);

        // 重力
        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = -2f; // 贴地

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}