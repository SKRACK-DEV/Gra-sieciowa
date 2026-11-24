using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string lobbyceneName = "lobby";

    public void StartGame()
    {
        SceneManager.LoadScene(lobbyceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR  //to do edycji później bo to tylko do testowania w editorze, nie potrzebne w final wersji
        UnityEditor.EditorApplication.isPlaying = false;  

#else
            Application.Quit();

#endif

    }
}
