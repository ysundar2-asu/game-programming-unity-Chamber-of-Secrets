using System.Collections;
using UnityEngine;
using TMPro;

public class SkullClueInteract : MonoBehaviour
{
    [Header("Scene References")]
    public Transform skullMesh;              
    public Transform targetPosition;         
    public float moveTime = 0.8f;           

    [Header("UI Popup (Optional)")]
    public GameObject popupPanel;           
    public TextMeshProUGUI popupText;        
    [TextArea] public string clueText = "DECODE SER"; 

    [Header("Reveal Item (Optional)")]
    public GameObject revealObject;          
    public bool disableAfterUse = true;      

    private bool inRange = false;            
    private bool moved = false;              

    void Update()
    {
        if (!inRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!moved)
                StartCoroutine(SlideSkull());
            else
            {
                if (popupPanel)
                {
                    popupPanel.SetActive(!popupPanel.activeSelf);
                    if (popupText) popupText.text = clueText;
                }
            }
        }
    }

    IEnumerator SlideSkull()
    {
        if (!skullMesh || !targetPosition) yield break;
        moved = true;

        var idle = skullMesh.GetComponent<MonoBehaviour>();
        if (idle) idle.enabled = false;

        var anim = skullMesh.GetComponent<Animator>();
        if (anim) anim.enabled = false;

        Vector3 startPos = skullMesh.position;
        Quaternion startRot = skullMesh.rotation;
        Vector3 endPos = targetPosition.position;
        Quaternion endRot = targetPosition.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;
            float s = Mathf.SmoothStep(0f, 1f, t);
            skullMesh.position = Vector3.Lerp(startPos, endPos, s);
            skullMesh.rotation = Quaternion.Slerp(startRot, endRot, s);
            yield return null;
        }

        skullMesh.SetPositionAndRotation(endPos, endRot);

        if (revealObject) revealObject.SetActive(true);

        if (popupPanel)
        {
            popupPanel.SetActive(true);
            if (popupText) popupText.text = clueText;
        }

        if (disableAfterUse) enabled = false;
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
            if (popupPanel) popupPanel.SetActive(false);
        }
    }
}
