using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Called by the Start button
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // <-- exact scene name
    }

    // Optional: hook to a Quit button
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit requested");
    }
}