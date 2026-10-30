using UnityEngine;

public class GiantCoinEffect : MonoBehaviour
{
    public float floatSpeed = 3f;
    public float floatAmount = 0.15f;
    public float rotateSpeed = 60f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
