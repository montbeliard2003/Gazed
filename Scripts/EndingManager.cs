using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup blackOverlay;
    public TMP_Text endingText;

    [Header("Gameplay UI")]
    public GameObject hintUI;
    public GazeFeedbackUI gazeUI;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float typeSpeed = 0.05f;
    public float lineDelay = 0.5f;

    private bool _started = false;
    private bool _waitForClick = false;

    void Update()
    {
        if (_waitForClick && Input.GetMouseButtonDown(0))
        {
            _waitForClick = false;
        }
    }

    public void BeginEnding()
    {
        if (_started) return;

        if (blackOverlay == null || endingText == null)
        {
            Debug.LogError("EndingManager: UI 组件未绑定。");
            return;
        }

        _started = true;

        // --- 【新增：停止环境音逻辑】 ---
        if (AmbienceManager.Instance != null)
        {
            AmbienceManager.Instance.StopAmbience();
        }
        // ------------------------------

        // --- 【核心修改：结局封印逻辑】 ---

        // 1. 重新标记为 IsEnding，这会让 NotebookUI 的 Update 再次被拦截
        GameState.IsEnding = true;

        // 2. 彻底关闭记事本系统
        if (NotebookUI.Instance != null)
        {
            // 如果记事本开着，先执行它的关闭逻辑（重置状态和鼠标）
            if (NotebookUI.Instance.IsOpen)
            {
                NotebookUI.Instance.CloseNotebook();
            }
            // 彻底禁用物体
            NotebookUI.Instance.gameObject.SetActive(false);
        }

        // 3. 隐藏游戏内的功能性 UI
        if (hintUI != null) hintUI.SetActive(false);
        if (gazeUI != null) gazeUI.gameObject.SetActive(false);

        // 4. 为结局文本准备鼠标：解锁并可见，以便玩家点击翻页
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ----------------------------------

        StartCoroutine(EndingSequence());
    }

    IEnumerator EndingSequence()
    {
        // 初始化文本状态
        endingText.text = "";
        SetTextAlpha(0f);

        // 1. 彻底黑屏
        yield return FadeOverlay(0f, 1f, fadeDuration);

        // 2. 音乐淡入
        GameObject bgmObj = GameObject.FindWithTag("BGM");
        if (bgmObj != null)
        {
            AudioSource bgmSource = bgmObj.GetComponent<AudioSource>();
            if (bgmSource != null)
            {
                bgmSource.Play();
                StartCoroutine(FadeInAudio(bgmSource, 3f));
            }
        }

        // 3. 依次显示每一页故事 (打字机模式)
        yield return ShowPageOnClick(new string[]

        {
            "[The Ending]"

          });



        yield return ShowPageOnClick(new string[]

        {

            "You stand in the center of the room. You have seen it all.",

            "Except for one thing.",

            "You looked at it when you first entered. " ,

            "You gazed at the heap of objects. " ,

            "You saw him on that night. Sitting on the floor. Watching the light in the crack of the door. Pressing his ear against the wood. And then moving things, piece by piece. To block it.",

            "But you have never looked—",

            "Behind the door."

        });



        yield return ShowPageOnClick(new string[]

        {

            "[You walk toward the door]",

            "You stand before the pile of things.",

            "Exactly where he stood that night.",

            "There is light in the crack of the door. Very thin. Like a thread. Piercing through from the other side.",

            "You reach out. You touch the first object. You begin to move it. Piece by piece. Just as he moved them in. Now, you move them away.",

            "The last object is cleared.",

            "The door is revealed.",

            "The handle is iron.",

            "Old. Rusted. But unlocked."

        });



        yield return ShowPageOnClick(new string[]

        {

            "You stand there. Gripping the handle. You do not turn it yet.",

            "You think of him. That night. He stood here, too. He reached out. His hand almost touched the handle.",

            "He pulled back. He reached out again. He pulled back again.",

            "And then he began to move things. Not moving them away. But moving them in. One by one. To block it.",

            "You look at the light in the crack. The light is still there. Just as it was that night."

        });



        yield return ShowPageOnClick(new string[]

        {

            "[You turn the handle]",

            "[You push the door open]",

            "On the other side—",

            "It is not another room. It is not a hallway. It is not the outside world.",

            "It is... This room.",

            "The same layout. The same window. The same bed. The same sofa.",

            "The same small table by the door.",

            "But it is different."

        });



        yield return ShowPageOnClick(new string[]

        {

            "There is no duffel bag. There is no trash bag. There are no objects blocking the door.",

            "There is a person.",

            "You know who he is. You have seen him. In the notepad. In the trash bag. In the ashtray. In the newspaper rack. In the portrait. In the speaker. In the duffel bag.",

            "You have seen him countless times. In all the traces he left behind.",

            "He stands there. Watching you.",

            "“You’ve come.”"

        });



        yield return ShowPageOnClick(new string[]

        {

            "You are on this side of the door. Looking at yourself on the other side.",

            "The self on the other side. Looking at the you on this side.",

            "Two doors. Two rooms. Two people. Staring at each other."

        });



        yield return ShowPageOnClick(new string[]

        {

            "You look up. Looking at the other side. It is empty. He is no longer there.",

            "But you see—",

            "In the room over there. By the door you just walked through. A person is standing at the threshold.",

            "That person is also watching you. It is yourself. But then again, it is not yourself."

        });



        yield return ShowPageOnClick(new string[]

        {

            "Because in that person's hand—",

            "Is also a slip of paper.",

            "“Everything within your sight—”",

            "“In the places you cannot see—”",

            "“Is watching you.”"

        });



        // 4. 所有内容播放完毕，淡出并重启
        yield return FadeTextOut(1f);
        yield return new WaitForSeconds(1f);
        ReturnToMainMenu();
    }

    // --- 打字机显示逻辑 ---
    IEnumerator ShowPageOnClick(string[] lines)
    {
        endingText.text = "";
        SetTextAlpha(1f);

        for (int i = 0; i < lines.Length; i++)
        {
            string currentLine = lines[i];
            string alreadyTyped = endingText.text;

            if (alreadyTyped != "")
            {
                alreadyTyped += "\n\n";
            }

            for (int j = 0; j <= currentLine.Length; j++)
            {
                endingText.text = alreadyTyped + currentLine.Substring(0, j);
                yield return new WaitForSeconds(typeSpeed);
            }

            yield return new WaitForSeconds(lineDelay);
        }

        _waitForClick = true;
        while (_waitForClick)
        {
            yield return null;
        }

        yield return FadeTextOut(0.5f);
        endingText.text = "";
    }

    void ReturnToMainMenu()
    {
        // 在离开场景前，必须把静态闸门打开，否则下次进来默认就是锁死的
        GameState.IsEnding = false;

        // 游戏结束重启
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 音频处理 ---
    IEnumerator FadeInAudio(AudioSource source, float duration)
    {
        float targetVolume = 0.2f;
        source.volume = 0f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    // --- UI 辅助方法 ---
    IEnumerator FadeOverlay(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            blackOverlay.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        blackOverlay.alpha = to;
    }

    IEnumerator FadeTextOut(float duration)
    {
        float t = 0f;
        Color c = endingText.color;
        float startAlpha = c.a;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, 0f, t / duration);
            endingText.color = c;
            yield return null;
        }
        c.a = 0f;
        endingText.color = c;
    }

    void SetTextAlpha(float alpha)
    {
        Color c = endingText.color;
        c.a = alpha;
        endingText.color = c;
    }
}