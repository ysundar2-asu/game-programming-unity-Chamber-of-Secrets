using UnityEngine;
using TMPro;

public class HintNote : MonoBehaviour
{
[SerializeField] private string hintText = "Find the Dragon Skull.";
    public GameObject prompt;               
    public GameObject popupPanel;     
    public TextMeshProUGUI popupText;        
    public Transform playerCamera;          
    public float useDistance = 2f;

    bool isPlayerInRange;

    void Update()
    {
        if (!isPlayerInRange) return;

        if (prompt) prompt.transform.LookAt(playerCamera);

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
