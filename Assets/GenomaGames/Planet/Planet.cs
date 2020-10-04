using UnityEngine;

public class Planet : MonoBehaviour
{
  public float Radius => cCol2D.radius;

  public bool isMovingClockwise = true;
  public float orbitalSpeed = 1;
  public Planet parentPlanet;

  private float distanceFromParent;
  private float lastMovementAngle;
  private CircleCollider2D cCol2D;
  private Rigidbody2D rb2D;

  private void Awake()
  {
    cCol2D = GetComponent<CircleCollider2D>();
    rb2D = GetComponent<Rigidbody2D>();
  }

  private void Start()
  {
    if (parentPlanet == null) return;

    Vector2 relativePositionFromPlanet = transform.position - parentPlanet.transform.position;
    float unsignedAngleToPlanet = Vector2.Angle(Vector2.up, relativePositionFromPlanet);
    float sign = relativePositionFromPlanet.x > 0 ? 1 : -1;
    float signedAngleToPlanet = unsignedAngleToPlanet * sign;

    distanceFromParent = Vector2.Distance(transform.position, parentPlanet.transform.position);
    lastMovementAngle = signedAngleToPlanet * Mathf.Deg2Rad;
  }

  private void Update()
  {
    if (PauseMenu.Instance.IsPaused || parentPlanet == null) return;

    float newMovementAngle = lastMovementAngle + (isMovingClockwise ? 1 : -1) * orbitalSpeed * Time.deltaTime;
    float radius = distanceFromParent;
    Vector2 relativePosition = new Vector2(Mathf.Sin(newMovementAngle), Mathf.Cos(newMovementAngle)) * radius;
    Vector2 newPosition = parentPlanet.transform.position + (Vector3)relativePosition;

    lastMovementAngle = newMovementAngle;

    rb2D.MovePosition(newPosition);
  }
}
