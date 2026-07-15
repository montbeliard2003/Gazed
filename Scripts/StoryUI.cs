using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; // --- 必须新增这个引用 ---

public class StoryUI : MonoBehaviour
{
    public static StoryUI Instance { get; private set; }

    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button closeButton;

    public bool IsOpen { get; private set; }

    string[] _paragraphs;
    int _index;

    private void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
        Hide();
    }

    public void ShowCard(StoryData card)
    {
        if (card == null) return;

        panel.SetActive(true);
        titleText.text = card.title;
        _paragraphs = card.body.Split(new string[] { "\n\n", "\r\n\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        _index = 0;
        bodyText.text = _paragraphs[_index];

        IsOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!IsOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            // 1. 获取当前鼠标悬停的 UI 物体
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            // 2. 只有当点击的是 closeButton 本身时，才拦截翻页
            // 我们通过比较 gameObject 来实现精确拦截
            if (currentSelected != null && currentSelected == closeButton.gameObject)
            {
                return; // 拦截，不翻页
            }

            // 如果点击的是面板其他地方或者空白处，正常翻页
            NextParagraph();
        }
    }

    void NextParagraph()
    {
        _index++;

        if (_paragraphs != null && _index < _paragraphs.Length)
        {
            bodyText.text = _paragraphs[_index];
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        IsOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}