using UnityEngine;

public class QuestionMarkSpin : MonoBehaviour
{
    [Header("Rotate speed")]
    [Tooltip("Spin speed of question mark.")]
    [SerializeField] float spinSpeed = 1;
    [Tooltip("Bobbing speed of question mark.")]
    [SerializeField] float bobSpeed = 1;
    [Tooltip("Bob height change of the question mark.")]
    [SerializeField] float bobHeight = 3;

    [Tooltip("Starting position of the question mark assigned at awake.")]
    private Vector3 startPos;
    [Tooltip("Current time of question mark spin.")]
    private float time = 0;

    /// <summary>
    /// Store start position of question mark
    /// </summary>
    void Awake()
    {
        startPos = transform.position;
    }

    /// <summary>
    /// Spin and bob question mark
    /// </summary>
    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(time) * bobHeight);
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        time += Time.deltaTime * bobSpeed;
        time %= 90;
    }
}
