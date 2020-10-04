using UnityEngine;

public class CameraFollow : MonoBehaviour
{

  public Player target;
  public Planet sun;
  public float followSpeed = 1;
  public float zoomSpeedRatio = 0.5f;
  public float zoomAltitudeRatio = 0.5f;

  private Camera cam;
  private float minOrthographicSize;

  private void Awake()
  {
    cam = GetComponent<Camera>();
  }

  private void Start()
  {
    minOrthographicSize = cam.orthographicSize;
  }

  private void LateUpdate()
  {
    if (PauseMenu.Instance.IsPaused)
    {
      Vector3 pausePosition = transform.position;

      pausePosition.x = sun.transform.position.x;
      pausePosition.y = sun.transform.position.y;

      transform.position = Vector3.Lerp(transform.position, pausePosition, Time.deltaTime * 10);

      float pauseOthographicSize = 200;

      if (pauseOthographicSize > minOrthographicSize)
      {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, pauseOthographicSize, Time.deltaTime * 10);
      }
    }

    Vector3 newPosition = transform.position;

    newPosition.x = target.currentPlanet.transform.position.x;
    newPosition.y = target.currentPlanet.transform.position.y;

    transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followSpeed);

    float newOrthographicSize = (target.OrbitalSpeed * zoomSpeedRatio) + (target.DistanceToPlanetCenter * zoomAltitudeRatio);

    if (newOrthographicSize > minOrthographicSize)
    {
      cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newOrthographicSize, Time.deltaTime);
    }
  }
}
