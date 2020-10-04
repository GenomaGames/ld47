using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
  public void OnStart()
  {
    LoadScene("Game");
  }

  public void OnExit()
  {
    Application.Quit();
  }

  public void LoadScene (string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }
}
