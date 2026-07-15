using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 5f;
    public float sensitivityY = 4f;

    float xRotation = 0f;

    void Start()
    {
        // 只有在非剧情阶段才锁定鼠标
        if (!GameState.IsOpening && !GameState.IsEnding)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // --- 【核心修改】检查所有需要拦截镜头旋转的状态 ---
        // 1. 剧情或结局状态 (GameState)
        // 2. 暂停菜单打开状态 (PauseMenuManager)
        // 3. 剧情文本 UI 或 记事本 UI 打开状态
        if (GameState.IsOpening || GameState.IsEnding || PauseMenuManager.IsPaused)
            return;

        if ((StoryUI.Instance != null && StoryUI.Instance.IsOpen) ||
            (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen))
        {
            return;
        }

        // --- 以下逻辑保持不变 ---

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // 1. 处理摄像机（头）的上下旋转
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. 处理父物体（身体）的左右转身
        if (transform.parent != null)
        {
            transform.parent.Rotate(Vector3.up * mouseX);

            // 修正父物体（Player）位置
            Vector3 parentRot = transform.parent.localEulerAngles;
            transform.parent.localRotation = Quaternion.Euler(0f, parentRot.y, 0f);
        }
    }
}