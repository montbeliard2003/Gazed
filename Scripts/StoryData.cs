using UnityEngine;

[CreateAssetMenu(menuName = "Story/Story Data", fileName = "StoryData_")]
public class StoryData : ScriptableObject
{
    [Header("Basic Info")]
    public string clueId;
    public string title;

    [TextArea(3, 10)]
    public string body;

    [Header("Notebook")]
    public Sprite photo;

    [TextArea(2, 5)]
    public string notebookText;
}