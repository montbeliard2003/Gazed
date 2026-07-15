using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    public StoryData[] storyCards;
    public bool triggerOnlyOnce = true;

    private bool _triggered;

    public bool CanTrigger => !triggerOnlyOnce || !_triggered;

    public void MarkTriggered()
    {
        _triggered = true;
    }
}