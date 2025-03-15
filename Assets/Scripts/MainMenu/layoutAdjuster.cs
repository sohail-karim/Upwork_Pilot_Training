using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class LayoutAdjuster : MonoBehaviour
{
    private RectTransform parentRect;
    private RectTransform[] children;

    void Start()
    {
        parentRect = GetComponent<RectTransform>();

        if (transform.childCount != 3)
        {
            Debug.LogError("This script requires exactly 3 children!");
            return;
        }

        children = new RectTransform[3];
        for (int i = 0; i < 3; i++)
        {
            children[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }

        AdjustSizes();
    }

    void AdjustSizes()
    {
        float parentWidth = parentRect.rect.width;

        children[0].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth * 0.2f);
        children[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth * 0.6f);
        children[2].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentWidth * 0.2f);
    }

    void Update()
    {
        AdjustSizes(); // Ensures it adapts if parent resizes dynamically
    }
}
