using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    public FirstPersonMovement movement; // 拖 Player 上的 FirstPersonMovement

    [Header("Bob Settings")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;

    public float sprintBobSpeed = 14f;
    public float sprintBobAmount = 0.08f;

    [Header("Smoothing")]
    public float returnSpeed = 10f;

    private Vector3 _startLocalPos;
    private float _timer;

    void Start()
    {
        _startLocalPos = transform.localPosition;

        if (movement == null)
            movement = FindFirstObjectByType<FirstPersonMovement>();
    }

    void Update()
    {
        if ((StoryUI.Instance != null && StoryUI.Instance.IsOpen) || (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen))
        {
            ResetToStart();
            return;
        }

        if (movement == null)
        {
            ResetToStart();
            return;
        }

        // 不移动：平滑回到初始位置
        if (!movement.IsMoving)
        {
            ResetToStart();
            return;
        }

        // 移动中：根据走路/跑步使用不同的晃动参数
        float bobSpeed = movement.IsSprinting ? sprintBobSpeed : walkBobSpeed;
        float bobAmount = movement.IsSprinting ? sprintBobAmount : walkBobAmount;

        _timer += Time.deltaTime * bobSpeed;

        float offsetY = Mathf.Sin(_timer) * bobAmount;
        float offsetX = Mathf.Cos(_timer * 0.5f) * bobAmount * 0.5f; // 轻微左右

        Vector3 target = _startLocalPos + new Vector3(offsetX, offsetY, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * returnSpeed);
    }

    private void ResetToStart()
    {
        _timer = 0f;
        transform.localPosition = Vector3.Lerp(transform.localPosition, _startLocalPos, Time.deltaTime * returnSpeed);
    }
}