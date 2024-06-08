using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MainScene";
    [SerializeField] private GameObject loadingScreen; // Assign a UI Panel for the loading screen
    [SerializeField] private GameObject loadingSpinner; // Optional: Assign a spinner or progress bar for loading animation

    private static bool isNewGame = true;

    // Method to start a new game
    public void StartNewGame()
    {
        isNewGame = true;
        StartCoroutine(LoadSceneAsync());
    }

    // Method to load the game
    public void LoadGame()
    {
        isNewGame = false;
        StartCoroutine(LoadSceneAsync());
    }

    // Method to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    private IEnumerator LoadSceneAsync()
    {
        // Show the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Optionally, you can add a delay here to simulate loading time
        yield return new WaitForSeconds(1f);

        // Load the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Once the scene is loaded, hide the loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    // Method to determine if it's a new game or loading a game
    public static bool IsNewGame()
    {
        return isNewGame;
    }
}