using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public int minBalls = 8;
    public int maxBalls = 13;
    [Space]
 

    private int ballCount;
    public Vector2 spawnRange = new Vector2(2f, 2f);

    private static BallManager _instance;

    public static BallManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BallManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        _instance = this;
    }

    void Start()
    {
        
     
    //   SpawnBalls();
      
    }




    public void SpawnBalls()
    {


        // Randomly select the number of balls to spawn within the range
        ballCount = Random.Range(minBalls, maxBalls + 1);

        for (int i = 0; i < ballCount; i++)
        {
            // Random radius (distance from the center) within the sphere's radius
            float radius = 2f; // spawnRange.x is the radius of the sphere

            // Random angle in spherical coordinates
            float theta = Random.Range(0f, 2f * Mathf.PI); // Random angle around the Y-axis
            float phi = Random.Range(0f, Mathf.PI); // Random angle from the vertical (Z-axis)

            // Convert spherical coordinates to Cartesian coordinates
            float xPos = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float yPos = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float zPos = radius * Mathf.Cos(phi);

            // Create the spawn position (ignoring the Z-axis for 2D game)
            Vector3 spawnPosition = new Vector3(xPos, yPos, 0);  // Assuming 2D

            // Instantiate the ball at the calculated position
            Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        }
    }


    public int GetBallCount()
    {
        return ballCount;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }

}
