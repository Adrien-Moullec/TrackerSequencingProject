using UnityEngine;

public class QuestionMarkSpin : MonoBehaviour
{
    [Header("Rotate speed")]
    [SerializeField] float spinSpeed = 1;
    [SerializeField] float bobSpeed = 1;
    [SerializeField] float bobHeight = 3;

    private Vector3 startPos;
    private float time = 0;
    void Awake()
    {
        startPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(time) * bobHeight);
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        time += Time.deltaTime * bobSpeed;
        time %= 90;
    }
}
