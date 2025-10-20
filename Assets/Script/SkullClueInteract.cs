using System.Collections;
using UnityEngine;
using TMPro;

public class SkullClueInteract : MonoBehaviour
{
    [Header("Scene References")]
    public Transform skullMesh;          // Visible skull model to move
    public Transform targetPosition;     // Empty transform = destination
    public float moveTime = 0.8f;

    [Header("UI (optional)")]
    public GameObject popupPanel;        // Screen-space panel to show clue
    public TextMeshProUGUI popupText;    // TMP text inside panel
    [TextArea] public string clueText = "Look behind me... 3842";

    [Header("Reveal (optional)")]
    public GameObject revealObject;      // Hidden object to enable at target

    [Header("Auto-Hide Safety")]
    public Transform playerRoot;         // Drag Player root here
    public float autoHideDistance = 3.0f; // Hide popup if player moves farther than this

    // State
    bool inRange;
    bool isMoving;
    bool atTarget;

    // Original pose
    Vector3 origPos;
    Quaternion origRot;

    void Awake()
    {
        if (!skullMesh) skullMesh = transform; // fallback

        origPos = skullMesh.position;
        origRot = skullMesh.rotation;

        var anim = skullMesh.GetComponent<Animator>();
        if (anim) anim.enabled = false;

        if (popupPanel) popupPanel.SetActive(false);
        if (revealObject) revealObject.SetActive(false);
    }

    void Update()
    {
        // SAFETY: auto-hide popup if player walked away
        if (popupPanel && popupPanel.activeSelf && playerRoot)
        {
            float dist = Vector3.Distance(playerRoot.position, skullMesh.position);
            if (dist > autoHideDistance) popupPanel.SetActive(false);
        }

        if (!inRange || isMoving) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (atTarget)
            {
                // Toggle popup first (no movement)
                if (popupPanel)
                {
                    bool nowActive = !popupPanel.activeSelf;
                    popupPanel.SetActive(nowActive);
                    if (nowActive && popupText) popupText.text = clueText;

                    // If we just toggled it on/off, stop here.
                    if (nowActive) return;
                }

                // Popup either doesn't exist or is already hidden -> move back
                StartCoroutine(MoveSkull(skullMesh.position, skullMesh.rotation, origPos, origRot, toTarget: false));
            }
            else
            {
                // Move to reveal position
                StartCoroutine(MoveSkull(origPos, origRot, targetPosition.position, targetPosition.rotation, toTarget: true));
            }
        }
    }

    IEnumerator MoveSkull(Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot, bool toTarget)
    {
        isMoving = true;

        var anim = skullMesh.GetComponent<Animator>();
        if (anim) anim.enabled = false;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / moveTime;
            float s = Mathf.SmoothStep(0f, 1f, t);
            skullMesh.position = Vector3.Lerp(fromPos, toPos, s);
            skullMesh.rotation = Quaternion.Slerp(fromRot, toRot, s);
            yield return null;
        }

        skullMesh.SetPositionAndRotation(toPos, toRot);

        atTarget = toTarget;
        isMoving = false;

        if (toTarget)
        {
            if (revealObject) revealObject.SetActive(true);
            if (popupPanel)
            {
                if (popupText) popupText.text = clueText;
                popupPanel.SetActive(true); // show once on arrival
            }
        }
        else
        {
            if (revealObject) revealObject.SetActive(false);
            if (popupPanel) popupPanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) inRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            // Force-hide popup when leaving range
            if (popupPanel) popupPanel.SetActive(false);
        }
    }
}
