using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    // 使用静态变量方便其他脚本（如 NotebookUI）检查状态
    public static bool IsPaused = false;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;      // 暂停菜单总面板
    public GameObject controlsSubPanel; // 控制说明子面板
    public GameObject menuContent;      // 包含四个按钮的面板

    void Awake()
    {
        // --- 核心修复：确保游戏启动时处于正确状态 ---
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (controlsSubPanel != null)
        {
            controlsSubPanel.SetActive(false);
        }

        if (menuContent != null)
        {
            menuContent.SetActive(true); // 确保一开始显示按钮面板
        }

        IsPaused = false;
        Time.timeScale = 1.0f; // 确保场景重载后时间是流动的
    }

    void Update()
    {
        // 只有在【非开场剧情】且【非结局动画】状态下，才允许响应 Esc 键
        if (GameState.IsOpening || GameState.IsEnding)
        {
            // 如果在剧情中由于某种原因面板开了，强制关掉它
            if (IsPaused) Resume();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (controlsSubPanel != null) controlsSubPanel.SetActive(false);
        if (menuContent != null) menuContent.SetActive(true); // 恢复时显示主面板

        Time.timeScale = 1f;               // 恢复游戏时间
        IsPaused = false;

        // 只有在记事本也没打开的情况下，才把鼠标锁回去
        if (NotebookUI.Instance != null && !NotebookUI.Instance.IsOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (NotebookUI.Instance == null)
        {
            // 如果没有记事本系统，直接锁定鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Pause()
    {
        // --- 联动修改：如果打开暂停菜单时记事本开着，先关掉记事本 ---
        if (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen)
        {
            NotebookUI.Instance.CloseNotebook();
        }

        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (menuContent != null) menuContent.SetActive(true); // 唤出暂停菜单时显示主面板
        if (controlsSubPanel != null) controlsSubPanel.SetActive(false); // 确保控制面板默认关闭

        Time.timeScale = 0f;               // 彻底冻结游戏逻辑
        IsPaused = true;

        // 释放鼠标以便操作菜单按钮
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleControls()
    {
        if (controlsSubPanel != null)
        {
            bool showControls = !controlsSubPanel.activeSelf;

            // 1. 切换控制说明面板
            controlsSubPanel.SetActive(showControls);

            // --- 核心修复开始 ---
            if (showControls)
            {
                // 在面板开启时，强制寻找并开启名为 "Hint" 的子物体
                Transform hintTransform = controlsSubPanel.transform.Find("Hint");
                if (hintTransform != null)
                {
                    hintTransform.gameObject.SetActive(true);
                }
            }
            // --- 核心修复结束 ---

            // 2. 切换按钮面板
            if (menuContent != null)
            {
                menuContent.SetActive(!showControls);
            }
        }
    }

    public void GoToMainMenu()
    {
        // --- 核心修复：返回主菜单前必须重置静态变量和时间 ---
        Time.timeScale = 1f;
        IsPaused = false;
        GameState.IsOpening = true; // 让下次进入游戏时重新进入开场逻辑
        GameState.IsEnding = false;

        // 重新加载当前场景回到初始状态
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}