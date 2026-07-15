using UnityEngine;

public class GazeDetector : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public GazeFeedbackUI feedbackUI;

    [Header("Gaze Settings")]
    public float maxDistance = 5f;
    public float dwellTime = 1f;

    private InspectableObject _currentTarget;
    private ScanHighlighter _currentScan;
    private float _timer;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (feedbackUI == null)
            feedbackUI = FindFirstObjectByType<GazeFeedbackUI>();

        if (feedbackUI != null)
            feedbackUI.SetIdle();
    }

    void Update()
    {
        if (feedbackUI != null)
        {
            if ((StoryUI.Instance != null && StoryUI.Instance.IsOpen) ||
                (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen))
            {
                feedbackUI.SetUIVisible(false);  // 隐藏准星和进度条
                ClearGaze();                     // 停止扫描
                return;
            }
            else
            {
                feedbackUI.SetUIVisible(true);   // UI 关闭时显示准星
            }
        }

        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            ClearGaze();
            return;
        }

        InspectableObject target = hit.collider.GetComponentInParent<InspectableObject>();

        if (target == null || !target.CanTrigger)
        {
            ClearGaze();
            return;
        }

        if (target != _currentTarget)
        {
            EndScanOnly();

            _currentTarget = target;
            _timer = 0f;

            _currentScan = target.GetComponentInParent<ScanHighlighter>();

            if (_currentScan != null)
            {
                _currentScan.BeginScan();
                _currentScan.SetScanProgress(0f);
            }
        }

        _timer += Time.deltaTime;

        float progress = dwellTime <= 0f
            ? 1f
            : Mathf.Clamp01(_timer / dwellTime);

        if (feedbackUI != null)
            feedbackUI.SetAiming(progress);

        if (_currentScan != null)
            _currentScan.SetScanProgress(progress);

        if (_timer >= dwellTime)
        {
            CompleteScan();
        }
    }

    void CompleteScan()
    {
        if (feedbackUI != null)
            feedbackUI.SetIdle();

        EndScanOnly();

        if (_currentTarget != null &&
            _currentTarget.storyCards != null &&
            _currentTarget.storyCards.Length > 0)
        {
            StoryData clue = _currentTarget.storyCards[0];

            if (ClueJournal.Instance != null)
                ClueJournal.Instance.AddClue(clue);

            if (StoryUI.Instance != null)
                StoryUI.Instance.ShowCard(clue);
        }

        if (_currentTarget != null)
            _currentTarget.MarkTriggered();

        _currentTarget = null;
        _timer = 0f;
    }

    void EndScanOnly()
    {
        if (_currentScan != null)
        {
            _currentScan.EndScan();
            _currentScan = null;
        }
    }

    void ClearGaze()
    {
        EndScanOnly();

        _currentTarget = null;
        _timer = 0f;

        if (feedbackUI != null)
            feedbackUI.SetIdle();
    }
}