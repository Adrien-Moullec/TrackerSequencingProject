using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterEvents : MonoBehaviour
{
    [Header("Enter Area Events")]
    [Tooltip("Events that activate on enter.")]
    [SerializeField] UnityEvent onAwake;
    [Tooltip("Events that activate on exit.")]
    [SerializeField] UnityEvent onEnter;

    [Tooltip("Is the music activated at least once.")]
    bool activated = false;

    /// <summary>
    /// On awake events
    /// </summary>
    void Awake()
    {
        onAwake.Invoke();
    }

    /// <summary>
    /// On enter events
    /// </summary>
    void Start()
    {
        UIManager.instance.TotalEventsCountUp();
    }

    /// <summary>
    /// For level area activation
    /// </summary>
    /// <param name="other"> Entering collider </param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        onEnter.Invoke();
        if (!activated) UIManager.instance.FoundEventsCountUp();
        activated = true;
    }
}
