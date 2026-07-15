using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class OpeningManager : MonoBehaviour
{
    [Header("Room Setup")]
    public GameObject roomParent;

    [Header("UI - Core")]
    public CanvasGroup blackOverlay;
    public GazeFeedbackUI gazeUI;
    public GameObject hintUI;
    public GameObject controlsPopupPanel;
    public TMP_Text openingText;

    [Header("UI - Credit Section")]
    public CanvasGroup creditPanel;
    public TMP_Text creditText;

    [Header("Audio")]
    public AudioSource typeSoundSource;

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    private bool _hasStarted = false;
    private bool _waitForPlayerClick = false;
    private bool _waitForPopup = false;

    void Start()
    {
        if (blackOverlay != null) blackOverlay.alpha = 1f;
        if (openingText != null) openingText.text = "";
        if (controlsPopupPanel != null) controlsPopupPanel.SetActive(false);
    }

    void Update()
    {
        if (_waitForPlayerClick && Input.GetMouseButtonDown(0))
        {
            _waitForPlayerClick = false;
        }
    }

    public void BeginOpening()
    {
        if (_hasStarted) return;
        _hasStarted = true;

        gameObject.SetActive(true);
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);
            blackOverlay.alpha = 1f;
        }

        GameState.IsOpening = true;

        if (gazeUI != null) gazeUI.gameObject.SetActive(false);
        if (hintUI != null) hintUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StopAllCoroutines();
        StartCoroutine(OpeningSequence());
    }

    IEnumerator OpeningSequence()
    {
        // 1. 制作人名单
        if (creditPanel != null && creditText != null)
        {
            creditPanel.gameObject.SetActive(true);
            creditPanel.alpha = 1f;
            creditText.text = "";
            yield return new WaitForSeconds(1.0f);
            yield return Typewriter(creditText, "MADE BY MONTY AND JOYA");
            yield return new WaitForSeconds(2.5f);
            yield return FadeCanvas(creditPanel, 1f, 0f, 2.0f);
            creditText.text = "";
            creditPanel.gameObject.SetActive(false);
        }

        if (openingText != null) openingText.text = "";
        yield return new WaitForSeconds(0.5f);

        // 2. 睁眼效果 (此时稍微看清一点点，增加氛围)
        yield return FadeOverlay(1f, 0.6f, 3.0f);

        // 3. 剧情文本段落
        yield return Typewriter(openingText, "You open your eyes.\n\nNo—you never closed your eyes. You were just... standing here.\n\nYou remember being here.\n\nBut you don't remember why you're only here now.");

        yield return WaitForKeyDown();



        yield return Typewriter(openingText, "You stand at the threshold.\n\nThe room is silent.\n\nA silence so deep, it feels as though time itself has come to a standstill.\n\nLight filters through the window, resting upon the objects within.\n\nThey are all here.\n\nOne by one, as if waiting for you.");

        yield return WaitForKeyDown();



        openingText.text = "";

        yield return AddLineWithTypewriter("He is not here.");

        yield return WaitForKeyDown();

        yield return AddLineWithTypewriter("But these things remain.");

        yield return WaitForKeyDown();

        yield return AddLineWithTypewriter("They knew him.\nThey knew him long before you did.");

        yield return AddLineWithTypewriter("And now, they are looking at you.");

        yield return WaitForKeyDown();



        openingText.text = "";

        yield return Typewriter(openingText, "[Visual Guide]\nCenter your reticle on an object and hold for 3 seconds.\nIt will tell you what it has witnessed.\n\n[System Prompt]\nThere are eight items here.\nYou may only gaze upon them once.");

        yield return WaitForKeyDown();



        openingText.text = "";

        yield return AddLineWithTypewriter("Did you come for him?");

        yield return WaitForKeyDown();

        yield return AddLineWithTypewriter("Or did you come for these things?");

        yield return WaitForKeyDown();



        openingText.text = "";

        yield return Typewriter(openingText, "[The game begins]\n\n[Free movement enabled. Choose your first gaze.]");

        yield return WaitForKeyDown();


        // 4. 背景音乐处理
        GameObject bgmObj = GameObject.FindWithTag("BGM");
        if (bgmObj != null)
        {
            AudioSource bgmSource = bgmObj.GetComponent<AudioSource>();
            if (bgmSource != null) yield return FadeOutAudio(bgmSource, 2f);
        }

        // 文本彻底消失
        if (openingText != null) openingText.text = "";

        // 5. 调用新的启动流程
        StartCoroutine(StartGameSequence());
    }

    // 新的启动流程：画面变亮 -> 弹窗 -> 正式开始
    IEnumerator StartGameSequence()
    {
        // 1. 开启房间
        if (roomParent != null) roomParent.SetActive(true);

        // 2. 画面由暗完全变亮 (睁眼看清房间)
        if (blackOverlay != null)
            yield return FadeOverlay(blackOverlay.alpha, 0f, 2.0f);

        // 3. 画面亮起后，弹出控制说明
        if (controlsPopupPanel != null)
        {
            controlsPopupPanel.SetActive(true);
            _waitForPopup = true;

            // 确保鼠标显示，方便点击
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 等待玩家点击关闭按钮
            while (_waitForPopup)
            {
                yield return null;
            }

            // 关闭弹窗
            controlsPopupPanel.SetActive(false);
        }

        // 4. 弹窗关闭后，执行正式开始的收尾逻辑
        FinalizeStartGame();
    }

    public void CloseControlsPopup()
    {
        _waitForPopup = false;
    }

    void FinalizeStartGame()
    {
        // 解锁记事本等状态
        GameState.IsOpening = false;

        if (NotebookUI.Instance != null)
        {
            NotebookUI.Instance.gameObject.SetActive(true);
        }

        // 恢复准心
        if (gazeUI != null)
        {
            gazeUI.gameObject.SetActive(true);
            Transform crossTransform = gazeUI.transform.Find("Crosshair");
            if (crossTransform != null) crossTransform.gameObject.SetActive(true);
        }

        // --- 核心修改：在准心亮起、玩家可以动、正式进入扫描环节时，启动环境音 ---
        if (AmbienceManager.Instance != null)
        {
            AmbienceManager.Instance.StartAmbience();
        }
        // -----------------------------------------------------------------

        // 延迟显示左上角操作提示
        if (hintUI != null)
        {
            hintUI.SetActive(false);
            StartCoroutine(ShowHintWithDelay(1.0f));
        }

        // 锁定鼠标并隐藏，进入第一人称控制
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 彻底移除过场物体
        Invoke("DisableAll", 0.5f);
    }

    IEnumerator ShowHintWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hintUI != null) hintUI.SetActive(true);
    }

    void DisableAll() => gameObject.SetActive(false);

    IEnumerator WaitForKeyDown()
    {
        _waitForPlayerClick = true;
        while (_waitForPlayerClick) yield return null;
    }

    IEnumerator AddLineWithTypewriter(string line)
    {
        string prefix = (openingText.text == "") ? "" : "\n\n";
        yield return Typewriter(openingText, prefix + line, true);
    }

    IEnumerator Typewriter(TMP_Text textObj, string content, bool append = false)
    {
        if (!append) textObj.text = "";
        foreach (char c in content)
        {
            textObj.text += c;
            if (typeSoundSource != null && !char.IsWhiteSpace(c))
                typeSoundSource.PlayOneShot(typeSoundSource.clip);
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    IEnumerator FadeOverlay(float start, float end, float duration)
    {
        if (blackOverlay == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            blackOverlay.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }
        blackOverlay.alpha = end;
    }

    IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        source.Stop();
        source.volume = startVol;
    }
}