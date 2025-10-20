using UnityEngine;

public class door : MonoBehaviour
{
    public enum HingeAxis { X, Y, Z }

    [Header("Door Settings")]
    public HingeAxis hingeAxis = HingeAxis.Y;   // For side-hinged 3D doors use Y; for side-view 2D use Z
    public float openAngle = 90f;
    public float openSpeed = 4f;
    public bool isOpen;

    [Header("Player Interaction")]
    public Transform player;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    Quaternion closedRot, openRot;

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        closedRot = transform.localRotation;
        openRot   = Quaternion.Euler(transform.localEulerAngles + AxisVector() * openAngle);
    }

    void Update()
    {
        if (!player) return;

        float d = Vector3.Distance(player.position, transform.position);
        if (d <= interactDistance && Input.GetKeyDown(interactKey))
            isOpen = !isOpen;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            isOpen ? openRot : closedRot,
            Time.deltaTime * openSpeed
        );
    }

    Vector3 AxisVector()
    {
        switch (hingeAxis)
        {
            case HingeAxis.X: return Vector3.right;
            case HingeAxis.Y: return Vector3.up;
            default:          return Vector3.forward; // Z
        }
    }
}
