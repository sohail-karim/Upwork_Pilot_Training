using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 5f;                  // Speed of the balls
    public float circleRadius;               // Automatically calculated radius
    private Vector2 circleCenter;             // Center position of the circle
    private Rigidbody2D rb;
    private float boundaryOffset = 0.1f;      // Offset to keep the ball inside the boundary
    private Vector2 targetPosition;           // New target position inside the circle
    private float targetChangeDelay;     // Time delay before changing target (seconds)
    private float timeSinceLastChange = 0f;   // Track time since last target change
    private float currentSpeed;               // Current speed of the ball for smooth acceleration
    public float slowingTime = 1f;           // Time to slow down before target change
    public float accelerationTime = 2f;      // Time to accelerate after target change



    #region Instance

    private static BallMovement _instance;

    public static BallMovement instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BallMovement>();
            }

            return _instance;
        }
    }

    #endregion

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find the Circle GameObject and calculate its radius
        GameObject circle = GameObject.Find("CircleArea");
        if (circle != null)
        {
            circleCenter = circle.transform.position;

            // Get the radius by measuring the extents of the sprite's bounds
            SpriteRenderer circleRenderer = circle.GetComponent<SpriteRenderer>();
            //    circleRadius = circleRenderer.bounds.extents.x;
        }
        else
        {
            Debug.LogError("CircleArea GameObject not found!");
        }

        // Set an initial random direction
        SetNewRandomDirection();
        targetChangeDelay = Random.Range(2f, 2.5f);

    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero; // Stop the ball when disabled
    }


    //Whenever user clicks on Ans Buttons this function will again instentiate balls.
    public void NextLevel()
    {
        SetNewRandomDirection();
        targetChangeDelay = Random.Range(2f, 2.5f);
    }

    void FixedUpdate()
    {
        // Track time since the last target change
        timeSinceLastChange += Time.deltaTime;

        // If the delay has passed, change the target position
        if (timeSinceLastChange >= targetChangeDelay)
        {
            // Slowly decrease the speed before changing target
            SlowDownBeforeTargetChange();

            // Once slowed down, set the new target and accelerate the ball
            if (currentSpeed <= 0.1f)  // Speed is low enough to change target
            {
                SetNewRandomDirection();
                AccelerateAfterTargetChange();
                timeSinceLastChange = 0f;  // Reset the timer

                targetChangeDelay = Random.Range(2f, 2.5f);

            }
        }

        // Move the ball based on its current velocity
        rb.velocity = rb.velocity.normalized * currentSpeed;

        // Check if the ball is outside the circle radius
        Vector2 position = transform.position;
        float distanceFromCenter = Vector2.Distance(position, circleCenter);

        if (distanceFromCenter > circleRadius)
        {
            // Reflect the ball's direction at the boundary
            ReflectBall(position);

            // Move the ball back within bounds with a slight offset
            transform.position = circleCenter + (position - circleCenter).normalized * (circleRadius - boundaryOffset);
        }
    }

    void SetNewRandomDirection()
    {
        // Generate a random direction for the ball to start moving
        float randomAngle = Random.Range(0f, 1.5f * Mathf.PI);
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        // Set the ball's initial velocity
        currentSpeed = speed;
        rb.velocity = direction * currentSpeed;
    }

    void ReflectBall(Vector2 currentPosition)
    {
        // Calculate the normal vector at the boundary point
        Vector2 normal = (currentPosition - circleCenter).normalized;

        // Calculate the reflection of the velocity vector
        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, normal);

        // Apply the reflected velocity
        rb.velocity = reflectedVelocity.normalized * currentSpeed;
    }

    // Slow down the ball before changing the target position
    void SlowDownBeforeTargetChange()
    {
        // Gradually reduce the current speed until it reaches a minimum threshold (0)
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime / slowingTime);
    }

    // Accelerate the ball after the target change
    void AccelerateAfterTargetChange()
    {
        // Gradually increase the current speed to the original speed (acceleration effect)
        currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime / accelerationTime);
    }
}