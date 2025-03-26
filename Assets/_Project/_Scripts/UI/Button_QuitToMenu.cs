using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_QuitToMenu : Button_Base
{
    protected override void OnClick()
    {
        // Optionally perform any cleanup or saving here
        Time.timeScale = 1f; // Ensure time is running when quitting
        // Load the main menu scene with index 1
        SceneManager.LoadScene(1);
        
        Debug.Log("Quit to Menu");
    }
}
