using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    string messageTemplate = "Events left to find: ";
    int totalEvents = 0;
    int foundEvents = 0;

    void Awake()
    {
        instance = this;
    }

    void UpdateMessage()
    {
        textMeshProUGUI.text = messageTemplate + foundEvents + " / " + totalEvents;
        if (totalEvents == foundEvents) textMeshProUGUI.text = "You found everything, thanks for playing. Feel free to play the sounds again.";
    }
    public void FoundEventsCountUp()
    {
        foundEvents++;
        UpdateMessage();
    }
    public void TotalEventsCountUp()
    {
        totalEvents++;
        UpdateMessage();
    }
}
