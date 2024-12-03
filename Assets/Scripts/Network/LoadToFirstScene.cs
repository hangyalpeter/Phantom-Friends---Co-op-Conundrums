using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadToFirstScene : MonoBehaviour
{
    private void Start()
    {
        UIScreenEvents.HideAllScreens?.Invoke();
        SceneManager.LoadScene(Scene.First.ToString());
    }

}
