using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterEvents : MonoBehaviour
{
    [Header("Enter Area Events")]
    [SerializeField] UnityEvent onAwake;
    [SerializeField] UnityEvent onEnter;
    void Awake()
    {
        onAwake.Invoke();
    }
    // For level area activation
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        onEnter.Invoke();
    }
}
