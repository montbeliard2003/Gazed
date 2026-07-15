using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoIntroManager : MonoBehaviour
{
    [Header("Room Setup")]
    public GameObject roomParent;

    [Header("UI - Controls")]
    public GameObject videoMenuPanel;
    public Button startButton;
    public GameObject hintUI;
    public GameObject gameSystems; // 建议手动拖入挂载了 NotebookUI 的那个父物体

    [Header("Crosshair Setup")]
    public GameObject crosshair;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip introClip;

    [Header("Next")]
    public OpeningManager openingManager;

    void Awake()
    {
        // --- 【新增：核心重置】 ---
        Time.timeScale = 1.0f;          // 确保时间流逝正常
        GameState.IsOpening = true;    // 确保在一开始就封印玩家操作
        GameState.IsEnding = false;

        // 1. 关闭房间
        if (roomParent != null) roomParent.SetActive(false);

        // 2. 彻底关闭 UI 干扰项
        if (crosshair != null)
        {
            crosshair.SetActive(false);
            if (crosshair.transform.parent != null)
                crosshair.transform.parent.gameObject.SetActive(false);
        }

        // 3. 强制关闭 Hint
        if (hintUI != null) hintUI.SetActive(false);

        // 4. 彻底禁用记事本系统，防止第二次游戏残留显示
        if (gameSystems != null)
        {
            gameSystems.SetActive(false);
        }
        else
        {
            // 如果没拖引用，尝试搜索
            GameObject notebook = GameObject.Find("GameSystems");
            if (notebook != null) notebook.SetActive(false);
        }

        if (videoMenuPanel != null) videoMenuPanel.SetActive(true);
        if (openingManager != null) openingManager.gameObject.SetActive(false);
    }

    void Start()
    {
        // 初始化鼠标，确保菜单阶段可以点击按钮
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (videoPlayer != null)
        {
            videoPlayer.clip = introClip;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClicked);
        }
    }

    void OnStartClicked()
    {
        if (videoPlayer != null) videoPlayer.Stop();

        if (videoMenuPanel != null) videoMenuPanel.SetActive(false);

        if (openingManager != null)
        {
            openingManager.gameObject.SetActive(true);
            openingManager.BeginOpening();
        }

        this.enabled = false;
    }
}