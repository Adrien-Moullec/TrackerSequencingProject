using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterEvents : MonoBehaviour
{
    [Header("Enter Area Events")]
    [SerializeField] UnityEvent onAwake;
    [SerializeField] UnityEvent onEnter;
    bool activated = false;
    void Awake()
    {
        onAwake.Invoke();
    }
    void Start()
    {
        UIManager.instance.TotalEventsCountUp();
    }
    // For level area activation
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        onEnter.Invoke();
        if (!activated) UIManager.instance.FoundEventsCountUp();
        activated = true;
    }
}
