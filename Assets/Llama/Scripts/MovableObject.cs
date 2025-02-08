using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [Header("Static Object")]
    public bool isStatic;
    public Sprite bgSprites;
    [Space]
    public Rigidbody2D rb;
    public Transform[] raycastPositions;
    public LayerMask gridLayer;
    public GameManager gameManager;
    public bool isPlayer;
    public bool isMoving;
    private SpriteRenderer spriteRenderer;
    [Tooltip("This is storing the last Position and then when it reaches to new position it will again store that position")]
    public Vector3 lastPosition;


    //bool to adjust the firstspawn moves to make them zero as they are spawned and counted
    private bool firstSpawn = true;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        MotionGridManager motionGridManager = MotionGridManager.Instance;
        if (!isPlayer)
        {
            if (isStatic)
            {
                spriteRenderer.color = Color.white;
                spriteRenderer.sprite = bgSprites;
                gameObject.GetComponent<MovableObject>().enabled = false;
            }
            else
            {
                int RanColor = Random.Range(0, motionGridManager.ObstaclesColors.Length);
                while (motionGridManager.ColorsUsedList.Contains(RanColor))
                {
                    RanColor = Random.Range(0, motionGridManager.ObstaclesColors.Length); // select a random Color from the list
                }

                motionGridManager.ColorsUsedList.Add(RanColor);
                // Enable or disable the SlideableObstacle component based on isStatic
                spriteRenderer.color = motionGridManager.ObstaclesColors[RanColor];
                var color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }
        
        
        isMoving = false;
        lastPosition = transform.position;
        SnapOnGrid();
    }
    public void SnapOnGrid()
    {
        //Stop rigidbody
        rb.velocity = Vector2.zero;
        //Stop trail
        isMoving = false;
        //Modify values in GameManager
        gameManager.directionDecided = false;
        gameManager.currentObjectScript = null;

        Transform[] gridSquaresTransform = new Transform[raycastPositions.Length];
        //Find grid squares below this object
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastPositions[i].position, Vector2.zero, 1.0f, gridLayer);

            // Check if the raycast hit something
            if (hit.collider != null)
            {
             //   Debug.Log("Grid square found.");
                gridSquaresTransform[i] = hit.collider.transform;
            }
            //If there is not grid square
            else
            {
                transform.position = lastPosition;
                return;
            }
        }
        //Calculate position to snap to
        Vector3 snapPosition;
        float xPosition = 0f;
        float yPosition = 0f;

        //Calculate xPosition
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            xPosition += gridSquaresTransform[i].position.x;
        }
        xPosition /= raycastPositions.Length;

        //Calculate yPosition
        for (int i = 0; i < raycastPositions.Length; i++)
        {
            yPosition += gridSquaresTransform[i].position.y;
        }
        yPosition /= raycastPositions.Length;

        snapPosition = new Vector3(xPosition, yPosition, 0.0f);

        //Place this object at correct position on grid
    //    Debug.Log("Snap position = " + xPosition.ToString() + "/" + yPosition.ToString());
        transform.position = snapPosition;
        if(firstSpawn)
        {
            //Save position
            lastPosition = transform.position;
            firstSpawn = false;
        }
        else
        {
            if (lastPosition != transform.position)
            {
                gameManager.IncreaseMoves();
            }
            //Save position
            lastPosition = transform.position;
        }
        
        
        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayer)
        {
            if (collision.CompareTag("Obstacle"))
                SnapOnGrid();
            if (collision.CompareTag("EndCircle"))
                rb.velocity = Vector2.zero;
        }
        else
        {
            if (collision.CompareTag("Obstacle") || collision.CompareTag("EndCircle") || collision.CompareTag("Player"))
                SnapOnGrid();
        }

    }

}
