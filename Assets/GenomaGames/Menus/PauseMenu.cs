using UnityEngine;

public class PauseMenu : MonoBehaviour
{
  public static PauseMenu Instance
  {
    get;
    private set;
  }

  public bool IsPaused { get; private set; }

  public GameObject canvas;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = FindObjectOfType<PauseMenu>();
    } else
    {
      throw new UnityException("No Pause Menu in Scene");
    }
  }

  private void Start()
  {
    IsPaused = canvas.activeSelf;
  }

  private void Update()
  {
    if (Input.GetButtonDown("Cancel"))
    {
      TogglePause();
    }
  }

  public void TogglePause()
  {
    IsPaused = !IsPaused;


    canvas.SetActive(IsPaused);
  }
}
