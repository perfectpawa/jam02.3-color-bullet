using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
/*    [SerializeField] private GameObject _optionsMenu;*/
    [SerializeField] private GameObject _bulletsMenu;
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void BulletMenu()
    {
        _mainMenu.SetActive(false);
        _bulletsMenu.SetActive(true);
    }
/*    public void OptionsMenu()
    {
        _mainMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }*/
    public void BackToMainMenu()
    {
        _mainMenu.SetActive(true);
/*        _optionsMenu.SetActive(false);*/
        _bulletsMenu.SetActive(false);
    }
}
