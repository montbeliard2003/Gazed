using System.Collections.Generic;
using UnityEngine;

public class ClueJournal : MonoBehaviour
{
    public static ClueJournal Instance { get; private set; }

    private HashSet<string> _collectedIds = new HashSet<string>();
    private List<StoryData> _collectedClues = new List<StoryData>();

    public IReadOnlyList<StoryData> CollectedClues => _collectedClues;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool HasClue(string clueId)
    {
        if (string.IsNullOrEmpty(clueId)) return false;
        return _collectedIds.Contains(clueId);
    }

    public bool AddClue(StoryData clue)
    {
        if (clue == null) return false;
        if (string.IsNullOrEmpty(clue.clueId)) return false;

        if (_collectedIds.Contains(clue.clueId))
            return false;

        _collectedIds.Add(clue.clueId);
        _collectedClues.Add(clue);

        return true;
    }
}