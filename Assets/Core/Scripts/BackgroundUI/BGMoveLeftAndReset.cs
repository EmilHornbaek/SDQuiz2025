using UnityEngine;

public class BGMoveLeftAndReset : MonoBehaviour
{
    [SerializeField] float minSpeed = 40f;
    [SerializeField] float maxSpeed = 80f;
    [SerializeField] float resetDistance = 800f;
    [SerializeField] float verticalOffsetOnReset = 50f;
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
        transform.Translate(Vector3.left * Time.deltaTime * speed);
        if (transform.position.x < startPos.x - resetDistance)
        {
            transform.position = startPos;
            transform.position += new Vector3(0, Random.Range(-verticalOffsetOnReset, verticalOffsetOnReset), 0);
        }
    }
}
