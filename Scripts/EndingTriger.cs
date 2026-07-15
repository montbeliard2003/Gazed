using System.Collections;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public EndingManager endingManager;
    public int requiredClues = 8;

    private bool _endingQueued = false;
    private bool _endingStarted = false;

    void Update()
    {
        if (_endingStarted) return;
        if (GameState.IsOpening || GameState.IsEnding) return;
        if (ClueJournal.Instance == null) return;

        if (!_endingQueued && ClueJournal.Instance.CollectedClues.Count >= requiredClues)
        {
            _endingQueued = true;
            StartCoroutine(WaitAndStartEnding());
        }
    }

    IEnumerator WaitAndStartEnding()
    {
        yield return null;

        while ((StoryUI.Instance != null && StoryUI.Instance.IsOpen) ||
               (NotebookUI.Instance != null && NotebookUI.Instance.IsOpen))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (!_endingStarted && endingManager != null)
        {
            _endingStarted = true;
            endingManager.BeginEnding();
        }
        else if (endingManager == null)
        {
            Debug.LogError("EndingTrigger: endingManager Ã»ÓÐ°ó¶¨¡£");
        }
    }
}