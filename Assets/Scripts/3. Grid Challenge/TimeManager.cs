using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private float customTimeScale = 1f;
    private float customDeltaTime;
    private float lastFrameTime;

    public float CustomDeltaTime => customDeltaTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Update customDeltaTime based on customTimeScale
        float currentTime = Time.realtimeSinceStartup;
        customDeltaTime = (currentTime - lastFrameTime) * customTimeScale;
        lastFrameTime = currentTime;
    }

    public void SetCustomTimeScale(float newScale)
    {
        customTimeScale = newScale;
    }
}
