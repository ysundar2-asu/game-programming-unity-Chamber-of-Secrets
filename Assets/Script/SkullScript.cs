using UnityEngine;

public class SkullBobbing : MonoBehaviour
{
    public float bobAmplitude = 0.05f;
    public float bobSpeed = 1.5f;
    public float yawSpeed = 20f;

    Vector3 startPos;

    void Start() { startPos = transform.localPosition; }

    void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.localPosition = startPos + new Vector3(0f, y, 0f);
        transform.Rotate(0f, yawSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
