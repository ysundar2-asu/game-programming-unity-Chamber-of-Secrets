using UnityEngine;
using TMPro;

public class HintNote : MonoBehaviour
{
[SerializeField] private string hintText = "Find the Door Lock code.";
    public GameObject prompt;                 // the world-space "Press E to read"
    public GameObject popupPanel;             // a screen-space Panel
    public TextMeshProUGUI popupText;         // TMP text inside the panel
    public Transform playerCamera;            // usually Main Camera
    public float useDistance = 2f;

    bool isPlayerInRange;

    void Update()
    {
        if (!isPlayerInRange) return;

        // Optional: face the prompt toward the camera
        if (prompt) prompt.transform.LookAt(playerCamera);

        // Let player read/close
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool showing = popupPanel.activeSelf;
            popupPanel.SetActive(!showing);
            if (!showing) popupText.text = hintText;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (prompt) prompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (prompt) prompt.SetActive(false);
            if (popupPanel) popupPanel.SetActive(false);
        }
    }
}
