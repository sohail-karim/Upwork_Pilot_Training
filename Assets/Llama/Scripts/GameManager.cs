using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Managing")]
    public Slider slider_timer;
    public TextMeshProUGUI timer_Text;
    public TextMeshProUGUI level_Text;
    public TextMeshProUGUI moves_Text;
    public Button skip_Button;
    public Button ExitButton;
    private int movesDone = 0;

    public MovableObject currentObjectScript;
    private Touch currentTouch;
    public LayerMask touchLayerMask;
    public bool hasControls;
    private Vector3 currentTouchStartPosition;
    public bool directionDecided;
    private bool moveDirection; // if true, move on X, if false, move on Y
    private bool movePositive;
    [SerializeField] private float moveSpeed;
    public float touchDistance;
    [SerializeField] private float lerpSpeed = 5.0f; // Speed of lerp interpolation
    [SerializeField] private float maxSpeed = 10f; // Maximum movement speed

    private Vector3 targetPosition;
    private MotionGridManager motionGridManager;

    void Start()
    {

        slider_timer = GameObject.Find("Slider_Timer").GetComponent<Slider>();
        timer_Text = GameObject.Find("Timer_Text").GetComponent<TextMeshProUGUI>();
        level_Text = GameObject.Find("Level_Text").GetComponent<TextMeshProUGUI>();
        moves_Text = GameObject.Find("Moves_Text").GetComponent<TextMeshProUGUI>();
        skip_Button = GameObject.Find("Skip_Button").GetComponent<Button>();
        ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();

        Camera.main.orthographicSize = 6f;
        motionGridManager = MotionGridManager.Instance;

        if (motionGridManager.isEditor)
        {
            lerpSpeed = 15f;
            maxSpeed = 15f;
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
            //   lerpSpeed = PlayerPrefs.GetFloat("LerpSpeed",10f);
            //   maxSpeed = PlayerPrefs.GetFloat("MaxSpeed", 5f);
            lerpSpeed = 10f;
            maxSpeed = 5f;
        }
        ExitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        skip_Button.onClick.AddListener(() =>
        {
            motionGridManager.LevelSkip();
        });
        hasControls = true;
        directionDecided = false;
        slider_timer.maxValue = motionGridManager.MaxTime;
        level_Text.text = $"Level {motionGridManager.CurrentLevel}";
    }

    void Update()
    {
        if (motionGridManager != null)
        {
            int minutes = Mathf.FloorToInt(motionGridManager.timeRemaining / 60);
            int seconds = Mathf.FloorToInt(motionGridManager.timeRemaining % 60);
            timer_Text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            slider_timer.value = motionGridManager.timeRemaining;
        }

        if (hasControls)
        {
            if (Input.touchCount > 0)
            {
                currentTouch = Input.GetTouch(0);

                if (currentTouch.phase == TouchPhase.Began)
                {
                    Vector2 touchPosition = currentTouch.position;
                    currentTouchStartPosition = currentTouch.position;

                    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                    RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, 1.0f, touchLayerMask);

                    if (hit.collider != null)
                    {
                        currentObjectScript = hit.collider.GetComponent<MovableObject>();

                        if (currentObjectScript.isStatic)
                        {
                            currentObjectScript = null;
                            return;
                        }

                        currentObjectScript.isMoving = true;
                  //      Debug.Log($"Hit object: {hit.collider.name}");
                    }
                    else
                    {
                    //    Debug.Log("No object hit");
                    }
                }
            }

            if (currentObjectScript != null)
            {
                if (currentTouch.phase == TouchPhase.Moved)
                {
                    if (!directionDecided)
                    {
                        directionDecided = true;
                        if (Mathf.Abs(currentTouchStartPosition.x - currentTouch.position.x) > Mathf.Abs(currentTouchStartPosition.y - currentTouch.position.y))
                        {
                            moveDirection = true;
                            movePositive = currentTouchStartPosition.x < currentTouch.position.x;
                        }
                        else
                        {
                            moveDirection = false;
                            movePositive = currentTouchStartPosition.y < currentTouch.position.y;
                        }
                    }
                    else
                    {
                        Vector2 touchPosition = currentTouch.position;
                        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                        Vector3 worldPosition3D = new Vector3(worldPosition.x, worldPosition.y, 0.0f);

                        if (moveDirection)
                        {
                            targetPosition = new Vector3(worldPosition.x, currentObjectScript.transform.position.y, 0.0f);
                        }
                        else
                        {
                            targetPosition = new Vector3(currentObjectScript.transform.position.x, worldPosition.y, 0.0f);
                        }

                        Vector3 newPosition = Vector3.Lerp(currentObjectScript.transform.position, targetPosition, lerpSpeed * Time.deltaTime);

                        // Limit the movement speed
                        Vector3 movementDelta = newPosition - currentObjectScript.transform.position;
                        movementDelta = Vector3.ClampMagnitude(movementDelta, maxSpeed * Time.deltaTime);

                        currentObjectScript.transform.position += movementDelta;
                    }
                }

                if (currentTouch.phase == TouchPhase.Ended)
                {
                    currentObjectScript.SnapOnGrid();
                    currentObjectScript = null;
                    directionDecided = false;
                }
            }
        }
    }

    public void IncreaseMoves()
    {
        movesDone++;
        moves_Text.text = $"{movesDone}";
    }
}
