using UnityEngine;

public class BGMoveRightAndReturn : MonoBehaviour
{
    [SerializeField] float minSpeed = 0.2f;
    [SerializeField] float maxSpeed = 0.5f;
    [SerializeField] float resetDistance = 8f;
    [SerializeField] float verticalOffsetOnReset = 2f;
    private float speed;
    Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        if (transform.position.x > startPos.x + resetDistance)
        {
            transform.position = startPos;
            transform.position += new Vector3(0, Random.Range(-verticalOffsetOnReset, verticalOffsetOnReset), 0);
            speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
