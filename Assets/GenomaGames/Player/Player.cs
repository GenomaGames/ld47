using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
  public float OrbitalSpeed { get; private set; } = 0;
  public float DistanceToPlanetCenter { get => currentPlanet.Radius + lastAltitude; }

  public Image planetInRangeImage;
  public Planet currentPlanet;
  public bool isMovingClockwise = true;
  public float acceleration = 10;
  public float deceleration = 1;
  public float maxOrbitalSpeed = 10;
  public float altitudeChangeSpeed = 1;
  public float altitudeSpeedRatio = 1;

  private float lastAltitude = 0;
  private float maxAltitude = 0;
  private float lastMovementAngle = 0;
  private float spaceshipCenterHeight = 0;
  private Collider2D col2D;
  private Color planetInRangeOkColor = Color.green;
  private Color planetInRangeKoColor = Color.red;
  private Planet planetInRange;
  private Rigidbody2D rb2D;

  private void Awake()
  {
    rb2D = GetComponent<Rigidbody2D>();
    col2D = GetComponent<Collider2D>();
  }

  private void Start()
  {
    Bounds bounds = col2D.bounds;
    float heightPadding = 0.2f;
    planetInRangeImage.color = planetInRangeKoColor;

    spaceshipCenterHeight = bounds.center.y - bounds.min.y + heightPadding;
  }

  private void Update()
  {
    if (PauseMenu.Instance.IsPaused) return;

    Vector2 movement = GetMovementInput();
    float newAltitude = lastAltitude;

    if (movement.x != 0)
    {
      float speedChangeRatio = movement.x > 0 ? acceleration : deceleration;
      OrbitalSpeed += movement.x * speedChangeRatio * Time.deltaTime;

      if (OrbitalSpeed > maxOrbitalSpeed) OrbitalSpeed = maxOrbitalSpeed;
      if (OrbitalSpeed < 0) OrbitalSpeed = 0;
    }

    maxAltitude = OrbitalSpeed * altitudeSpeedRatio;

    if (movement.y != 0) newAltitude += movement.y * altitudeChangeSpeed * Time.deltaTime;
    if (newAltitude > maxAltitude) newAltitude = lastAltitude;
    if (newAltitude < 0) newAltitude = 0;
    if (lastAltitude > maxAltitude) newAltitude = Mathf.Lerp(lastAltitude, maxAltitude, altitudeChangeSpeed * Time.deltaTime);

    float newMovementAngle = lastMovementAngle + (isMovingClockwise ? 1 : -1) * OrbitalSpeed * Time.deltaTime;
    float radius = currentPlanet.Radius + newAltitude + spaceshipCenterHeight;
    Vector2 relativePosition = new Vector2(Mathf.Sin(newMovementAngle), Mathf.Cos(newMovementAngle)) * radius;
    Vector2 newPosition = currentPlanet.transform.position + (Vector3)relativePosition;

    lastAltitude = newAltitude;
    lastMovementAngle = newMovementAngle;

    rb2D.MovePosition(newPosition);
    rb2D.MoveRotation(-newMovementAngle * Mathf.Rad2Deg);

    if (Input.GetButton("Jump") && planetInRange != null)
    {
      ChangePlanet(planetInRange);
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Gravity Field"))
    {
      Planet planet = other.transform.parent.GetComponent<Planet>();

      if (planet != currentPlanet)
      {
        planetInRange = planet;
        planetInRangeImage.color = planetInRangeOkColor;
      }
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.CompareTag("Gravity Field"))
    {
      Planet planet = other.transform.parent.GetComponent<Planet>();

      if (planet == planetInRange)
      {
        planetInRange = null;
        planetInRangeImage.color = planetInRangeKoColor;
      }
    }
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawLine(transform.position, currentPlanet.transform.position);

    if (planetInRange)
    {
      Gizmos.color = Color.green;
      Gizmos.DrawLine(transform.position, planetInRange.transform.position);
      Gizmos.color = Color.white;
    }
  }

  private static Vector2 GetMovementInput()
  {
    bool isFire1Pressed = Input.GetButton("Fire1");
    bool isFire2Pressed = Input.GetButton("Fire2");
    float yAxis = Input.GetAxisRaw("Vertical");
    float xAxis = Input.GetAxisRaw("Horizontal");

    Vector2 movement = Vector2.zero;

    if (isFire1Pressed || xAxis > 0)
    {
      movement.x = 1;
    }
    else if (isFire2Pressed || xAxis < 0)
    {
      movement.x = -1;
    }

    if (yAxis != 0)
    {
      movement.y = yAxis > 0 ? 1 : -1;
    }

    return movement;
  }

  private void ChangePlanet (Planet planet)
  {
    currentPlanet = planet;
    planetInRange = null;
    planetInRangeImage.color = planetInRangeKoColor;

    float distanceToPlanetCenter = Vector2.Distance(transform.position, currentPlanet.transform.position);
    float newAltitude = distanceToPlanetCenter - currentPlanet.Radius;

    Vector2 relativePositionFromPlanet = transform.position - planet.transform.position;
    float unsignedAngleToPlanet = Vector2.Angle(Vector2.up, relativePositionFromPlanet);
    float sign = relativePositionFromPlanet.x > 0 ? 1 : -1;
    float signedAngleToPlanet = unsignedAngleToPlanet * sign;
    float newMovementAngle = signedAngleToPlanet * Mathf.Deg2Rad;

    isMovingClockwise = !isMovingClockwise;
    lastMovementAngle = newMovementAngle;
    lastAltitude = newAltitude;
  }

  // FIXED ROTATION

  //public float rotateSpeed = 1f;
  //public float radius = 1f;
  //public bool isMovingClockwise = true;

  //private float lastMovementAngle = 0;

  //private void Update()
  //{
  //  float movementAngle = lastMovementAngle + (isMovingClockwise ? 1 : -1) * rotateSpeed * Time.deltaTime;
  //  Vector2 relativePosition = new Vector2(Mathf.Sin(movementAngle), Mathf.Cos(lastMovementAngle)) * radius;

  //  lastMovementAngle = movementAngle;
  //  transform.position = target.position + (Vector3)relativePosition;
  //}

  //void OnDrawGizmos()
  //{
  //  Gizmos.DrawSphere(target.position, 0.1f);
  //  Gizmos.DrawLine(target.position, transform.position);
  //}


  // INITIAL ROTATION

  //public float Speed { get; private set; } = 0;

  //public Transform target;
  //public Planet currentPlanet;
  //private float currentAltitude = 0;
  //private float maxAltitude = 0;
  //private bool isInsideGravityField = false;

  //private void Start()
  //{
  //  if (currentPlanet != null)
  //  {
  //    Vector2 relativePosToPlanet = target.transform.position - transform.position;

  //    currentAltitude = relativePosToPlanet.magnitude;
  //    maxAltitude = currentPlanet.surfaceDistance;
  //  }
  //}

  //private void Update()
  //{
  //  Vector2 movement = Vector2.zero;

  //  movement.x = Speed * Time.deltaTime;

  //  bool isFire1Pressed = Input.GetButton("Fire1");
  //  bool isFire2Pressed = Input.GetButton("Fire2");
  //  float yAxis = Input.GetAxisRaw("Vertical");
  //  float xAxis = Input.GetAxisRaw("Horizontal");

  //  float speedVariation = 0;

  //  if (isFire1Pressed || xAxis > 0)
  //  {
  //    speedVariation = Mathf.Lerp(0, 30f, Time.deltaTime);
  //  } else if (isFire2Pressed || xAxis < 0)
  //  {
  //    speedVariation = Mathf.Lerp(0, -30f, Time.deltaTime);
  //  }

  //  Speed += speedVariation;

  //  if (Speed < 0) Speed = 0;

  //  if (isInsideGravityField)
  //  {
  //    Speed = Mathf.Lerp(Speed, 0, Time.deltaTime * 0.5f);
  //  }

  //  Vector2 relativePosToPlanet = target.transform.position - transform.position;
  //  currentAltitude = relativePosToPlanet.magnitude;

  //  //Debug.Log($"Current altitude: {currentAltitude}");

  //  float nextAltitude = currentAltitude + (yAxis * 0.01f);

  //  float spaceshipHeightToCenter = 0.3f;
  //  maxAltitude = currentPlanet.surfaceDistance + spaceshipHeightToCenter + (Speed * 0.1f);


  //  if (currentAltitude > maxAltitude)
  //  {
  //    nextAltitude = Mathf.Lerp(nextAltitude, maxAltitude, Time.deltaTime * 100);
  //    //Debug.Log($"Reducing altitude below max {maxAltitude} to {nextAltitude}");
  //  }

  //  //Debug.Log($"Next altitude: {nextAltitude}");
  //  float relativeAltitude = nextAltitude - currentAltitude;

  //  movement.y = relativeAltitude;

  //  //Debug.Log($"Movement: ({movement.x}, {movement.y})");

  //  Move(movement);
  //}

  //private void OnTriggerEnter2D(Collider2D other)
  //{
  //  if (other.CompareTag("Gravity Field"))
  //  {
  //    Planet planet = other.transform.parent.GetComponent<Planet>();

  //    if (planet == currentPlanet)
  //    {
  //      //Debug.Log($"Enter gravitiy field of planet: {planet.name}");
  //      isInsideGravityField = true;
  //    }
  //  }
  //}

  //private void OnTriggerExit2D(Collider2D other)
  //{
  //  if (other.CompareTag("Gravity Field"))
  //  {
  //    Planet planet = other.transform.parent.GetComponent<Planet>();

  //    if (planet == currentPlanet)
  //    {
  //      //Debug.Log($"Exit gravitiy field of planet: {planet.name}");
  //      isInsideGravityField = false;
  //    }
  //  }
  //}

  //private void Move(Vector2 relativePosition)
  //{
  //  Vector3 newPosition = target.transform.position + (Vector3)relativePosition;
  //  Vector3 newRelativePosToPlanet = newPosition - transform.position;
  //  float newZAngleToPlanet = (Mathf.Atan2(newRelativePosToPlanet.y, newRelativePosToPlanet.x) * Mathf.Rad2Deg) + 90;
  //  Quaternion newRotation = Quaternion.AngleAxis(newZAngleToPlanet, Vector3.forward);

  //  transform.localRotation = newRotation;
  //  transform.Translate(relativePosition, Space.Self);
  //}
}
