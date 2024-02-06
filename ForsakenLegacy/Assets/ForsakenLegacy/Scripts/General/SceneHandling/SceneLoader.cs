using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Expose the scene name as a serialized field in the Unity Editor
    [SerializeField] private string sceneToLoad = "SceneName";


    // Method to load the specified scene
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
