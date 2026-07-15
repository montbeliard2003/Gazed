using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotebookUI : MonoBehaviour
{
    public static NotebookUI Instance { get; private set; }

    [Header("Main")]
    public GameObject notebookPanel;
    public Button notebookButton;
    public Button closeNotebookButton;

    [Header("Page Buttons")]
    public Button nextPageButton;
    public Button prevPageButton;

    [Header("Slots (4 per page)")]
    public Transform[] clueSlots;

    [Header("Prefabs")]
    public GameObject leftPhotoCardPrefab;
    public GameObject rightPhotoCardPrefab;

    private List<GameObject> _spawnedCards = new List<GameObject>();

    public bool IsOpen { get; private set; }

    // 分页系统
    private int currentPage = 0;
    private int cluesPerPage = 4;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (notebookButton != null)
            notebookButton.onClick.AddListener(ToggleNotebook);

        if (closeNotebookButton != null)
            closeNotebookButton.onClick.AddListener(CloseNotebook);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);

        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(PrevPage);

        // 初始关闭状态
        CloseNotebook();
    }

    void Update()
    {
        // 如果在开场阶段、结局阶段 或 游戏暂停阶段，直接跳过按键监听
        if (GameState.IsOpening || GameState.IsEnding || PauseMenuManager.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsOpen)
                CloseNotebook();
            else
                OpenNotebook();
        }
    }

    public void ToggleNotebook()
    {
        if (GameState.IsOpening || GameState.IsEnding || PauseMenuManager.IsPaused) return;

        if (IsOpen)
            CloseNotebook();
        else
            OpenNotebook();
    }

    public void OpenNotebook()
    {
        if (notebookPanel == null) return;

        notebookPanel.SetActive(true);
        IsOpen = true;

        currentPage = 0; // 每次打开默认回到第一页

        RefreshNotebook();

        // 打开记事本时释放鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNotebook()
    {
        if (notebookPanel == null) return;

        notebookPanel.SetActive(false);
        IsOpen = false;

        // 只有在：非剧情、非结局、且非暂停 状态下关闭记事本，才需要把鼠标锁回去
        if (!GameState.IsOpening && !GameState.IsEnding && !PauseMenuManager.IsPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void RefreshNotebook()
    {
        ClearOldCards();

        if (ClueJournal.Instance == null) return;

        var clues = ClueJournal.Instance.CollectedClues;

        int startIndex = currentPage * cluesPerPage;
        int endIndex = Mathf.Min(startIndex + cluesPerPage, clues.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            int slotIndex = i - startIndex;

            StoryData clue = clues[i];

            GameObject prefab = (slotIndex % 2 == 0)
                ? leftPhotoCardPrefab
                : rightPhotoCardPrefab;

            CreateCard(clue, clueSlots[slotIndex], prefab);
        }

        UpdatePageButtons();
    }

    void UpdatePageButtons()
    {
        if (ClueJournal.Instance == null) return;

        int totalClues = ClueJournal.Instance.CollectedClues.Count;
        int maxPage = Mathf.CeilToInt(totalClues / (float)cluesPerPage) - 1;

        if (prevPageButton != null)
            prevPageButton.interactable = currentPage > 0;

        if (nextPageButton != null)
            nextPageButton.interactable = currentPage < maxPage;
    }

    public void NextPage()
    {
        if (ClueJournal.Instance == null) return;

        int totalClues = ClueJournal.Instance.CollectedClues.Count;
        int maxPage = Mathf.CeilToInt(totalClues / (float)cluesPerPage) - 1;

        if (currentPage < maxPage)
        {
            currentPage++;
            RefreshNotebook();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            RefreshNotebook();
        }
    }

    void CreateCard(StoryData clue, Transform parent, GameObject prefab)
    {
        if (clue == null || parent == null || prefab == null) return;

        GameObject card = Instantiate(prefab, parent);
        _spawnedCards.Add(card);

        Transform photo = card.transform.Find("PhotoFrame/PhotoImage");
        Transform text = card.transform.Find("ClueText");

        if (photo != null)
        {
            Image img = photo.GetComponent<Image>();
            if (img != null)
                img.sprite = clue.photo;
        }

        if (text != null)
        {
            TMP_Text t = text.GetComponent<TMP_Text>();
            if (t != null)
                t.text = clue.notebookText;
        }
    }

    void ClearOldCards()
    {
        foreach (var c in _spawnedCards)
        {
            if (c != null)
                Destroy(c);
        }

        _spawnedCards.Clear();
    }
}