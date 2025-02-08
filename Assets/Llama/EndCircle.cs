using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCircle : MonoBehaviour
{
    GameManager gameManager;
    bool endCoroutineRunning;
    private Transform redCircleTransform;
    private Vector3 redCircleStartPosition;
    MotionGridManager motionGridManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        motionGridManager = MotionGridManager.Instance;
        endCoroutineRunning = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    IEnumerator EndCoroutine(float time)
    {
        endCoroutineRunning = true;
        gameManager.currentObjectScript = null;
        gameManager.hasControls = false;

        //Move the red circle towards end circle
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            redCircleTransform.position = Vector3.Lerp(redCircleStartPosition, transform.position, t);
            yield return null;
        }
        Vector3 redCircleStartSize = redCircleTransform.localScale;
        Vector3 redCircleEndSize = new Vector3(0.1f, 0.1f, 1.0f);
        //Reduce size of red circle over time
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            redCircleTransform.localScale = Vector3.Lerp(redCircleStartSize, redCircleEndSize, t);
            yield return null;
        }
        Destroy(redCircleTransform.gameObject);
        if(!motionGridManager.PlaycustomLevel)
        {
            StartNewLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            redCircleTransform = collision.transform;
            redCircleStartPosition = collision.transform.position;
            if (!endCoroutineRunning)
            {
                StartCoroutine(EndCoroutine(0.5f));
            }
        }
    }

    public void StartNewLevel()
    {

        motionGridManager.CurrentLevel++;
        motionGridManager.DifficultyCount++;
        if (motionGridManager.DifficultyCount == 2)
        {
            motionGridManager.DifficultyCount = 0;
            motionGridManager.LevelDifficulty++;
        }
        
        motionGridManager.StartMotionGame();
    }

}
