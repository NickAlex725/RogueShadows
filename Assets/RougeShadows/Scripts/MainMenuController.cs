using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _creditsUI;

    public void LoadLevel(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }

    public void Credits()
    {
        _mainMenuUI.SetActive(false);
        _creditsUI.SetActive(true);
    }

    public void MainMenu()
    {
        _mainMenuUI.SetActive(true);
        _creditsUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
