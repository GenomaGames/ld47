using UnityEngine;

public class StarField : MonoBehaviour
{
  public Camera cam;
  public float moveRatio = 0;

  private Vector2 initialPosition;

  private void Start()
  {
    initialPosition = transform.position;
  }

  private void Update()
  {
    Vector2 newPosition = Vector2.Lerp(initialPosition, cam.transform.position, moveRatio * Time.deltaTime);
    transform.Translate(newPosition);
  }
}
