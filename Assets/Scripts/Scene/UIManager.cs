using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Tooltip("UIManager instance to reference in scene.")]
    public static UIManager instance;
    [Tooltip("Text in UI reference.")]
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [Tooltip("Template for event finding message.")]
    string messageTemplate = "Events left to find: ";
    [Tooltip("Total events to find.")]
    int totalEvents = 0;
    [Tooltip("Total events found.")]
    int foundEvents = 0;

    /// <summary>
    /// Set instance
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Update text message using message template and number of found messages
    /// </summary>
    void UpdateMessage()
    {
        textMeshProUGUI.text = messageTemplate + foundEvents + " / " + totalEvents;
        if (totalEvents == foundEvents) textMeshProUGUI.text = "You found everything, thanks for playing. Feel free to play the sounds again.";
    }
    /// <summary>
    /// Update number of found events.
    /// </summary>
    public void FoundEventsCountUp()
    {
        foundEvents++;
        UpdateMessage();
    }
    /// <summary>
    /// Update total number of events.
    /// </summary>
    public void TotalEventsCountUp()
    {
        totalEvents++;
        UpdateMessage();
    }
}
